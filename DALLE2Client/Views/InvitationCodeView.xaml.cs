namespace DALLE2Client.Views;

public partial class InvitationCodeView : ContentPage
{
    InvitationCodeViewModel invitationCodeViewModel;
    public InvitationCodeView(InvitationCodeViewModel invitationCodeViewModel)
	{
		InitializeComponent();
		BindingContext= invitationCodeViewModel;								
        this.invitationCodeViewModel= invitationCodeViewModel;


    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        invitationCodeViewModel.ResetAccountInfo();
    }
}