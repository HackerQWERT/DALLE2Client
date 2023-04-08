namespace DALLE2Client.Views;
 
public partial class RegisterAccountView : ContentPage
{
	public RegisterAccountView(RegisterAccountViewModel registerAccountViewModel)
	{
		InitializeComponent();
		BindingContext= registerAccountViewModel;					
	}
}