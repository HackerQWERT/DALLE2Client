namespace DALLE2Client.Models;

public partial class GalleryPieceModel:ObservableObject
{
              
    [ObservableProperty]
    public ObservableCollection<ImageModel>imagesList=new ();

    public DateTime DateTime { get; set; }
}



