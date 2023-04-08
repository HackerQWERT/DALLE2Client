
namespace DALLE2Client.ViewModels;

public partial class SettingViewModel:BaseViewModel
{

    public SettingViewModel()
    {

        RegisterSignalR();
        if(SignalR.DALLE2Connection.State is HubConnectionState.Connected)
            SignalR.DALLE2Connection.SendAsync(SignalR.SignalRMethod.ServerMethod.SettingSyncAccountInformation, App.GetAccountValue(), App.GetPasswordValue());

    }


    [ObservableProperty]private string account;
    [ObservableProperty]private int lastDrawCounts;
    [ObservableProperty]private int lastChatCounts;
    [ObservableProperty] private bool isLogged;


    [RelayCommand]
    async Task GoToChangePasswordViewAsync()
    {
        await Shell.Current.GoToAsync(nameof(ChangePasswordView));
    }


    [RelayCommand]
    async Task GoToLoginViewAsync()
    {
        await Shell.Current.GoToAsync(nameof(LoginView));
    }

    [RelayCommand]
    async Task GoToRegisterAccountViewAsync()
    {
        await Shell.Current.GoToAsync(nameof(RegisterAccountView));
    }

    [RelayCommand]
    async Task SignOutAsync()
    {
        App.SetAccountValue("-1");
        App.SetPasswordValue("-1");
        App.SetLastChatCountsValue(-1);                   
        App.SetLastDrawCountsValue(-1);
        App.SetExclusiveInvitationCodeValue(null);
        App.SetBoundInvitationCodeValue(null);
        ResetAccountInfo();
    }
  
    public   void ResetAccountInfo()
    {
        if (App.GetAccountValue() is not "-1")
        {
            IsLogged = true;
            Account = App.GetAccountValue();
            LastDrawCounts = App.GetLastDrawCountsValue();
            LastChatCounts = App.GetLastChatCountsValue();
        }
        else
        {
            IsLogged = false;
            Account = string.Empty;
            LastDrawCounts = 0;
            LastChatCounts = 0;
        }
    }

    [RelayCommand]
    async Task RefreshAsync()
    {
        IsRefreshing = true;
        try
        {
            DateTime startTime = DateTime.Now;
            if (SignalR.DALLE2Connection.State is not HubConnectionState.Connected)
            {
                await SignalR.DALLE2Connection.StopAsync();

                SignalR.DALLE2Connection.StartAsync();

                while (true)
                {
                    await Task.Delay(1000);
                    if (SignalR.DALLE2Connection.State is HubConnectionState.Connected)
                    {
                        WebConnectionIconAddress = "连接";
                        break;

                    }
                    else if (DateTime.Now - startTime > TimeSpan.FromSeconds(5))
                    {
                        await SignalR.DALLE2Connection.StopAsync();
                        break;
                    }
                }

            }
            else
            {
                await SignalR.DALLE2Connection.SendAsync(SignalR.SignalRMethod.ServerMethod.SettingSyncAccountInformation, App.GetAccountValue(), App.GetPasswordValue());
            }
        }
        catch (Exception? ex)
        {
        }
        finally
        {
            IsRefreshing = false;
        }
    }
    void RegisterSignalR()
    {
        SignalR.DALLE2Connection.On<bool,string, string, int, int, string, string>(SignalR.SignalRMethod.ClientMethod.SettingSyncAccountInformation,
            async (isSucceed,account, password, lastDrawCounts, lastChatCounts, exclusiveInvitationCode, boundInvitationCode) =>
            {
                if(isSucceed) 
                {
                    try
                    {
                        App.SetAccountValue(account);
                        App.SetPasswordValue(password);
                        App.SetLastDrawCountsValue(lastDrawCounts);
                        App.SetLastChatCountsValue(lastChatCounts);
                        App.SetExclusiveInvitationCodeValue(exclusiveInvitationCode);
                        App.SetBoundInvitationCodeValue(boundInvitationCode);
                        Account = account;
                        LastChatCounts = lastChatCounts;
                        LastDrawCounts = lastDrawCounts;
                        await ShowToast("刷新成功");
                    }
                    catch (Exception ex)
                    {
                        ShowDisplayAlert("提示", "账号或密码错误,请重新登录", "确定");
                        IsLogged = false;
                        Debug.WriteLine(ex);
                    }
                }
                else
                {
                    ShowDisplayAlert("提示", "账号或密码错误,请重新登录", "确定");
                    IsLogged = false;
                }
           
            }); 
    }

}

