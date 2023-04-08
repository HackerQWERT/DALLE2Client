
namespace DALLE2Client.ViewModels;

public partial class TestViewModel : BaseViewModel
{


    [ObservableProperty]
    private ObservableCollection<ImageModel> imageUrlsList;

    public TestViewModel()
    {
        ImageUrlsList = new ObservableCollection<ImageModel>
        {
            new ImageModel { Name = "Image", Url = "dotnet_bot.svg" } ,
            new ImageModel { Name = "Image", Url = "dotnet_bot.svg" } ,
            new ImageModel { Name = "Image", Url = "dotnet_bot.svg" } ,
            new ImageModel { Name = "Image", Url = "dotnet_bot.svg" } 
        }; 
        Image image= new Image();
    }

}

