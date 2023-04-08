namespace DALLE2Client.ViewModels;


public partial class GalleryViewModel:BaseViewModel
{
    public GalleryViewModel() : base()
    {
    }

    [ObservableProperty]
    private ObservableCollection<GalleryPieceModel> galleryPiecesModelsList= new();


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
            await InitializeGalleryAsync();
        }
        catch (Exception? ex)
        {
            Debug.WriteLine(ex.Message) ;
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    async Task InitializeGalleryAsync()
    { 
        var imageModelsList = await GetImagesFromSqliiteAsync();
        List<DateTime> dateTimes = new List<DateTime>();    
        foreach(var v in imageModelsList) 
        {
            dateTimes.Add(v.DateTime);
        }
        dateTimes= dateTimes.Distinct().ToList();
        //var dateTimes = imageModelsList.Select(x => x.DateTime.Date).Distinct().ToList();

        for(int i=0;i<dateTimes.Count;i++) 
        {
            GalleryPieceModel galleryPieceModel = new GalleryPieceModel();
            
            galleryPieceModel.DateTime =dateTimes[i];
            foreach (var imageModel in imageModelsList)
            {
                if(imageModel.DateTime.Date == galleryPieceModel.DateTime.Date)
                {

                    ImageSource imageSource = ImageSource.FromStream(()=>new MemoryStream(imageModel.ImageBytes));
                    imageModel.ImageSource = imageSource;
                    galleryPieceModel.ImagesList.Add(imageModel);
                }
            }
            GalleryPiecesModelsList.Add(galleryPieceModel);
        }         
    }
}
