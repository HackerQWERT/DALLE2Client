
namespace DALLE2Client.Views;

public partial class AIGenerateView : ContentPage
{
    AIGenereteViewModel aIGenereteViewModel;

    public AIGenerateView(AIGenereteViewModel aIGenereteViewModel)
	{   
        InitializeComponent();
        BindingContext= aIGenereteViewModel;
        this.aIGenereteViewModel = aIGenereteViewModel;
    }

    void ImageQuantitySliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        Slider slider = sender as Slider;
        slider.Value = (int)e.NewValue;
    }


    void ImageSizeRadioButtonCheckedChangedAsync(object sender, CheckedChangedEventArgs e)
    {
        RadioButton radioButton=sender as RadioButton;
        if (radioButton.IsChecked == false) return;
        if (radioButton.Content.ToString() == "1024x1024")
            aIGenereteViewModel.GeneretionImageSize = "1024x1024";
        else if (radioButton.Content.ToString() == "512x512")
            aIGenereteViewModel.GeneretionImageSize = "512x512";
        else if (radioButton.Content.ToString() == "256x256")
            aIGenereteViewModel.GeneretionImageSize = "256x256";

    }


}