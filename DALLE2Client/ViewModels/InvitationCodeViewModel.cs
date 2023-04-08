namespace DALLE2Client.ViewModels;

public partial class InvitationCodeViewModel : BaseViewModel
{
    public InvitationCodeViewModel()
    {
        
        RegisterSignalR();
        if(SignalR.DALLE2Connection.State is HubConnectionState.Connected) 
        {
            SignalR.DALLE2Connection.SendAsync(SignalR.SignalRMethod.ServerMethod.InvitationSyncAccountInformation,App.GetAccountValue(),App.GetPasswordValue());
        }
        ResetAccountInfo()
;
    }


    [ObservableProperty] private string exclusiveInvitationCode;
    [ObservableProperty] private string boundInvitationCode;
    [ObservableProperty] private string invitationCodeToBind;
    [ObservableProperty] private bool isHavingExclusiveInvitationCode;
    [ObservableProperty] private bool isHavingBoundInvitationCode;


    public void ResetAccountInfo()
    {
        if (App.GetExclusiveInvitationCodeValue() is not "-1" and not null)
        {
            IsHavingExclusiveInvitationCode = true;
            ExclusiveInvitationCode=App.GetExclusiveInvitationCodeValue();
        }
        else
            IsHavingExclusiveInvitationCode = false;

        if (App.GetBoundInvitationCodeValue() is not "-1" and not null)
        {
            IsHavingBoundInvitationCode = true;
            BoundInvitationCode = App.GetBoundInvitationCodeValue();                
        }
        else
            IsHavingBoundInvitationCode =false;

    }

    [RelayCommand]
    async Task GenerateInvitationCodeAsync()
    {
        if(SignalR.DALLE2Connection.State is not  HubConnectionState.Connected)
        {
            ShowDisplayAlert("提醒","没有网络,请稍后再试","OK");
            return;
        }
        await SignalR.DALLE2Connection.SendAsync(SignalR.SignalRMethod.ServerMethod.GenerateInvitationCode,App.GetAccountValue(),App.GetPasswordValue(),InvitationCodeToBind);
    }


    [RelayCommand]
    async Task CopyInvitationCodeAsync()
    {
        

    }

    [RelayCommand]
    async Task BindInvitationCodeAsync()
    {
        if (SignalR.DALLE2Connection.State is not HubConnectionState.Connected)
        {
            ShowDisplayAlert("提醒", "没有网络,请稍后再试", "OK");
            return;
        }
        await SignalR.DALLE2Connection.SendAsync(SignalR.SignalRMethod.ServerMethod.BindInvitationCode, App.GetAccountValue(), App.GetPasswordValue(), InvitationCodeToBind);
        InvitationCodeToBind = string.Empty;

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
                await SignalR.DALLE2Connection.SendAsync(SignalR.SignalRMethod.ServerMethod.InvitationSyncAccountInformation, App.GetAccountValue(), App.GetPasswordValue());
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
        SignalR.DALLE2Connection.On<bool, string, string, int, int, string, string>(SignalR.SignalRMethod.ClientMethod.InvitationSyncAccountInformation,
            async (isSucceed, account, password, lastDrawCounts, lastChatCounts, exclusiveInvitationCode, boundInvitationCode) =>
            {
                if (isSucceed)
                {
                    try
                    {
                        App.SetAccountValue(account);
                        App.SetPasswordValue(password);
                        App.SetLastDrawCountsValue(lastDrawCounts);
                        App.SetLastChatCountsValue(lastChatCounts);
                        App.SetExclusiveInvitationCodeValue(exclusiveInvitationCode);
                        App.SetBoundInvitationCodeValue(boundInvitationCode);
                        ResetAccountInfo()
;
                        await ShowToast("刷新成功");
                    }
                    catch (Exception ex)
                    {
                        ShowDisplayAlert("提示", "账号或密码错误,请重新登录", "确定");
                        Debug.WriteLine(ex);
                    }
                }
                else
                {
                    ShowDisplayAlert("提示", "账号或密码错误,请重新登录", "确定");
                }

            });

        SignalR.DALLE2Connection.On<bool, string, string,  string>(SignalR.SignalRMethod.ClientMethod.GenerateInvitationCode,
            async (isSucceed, account, password, exclusiveInvitationCode) =>
            {
                if (isSucceed)
                {
                    try
                    {
                        App.SetAccountValue(account);
                        App.SetPasswordValue(password);
                        App.SetExclusiveInvitationCodeValue(exclusiveInvitationCode);
                        ResetAccountInfo();         
                        await ShowToast("生成邀请码成功");
                    }
                    catch (Exception ex)
                    {
                        ShowDisplayAlert("提示", "生成邀请码失败,请重试", "确定");
                        Debug.WriteLine(ex);
                    }
                }
                else
                {
                    ShowDisplayAlert("提示", "生成邀请码失败,请重试", "确定");
                }

            });

        SignalR.DALLE2Connection.On<bool, string, string, string>(SignalR.SignalRMethod.ClientMethod.BindInvitationCode,
    async (isSucceed, account, password, boundInvitationCode) =>
    {
        if (isSucceed)
        {
            try
            {
                App.SetAccountValue(account);
                App.SetPasswordValue(password);
                App.SetBoundInvitationCodeValue(boundInvitationCode);
                ResetAccountInfo();
                await ShowToast("绑定邀请码成功");
            }
            catch (Exception ex)
            {
                ShowDisplayAlert("提示", "绑定邀请码失败,请重试", "确定");
                Debug.WriteLine(ex);
            }
        }
        else
        {
            ShowDisplayAlert("提示", "绑定邀请码失败,请重试", "确定");
        }

    });





    }

}
