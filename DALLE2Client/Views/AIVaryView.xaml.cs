namespace DALLE2Client.Views;

public partial class AIVaryView : ContentPage
{
    AIVaryViewModel aIVaryViewModel;
	public AIVaryView(AIVaryViewModel aIVaryViewModel)
	{
		InitializeComponent();
		BindingContext = aIVaryViewModel;		
        this.aIVaryViewModel = aIVaryViewModel;
    }
    private void ImageQuantitySliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        Slider slider = sender as Slider;
        slider.Value = (int)e.NewValue;
    }
    
    void ImageSizeRadioButtonCheckedChangedAsync(object sender, CheckedChangedEventArgs e)
    {
        RadioButton radioButton = sender as RadioButton;
        if (radioButton.IsChecked == false) return;
        if (radioButton.Content.ToString() == "1024x1024")
            aIVaryViewModel.GeneretionImageSize = "1024x1024";
        else if (radioButton.Content.ToString() == "512x512")
            aIVaryViewModel.GeneretionImageSize = "512x512";
        else if (radioButton.Content.ToString() == "256x256")
            aIVaryViewModel.GeneretionImageSize = "256x256";

    }

}