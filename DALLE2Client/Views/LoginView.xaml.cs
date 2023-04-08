namespace DALLE2Client.Views;
 
public partial class LoginView : ContentPage
{
	public LoginView(LoginViewModel loginViewModel)
	{
		InitializeComponent();
		BindingContext= loginViewModel;					
	}

}