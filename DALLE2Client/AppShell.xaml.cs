
namespace DALLE2Client;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(AIGenerateView), typeof(AIGenerateView));
        Routing.RegisterRoute(nameof(AIVaryView), typeof(AIVaryView));
        Routing.RegisterRoute(nameof(SettingView), typeof(SettingView));
        Routing.RegisterRoute(nameof(GalleryView), typeof(GalleryView));
        Routing.RegisterRoute(nameof(InvitationCodeView), typeof(InvitationCodeView));
        Routing.RegisterRoute(nameof(ChangePasswordView), typeof(ChangePasswordView));
        Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
        Routing.RegisterRoute(nameof(RegisterAccountView), typeof(RegisterAccountView));
    }
}
