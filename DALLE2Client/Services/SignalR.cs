namespace DALLE2Client.Services;

public class  SignalR
{
    public static HubConnection DALLE2Connection { get; set; }
    public static HubConnection AccountConnection { get; set; }
    public static bool IsDalleBusy { get; set; } = false;

    public static string WebsocketUrl { get; set; } = DeviceInfo.Platform == DevicePlatform.Android ? "http://43.155.129.173" : "http://43.155.129.173";
    public static string Port { get; set; } = "5034";
    public static string Dalle2HubEndPoint { get; set; } = "DALLE2Hub";




    public static class SignalRMethod
    {

        public static class ServerMethod
        {
            public static string GenerateImages { get; set; } = "DALLE2ImagesGeneration";
            public static string VaryImages { get; set; } = "DALLE2ImagesVary";
            public static string EditImages { get; set; } = "DALLE2ImagesEdit";
            public static string GenerateInvitationCode { get; set; } = "GenerateInvitationCode";



            public static string BindInvitationCode { get; set; } = "BindInvitationCode";



            public static string RegisterAccount { get; set; } = "RegisterAccount";
            public static string ChangePassword { get; set; } = "ChangePassword";
            public static string Login { get; set; } = "Login";
            public static string InvitationSyncAccountInformation { get; set; } = "InvitationSyncAccountInformation";
            public static string SettingSyncAccountInformation { get; set; } = "SettingSyncAccountInformation";
            public static string PurchaseDrawOrChat { get; set; } = "PurchaseDrawOrChat";
        }
        public static class ClientMethod
        {
            public static string GenerateImages { get; set; } = "GenerateImages";
            public static string VaryImages { get; set; } = "VaryImages";
            public static string EditImages { get; set; } = "EditImages";
            public static string ChangePassword { get; set; } = "ChangePassword";
            public static string Login { get; set; } = "Login";
            public static string RegisterAccount { get; set; } = "RegisterAccount";
            public static string SettingSyncAccountInformation { get; set; } = "SettingSyncAccountInformation";
            public static string InvitationSyncAccountInformation { get; set; } = "InvitationSyncAccountInformation";
            public static string GenerateInvitationCode { get; set; } = "GenerateInvitationCode";

            public static string BindInvitationCode { get; set; } = "BindInvitationCode";

        }


    }



}
