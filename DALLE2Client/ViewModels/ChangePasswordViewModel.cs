
namespace DALLE2Client.ViewModels;

public partial class ChangePasswordViewModel:BaseViewModel
{

    public ChangePasswordViewModel()
    {
        RegisterSignalR();
    }

    [ObservableProperty] private string oldPassword;
    [ObservableProperty] private string newPassword;
    [ObservableProperty] private string confirmNewPassword;
    [ObservableProperty] private bool isChangingPassword;

    [RelayCommand]
    async Task ChangePasswordAsync()
    {
        if (SignalR.DALLE2Connection.State is not HubConnectionState.Connected)
        {
            ShowDisplayAlert("提示", "没有网络,请检查网络😄", "好的!");
            return;
        }
        if (string.IsNullOrEmpty(OldPassword))
        {
            await ShowToast("请输入旧密码哦");
            return;
        }
        if (string.IsNullOrEmpty(NewPassword))
        {
            await ShowToast("请输入新密码哦");
            return;
        }
        if (NewPassword != ConfirmNewPassword)
        {
            await ShowToast("两次密码不同步");
            return;
        }
        OldPassword=App.SHA512Encrypt(OldPassword);                     
        NewPassword=App.SHA512Encrypt(NewPassword);                             
        await SignalR.DALLE2Connection.SendAsync(SignalR.SignalRMethod.ServerMethod.ChangePassword, App.GetAccountValue(), OldPassword, NewPassword);
        NewPassword = string.Empty;
        OldPassword = string.Empty;
        ConfirmNewPassword = string.Empty;
    }

    void RegisterSignalR()
    {
        SignalR.DALLE2Connection.On<bool,string ,int, int, string, string> (SignalR.SignalRMethod.ClientMethod.ChangePassword, (isChangedPassword, password ,lastDrawCounts, lastChatCounts, exclusiveInvitationCode, boundInvitationCode) =>
        {
            if (isChangedPassword)
            {
                try
                {
                    App.SetPasswordValue(password);
                    App.SetLastDrawCountsValue(lastDrawCounts);
                    App.SetLastChatCountsValue(lastChatCounts);
                    App.SetExclusiveInvitationCodeValue(exclusiveInvitationCode);
                    App.SetBoundInvitationCodeValue(boundInvitationCode); ShowDisplayAlert("提示", "修改成功", "确定");
                    Shell.Current.SendBackButtonPressed();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            else
            {
                ShowDisplayAlert("提示", "修改失败,请稍后再试", "确定");
            }
        });
    }




}
