global using Microsoft.Extensions.Logging;
global using Microsoft.AspNetCore.SignalR.Client;
global using Microsoft.Maui.Storage;

global using CommunityToolkit.Maui;
global using CommunityToolkit.Maui.Alerts;
global using CommunityToolkit.Maui.Core;
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;

global using System.Diagnostics;
global using System.Collections.ObjectModel;
global using System.Net.WebSockets;
global using System.Security.Cryptography;
global using System.Text;


global using DALLE2Client.ViewModels;
global using DALLE2Client.Views;
global using DALLE2Client.Models;
global using DALLE2Client.Services;

global using SkiaSharp;
global using SQLite;

using DALLE2Client;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
        builder.Logging.AddDebug();
#endif

        //注册服务
		builder.Services.AddSingleton<SignalR>();		
		builder.Services.AddSingleton<SqliteDatabase>();		




		//注册Views
		builder.Services.AddSingleton<AIGenerateView>();
		builder.Services.AddSingleton<AIVaryView>();
		builder.Services.AddSingleton<SettingView>();
		builder.Services.AddSingleton<GalleryView>();
		builder.Services.AddSingleton<InvitationCodeView>();
		builder.Services.AddSingleton<TestView>();

        builder.Services.AddTransient<ChangePasswordView>();
        builder.Services.AddTransient<LoginView>();
        builder.Services.AddTransient<RegisterAccountView>();

        //注册ViewModels
        builder.Services.AddSingleton<AIGenereteViewModel>();
        builder.Services.AddSingleton<AIVaryViewModel>();
        builder.Services.AddSingleton<SettingViewModel>();
        builder.Services.AddSingleton<GalleryViewModel>();
        builder.Services.AddSingleton<InvitationCodeViewModel>();
        builder.Services.AddSingleton<TestViewModel>();
        builder.Services.AddSingleton<BaseViewModel>();

        builder.Services.AddTransient<ChangePasswordViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterAccountViewModel>();



        return builder.Build();
	}
}
