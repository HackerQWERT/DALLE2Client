namespace DALLE2Client.Views;

public partial class ChangePasswordView : ContentPage
{
	public ChangePasswordView(ChangePasswordViewModel changePasswordViewModel)
	{
		InitializeComponent();
		BindingContext= changePasswordViewModel;					
	}
}