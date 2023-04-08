namespace DALLE2Client.Views;

public partial class SettingView : ContentPage
{

	SettingViewModel settingViewModel;
	public SettingView(SettingViewModel settingViewModel)
	{
		InitializeComponent();
		BindingContext= settingViewModel;
		this.settingViewModel = settingViewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		settingViewModel.ResetAccountInfo();
	}
}