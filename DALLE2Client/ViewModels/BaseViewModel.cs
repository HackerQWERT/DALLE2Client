
namespace DALLE2Client.ViewModels;

public partial class BaseViewModel:ObservableObject
{
    public BaseViewModel() 
    {
        if(SignalR.DALLE2Connection.State is HubConnectionState.Connected)
            WebConnectionIconAddress = "连接";
        else
            WebConnectionIconAddress = "未连接";
        RegisterSignalR();
        Task.Run(() => CheckConnnection());
    }


    [ObservableProperty]protected string webConnectionIconAddress;

    [ObservableProperty]protected ObservableCollection<ImageModel> imagesList = new();

    [ObservableProperty]protected bool isRefreshing;

    [ObservableProperty]protected string prompts;

    [ObservableProperty]protected int generetionImageQuantities;

    [ObservableProperty]protected string generetionImageSize;


    [ObservableProperty]protected bool isActivityIndicatorRunning;
 
    void  RegisterSignalR()
    {
        SignalR.DALLE2Connection.Reconnecting += async (ex) =>
        {
            WebConnectionIconAddress = "未连接";
        };

        SignalR.DALLE2Connection.Reconnected += async (message) =>
        {
            WebConnectionIconAddress = "连接";
        };
   
        SignalR.DALLE2Connection.On<string>("ReceiveMessage", (message) => App.Current.Dispatcher.DispatchAsync(() => Shell.Current.DisplayAlert("Message", $"{message}", "Ok")));

    }
    async Task CheckConnnection()
    {
        while (true)
        {
            await Task.Delay(100);
            try
            {
                await App.signalrSemaphoreSlim.WaitAsync();
                if (SignalR.DALLE2Connection.State is not HubConnectionState.Connected)
                {
                    await SignalR.DALLE2Connection.StopAsync();
                    DateTime startTime = DateTime.Now;
                    SignalR.DALLE2Connection.StartAsync();
                    while (true)
                    {
                        await Task.Delay(100);
                        if (DateTime.Now - startTime > TimeSpan.FromSeconds(5))
                        {
                            await SignalR.DALLE2Connection.StopAsync();
                            break;
                        }
                        else if (SignalR.DALLE2Connection.State == HubConnectionState.Connected)
                        {
                            WebConnectionIconAddress = "连接";
                            break;
                        }
                    }

                }
                else 
                    WebConnectionIconAddress = "连接";
            }
            catch
            {

            }
            finally
            {
                App.signalrSemaphoreSlim.Release();                                     
            }
        }
    }
    protected async Task ShowToast(string text)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        ToastDuration duration = ToastDuration.Short;
        double fontSize = 14;
        var toast = Toast.Make(text, duration, fontSize);

        Application.Current.Dispatcher.Dispatch(async () => await toast.Show(cancellationTokenSource.Token));

    }
    protected void ShowDisplayAlert(string alert, string content,string buttonText)
    {
        App.Current.Dispatcher.Dispatch(() => Shell.Current.DisplayAlert(alert, content, buttonText));
    }

    protected async Task SaveImageToSqliteAsync(ImageModel imageModel)
    {
        try
        {
            await App.SqliteDatabase.InsertImageModelAsync(imageModel);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);    
            await ShowToast(ex.Message);
        }
    }
    protected async Task SaveImagesToSqliteAsync(ObservableCollection<ImageModel> imageModelsList)
    {
        try
        {
            await App.SqliteDatabase.InsertImageModelsAsync(imageModelsList);
        }
        catch (Exception ex)
        {
            await ShowToast(ex.Message);
        }
    }
    protected async Task DeleteImageFromSqliiteAsync(ImageModel imageModel)
    {
        try
        {
            await App.SqliteDatabase.DeleteImageModelAsync(imageModel);
        }
        catch(Exception ex) 
        {
            await ShowToast(ex.Message);                      
        }
    }
    protected async Task DeleteImagesFromSqliiteAsync(ObservableCollection<ImageModel> imageModelsList)
    {
        try
        {
            await App.SqliteDatabase.DeleteImageModelsAsync(imageModelsList);
        }
        catch(Exception ex) 
        {
            await ShowToast(ex.Message);                      
        }
    }
    protected async Task<List<ImageModel>> GetImagesFromSqliiteAsync()
    {
        var imagesList = new List<ImageModel>();
        try
        {
            imagesList=await App.SqliteDatabase.GetImageModelsAsync();
        }
        catch (Exception ex)
        {
            await ShowToast(ex.Message);
        }
        return imagesList;

    }




}

