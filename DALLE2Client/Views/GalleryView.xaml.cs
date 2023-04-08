namespace DALLE2Client.Views;

public partial class GalleryView : ContentPage
{

	GalleryViewModel galleryViewModel;
	public GalleryView(GalleryViewModel galleryViewModel)
	{
		InitializeComponent();
		BindingContext=galleryViewModel;
		this.galleryViewModel=galleryViewModel;										
	}
	




}