namespace DALLE2Client.ViewModels;

public partial class LoginViewModel : BaseViewModel
{

    public LoginViewModel()
    {
        RegisterSignalR();
    }

    [ObservableProperty] private  string account;
    [ObservableProperty] private string password;
    
    [RelayCommand]
    async Task   LoginAsync()
    {
        if (SignalR.DALLE2Connection.State is not HubConnectionState.Connected)
        {
            ShowDisplayAlert("提示", "没有网络,请稍后再试", "确定");
            return;
        }
        if (string.IsNullOrEmpty(Account))
        {
            await ShowToast("请输入账号哦");
            return;
        }
        if (string.IsNullOrEmpty(Password))
        {
            await ShowToast("请输入密码哦");
            return;
        }
        try
        {
            Password=App.SHA512Encrypt(Password);                                                       
            await SignalR.DALLE2Connection.SendAsync(SignalR.SignalRMethod.ServerMethod.Login, Account, Password);
            Account = string.Empty;
            Password = string.Empty;
        }
        catch (Exception ex) 
        {
            Debug.WriteLine(ex.Message);
        }


    }



    void RegisterSignalR()
    {
        SignalR.DALLE2Connection.On<bool,string,string,int,int,string ,string>(SignalR.SignalRMethod.ClientMethod.Login,
            (isLoginSuccess,account,password,lastDrawCounts,lastChatCounts,exclusiveInvitationCode,boundInvitationCode) => 
        {
            if(isLoginSuccess) 
            {
                try
                {
                    App.SetAccountValue(account);        
                    App.SetPasswordValue(password);        
                    App.SetLastDrawCountsValue(lastDrawCounts);        
                    App.SetLastChatCountsValue(lastChatCounts  );        
                    App.SetExclusiveInvitationCodeValue(exclusiveInvitationCode);
                    App.SetBoundInvitationCodeValue(boundInvitationCode);
                    ShowDisplayAlert("提示", "登录成功", "确定");
                    Shell.Current.SendBackButtonPressed();                          
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);                    
                }
            }   
            else
            {
                ShowDisplayAlert("提示", "账号或密码错误", "确定");
            }
        });

    }

}
