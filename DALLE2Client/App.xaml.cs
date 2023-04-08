 
namespace DALLE2Client;

public partial class App : Application
{
    public App()
	{
        InitializeComponent();
		MainPage = new AppShell();

        InitializeSqliteDatabase();

        if (SignalR.DALLE2Connection is null)
        {
            _ = InitializeConnection();
            Task.Run(() => A());
            Task.Run(() => B());
        }
    }
    public static  SemaphoreSlim  signalrSemaphoreSlim = new SemaphoreSlim(1);

    public async Task RegisterAccountAsync()
    {
        if(SignalR.DALLE2Connection.State is not  HubConnectionState.Connected) 
        {
            return;
        }        
        else if(GetAccountValue() is "-1") 
        { 
            if(CheckFirstLogin()) 
            {
                Guid accountGuid = Guid.NewGuid();
                Guid passwordGuid = Guid.NewGuid();
                await SignalR.DALLE2Connection.SendAsync(SignalR.SignalRMethod.ServerMethod.RegisterAccount, accountGuid, passwordGuid);
                SetAccountValue(accountGuid.ToString());
            }
        }       
    }
   
    public static  string SHA512Encrypt(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        using (var sha3 = SHA512.Create())
        {
            byte[] hashBytes = sha3.ComputeHash(bytes);
            string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            
            Debug.WriteLine("Input: " + input);
            Debug.WriteLine("SHA3 hash: " + hash);
            return hash;
        }
    }

    public static SqliteDatabase SqliteDatabase { get; set; }

    public void InitializeSqliteDatabase()
    {
        SqliteDatabase = new SqliteDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ImageModel.db3"));
    }

    async Task B()
    {
        while (true)
        {
            await Task.Delay(1000);
            Debug.WriteLine(SignalR.DALLE2Connection.State + "\t" + DateTime.Now);
        }
    }
    async Task A()
    {
        var lastState = SignalR.DALLE2Connection.State;
        var lastTime = DateTime.Now;
        int count = 1;
        while (true)
        {
            await Task.Delay(1000);
            if (SignalR.DALLE2Connection.State == HubConnectionState.Reconnecting)
            {
                Debug.WriteLine("正在重连" + "\t" + DateTime.Now);
                try
                {
                    await SignalR.DALLE2Connection.StartAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            if (count == 1)
            {
                Debug.WriteLine(SignalR.DALLE2Connection.State + "\t" + DateTime.Now);
                count = -1;
            }
            else if (SignalR.DALLE2Connection.State != lastState)
            {
                lastState = SignalR.DALLE2Connection.State;
                Debug.WriteLine(SignalR.DALLE2Connection.State + "\t" + (DateTime.Now - lastTime));
                lastTime = DateTime.Now;
            }

        }


    }

    async Task InitializeConnection()
    {
        string url = $"{SignalR.WebsocketUrl}:{SignalR.Port}/{SignalR.Dalle2HubEndPoint}";
        SignalR.DALLE2Connection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();
        try
        {
            DateTime startTime = DateTime.Now;
            if (SignalR.DALLE2Connection.State is HubConnectionState.Disconnected)
            {
                await signalrSemaphoreSlim.WaitAsync();
                await SignalR.DALLE2Connection.StartAsync();
                while (true)
                {
                    await Task.Delay(1000);
                    if (DateTime.Now - startTime > TimeSpan.FromSeconds(5) && SignalR.DALLE2Connection.State is not HubConnectionState.Connected)
                    {
                        await SignalR.DALLE2Connection.StopAsync();
                        break;
                    }
                    else if (SignalR.DALLE2Connection.State is HubConnectionState.Connected)
                    {
                        break;
                    }
                }

            }
        }
        catch (Exception? ex)
        {
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            signalrSemaphoreSlim.Release();
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

	protected override void OnSleep()
	{
		base.OnSleep();				
	}

    protected override void OnResume()
    {
        base.OnResume();
    }


    public static bool CheckFirstLogin()
    {
        bool isFirstLogin = false;

        if(!Preferences.Default.Get("IsFirstLogin",false))
        {
            Preferences.Default.Set("IsFirstLogin", true);
        }
        else
        {
            isFirstLogin = true;
        }
        return isFirstLogin;

    }

    public static void SetAccountValue(string account)
    {
        Preferences.Default.Set("Account", account);
    }
    public static string GetAccountValue()
    {
        return Preferences.Default.Get("Account", "-1");
    }

    public static void SetPasswordValue(string password)
    {
        Preferences.Default.Set("Password",password);
    }
    public static string GetPasswordValue() 
    {
        return Preferences.Default.Get("Password","-1");
    }

    public static void SetLastDrawCountsValue(int lastDrawCounts)
    {
        Preferences.Default.Set("LastDrawCounts", lastDrawCounts);
    }
    public static int GetLastDrawCountsValue()
    {
        return Preferences.Default.Get("LastDrawCounts", -1);
    }

    public static void SetLastChatCountsValue(int lastChatCounts)
    {
        Preferences.Default.Set("LastChatCounts", lastChatCounts);
    }
    public static int GetLastChatCountsValue()
    {
        return Preferences.Default.Get("LastChatCounts", -1);
    }

    public static void SetExclusiveInvitationCodeValue(string exclusiveInvitationCode)
    {
        Preferences.Default.Set("ExclusiveInvitationCode", exclusiveInvitationCode);
    }
    public static string GetExclusiveInvitationCodeValue()
    {
        return Preferences.Default.Get("ExclusiveInvitationCode", "-1");
    }
    public static void SetBoundInvitationCodeValue(string boundInvitationCode)
    {
        Preferences.Default.Set("BoundInvitationCode", boundInvitationCode);
    }
    public static string GetBoundInvitationCodeValue()
    {
        return Preferences.Default.Get("BoundInvitationCode", "-1");
    }

}
