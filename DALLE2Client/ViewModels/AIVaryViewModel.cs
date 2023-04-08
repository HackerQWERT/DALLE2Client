
namespace DALLE2Client.ViewModels;

public partial class AIVaryViewModel:BaseViewModel
{
    public AIVaryViewModel()
    {
        RegisterSignalR();
    }
   
    [ObservableProperty]
    private ObservableCollection<ImageModel> upLoadImagesList=new ObservableCollection<ImageModel>();


    [RelayCommand]
    async Task CancelUpLoadImageAsync()
    {
        UpLoadImagesList.Clear();
        App.Current.Dispatcher.Dispatch(async () => await ShowToast("已取消"));
    }

    [RelayCommand]
    async Task RefreshAsync()
    {
        IsRefreshing = true;
        try
        {
            DateTime startTime = DateTime.Now;
            if (SignalR.DALLE2Connection.State is not HubConnectionState.Connected)
            {
                await SignalR.DALLE2Connection.StopAsync();

                SignalR.DALLE2Connection.StartAsync();
                while (true)
                {
                    await Task.Delay(1000);
                    if (SignalR.DALLE2Connection.State is HubConnectionState.Connected)
                    {
                        WebConnectionIconAddress = "连接";
                        break;
                    }
                    else if (DateTime.Now - startTime > TimeSpan.FromSeconds(5))
                    {
                        await SignalR.DALLE2Connection.StopAsync();
                        break;
                    }
                }
            }
        }
        catch (Exception? ex)
        {
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    async Task UpLoadImageAsync()
    {
        var result = await MediaPicker.PickPhotoAsync();
        if (result != null)
        {
            using FileStream imageFileStream = new FileStream(result.FullPath, FileMode.Open, FileAccess.Read);
   
            MemoryStream pngStream = new MemoryStream();
            try
            {
                //判断大小是否小于4M
                if (imageFileStream.Length > 4 * 1024 * 1024)
                {
                    App.Current.Dispatcher.Dispatch(async () => await Toast.Make("图片超过4M").Show(CancellationToken.None));
                    return;
                }
                var selectBitmap = SKBitmap.Decode(imageFileStream);
 
                //判断是否为方形
                if (selectBitmap.Width != selectBitmap.Height)
                {
                    App.Current.Dispatcher.Dispatch(async () => await Toast.Make("图片不是方形，请上传长宽一致的图片哦").Show(CancellationToken.None));
                    return;
                }
                //SKCodec codec = SKCodec.Create(new MemoryStream(selectBitmap.Bytes));
                //// 判断是否为PNG格式
                //if (codec.EncodedFormat is not SKEncodedImageFormat.Png)
                //{
                //}
                selectBitmap.Encode(pngStream, SKEncodedImageFormat.Png, 100);
                ImageModel imageModel = new ImageModel();
                imageModel.ImageBytes = pngStream.ToArray();
                pngStream.Position = 0;
                imageModel.ImageSource = ImageSource.FromStream(()=>pngStream);
                //UpLoadImagesList.Add(new ImageModel { ImageBytes = pngStream.ToArray(), ImageSource = ImageSource.FromStream(() => new MemoryStream(pngStream.ToArray())) }); ;
                UpLoadImagesList.Add(imageModel) ;
            }
            catch (Exception ex) 
            {
                await Toast.Make(ex.Message).Show(CancellationToken.None);
                Debug.WriteLine(ex.Message);
                return;         
            }
            App.Current.Dispatcher.Dispatch(async () => await Toast.Make("选取图片").Show(CancellationToken.None));

        }
    }

    int sizeCount = 0;
    [RelayCommand]
    async Task VaryImagesAsync()
    {
        if (SignalR.DALLE2Connection.State is not HubConnectionState.Connected)
        {
            ShowDisplayAlert("提示", "没有网络,请检查网络😄", "好的!");
            return;
        }
        if (SignalR.IsDalleBusy)
        {
            ShowDisplayAlert("提示", "已经有个请求,请结束后再发起新的请求哦😄", "好的!");
            return;
        }
        if (UpLoadImagesList.Count == 0)
        {
            App.Current.Dispatcher.Dispatch(async ()=>await Toast.Make("请先上传图片哦").Show(CancellationToken.None));
            return;
        }
        if (string.IsNullOrWhiteSpace(GeneretionImageSize) && sizeCount <= 2)
        {
            await ShowToast("亲，请选择图像尺寸哦😄");
            //App.Current.Dispatcher.Dispatch(() => Shell.Current.DisplayAlert("提示", "亲，请选择图像尺寸哦😄", "好哒!"));
            sizeCount++;
            return;
        }
        else if (string.IsNullOrWhiteSpace(GeneretionImageSize) && sizeCount is > 2 and <= 4)
        {
            await ShowToast("亲，请选择图像尺寸😡");

            //App.Current.Dispatcher.Dispatch(() => Shell.Current.DisplayAlert("提示", "亲，请选择图像尺寸😡", "好的呢!"));
            sizeCount++;
            return;
        }
        else if (string.IsNullOrWhiteSpace(GeneretionImageSize) && sizeCount > 4)
        {
            //await ShowToast("你最好是🙂");

            App.Current.Dispatcher.Dispatch(() => Shell.Current.DisplayAlert("提示", "你最好是🙂", "是!"));
            sizeCount++;
            return;
        }

        try
        {
            SignalR.IsDalleBusy = true;             
            IsActivityIndicatorRunning = true;
            await   SignalR.DALLE2Connection.SendAsync(SignalR.SignalRMethod.ServerMethod.VaryImages, SignalR.DALLE2Connection.ConnectionId, UpLoadImagesList[0].ImageBytes,GeneretionImageQuantities, GeneretionImageSize, null);
            App.Current.Dispatcher.Dispatch(() => Shell.Current.DisplayAlert("提示", "图像正在加载，请不要关闭APP哦😄", "一定不关!"));
        }
        catch (Exception ex)
        {
            App.Current.Dispatcher.Dispatch(() => Shell.Current.DisplayAlert("提示", "糟糕,网络异常啦😵‍💫", "确定"));
        }

    }

    [RelayCommand]
    async Task SavePicturesAsync()
    {
        if (ImagesList.Count is 0)
        {
            App.Current.Dispatcher.Dispatch(async () => await ShowToast("没有图片可下载哦"));
            return;
        }
        List<Task> tasks = new List<Task>();
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            await ShowToast("没有权限");
            return;
        }
        foreach (var image in ImagesList)
        {
            tasks.Add(
                Task.Run(async () =>
                {
                    string picturesPath = string.Empty;
#if ANDROID
                    picturesPath = MainActivity.PicturesPath;
#endif
                    CancellationToken cancellationToken = new CancellationToken();
                    try
                    {
                        string imageFilePath = Path.Combine(picturesPath, image.Name);
                        Stream imageStream = new MemoryStream(image.ImageBytes);
                        FileStream imageFileStream = new FileStream(imageFilePath, FileMode.Create, FileAccess.Write);
                        await imageStream.CopyToAsync(imageFileStream);
                        App.Current.Dispatcher.Dispatch(async () => await Toast.Make($"File is saved").Show(cancellationToken));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    finally
                    {


                    }
                    //await semaphoreSlim.WaitAsync();
                    //MemoryStream stream = new MemoryStream(image.ImageBytes);
                    //CancellationToken cancellationToken = new CancellationToken();
                    //var fileLocation = await FileSaver.Default.SaveAsync(image.Name, stream, cancellationToken);
                })
            );
        }
        await Task.WhenAll(tasks);
        await ShowToast("下载完毕");
    }

    [RelayCommand]
    async Task ClearPicturesAsync()
    {
        ImagesList.Clear();
        App.Current.Dispatcher.Dispatch(async () => await ShowToast("已清空图片缓存"));
    }

    void RegisterSignalR()
    {
        SignalR.DALLE2Connection.On<List<string>, string>(SignalR.SignalRMethod.ClientMethod.VaryImages, async (imageUrlsList, message) =>
        {
            List<Task> tasks = new List<Task>();
            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
            imageUrlsList.ForEach(async (url) =>
            {
                Debug.WriteLine(url);
                tasks.Add(Task.Run(async () =>
                {
                    HttpClient httpClient = new HttpClient();
                    Guid guid = Guid.NewGuid();
                    ImageModel image = new ImageModel { Name = $"{guid}-Image_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.png", Url = url };
                    image.ImageBytes = await httpClient.GetByteArrayAsync(image.Url);
                    try
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.ImageSource = ImageSource.FromStream(() => new MemoryStream(image.ImageBytes));
                        }
                        await semaphoreSlim.WaitAsync();
                        ImagesList.Add(image);
                    }
                    catch (Exception ex)
                    {
                        CancellationToken cancellationToken = new CancellationToken();
                        await Toast.Make($"Error,{ex.Message}").Show(cancellationToken);

                    }
                    finally
                    {
                        semaphoreSlim?.Release();
                    }
                }));
            });
            await Task.WhenAll(tasks); 
            IsActivityIndicatorRunning = false;
            SignalR.IsDalleBusy = false;
            App.Current.Dispatcher.Dispatch(async () => await ShowToast("加载完毕"));

        });

    }
}


