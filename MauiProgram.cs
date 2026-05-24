using CommunityToolkit.Maui;
using EdenProject.ViewModels;
using EdenProject.Views; // Ensure this is here
using Microsoft.Extensions.Logging;

namespace EdenProject
{
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
                    // Important: If you have a font file for your icons, add it here:
                    // fonts.AddFont("MaterialSymbols.ttf", "MaterialSymbolsFont");
                });

            // 1. Register AppShell
            builder.Services.AddSingleton<AppShell>();

            // 2. Register ViewModels
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<SignInPageViewModel>();
            builder.Services.AddSingleton<SignUpViewModel>();

            // 3. Register Pages (Required for Dependency Injection)
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<SignInPage>();
            builder.Services.AddSingleton<SignUpPage>();

            builder.Services.AddSingleton<AdminPageViewModel>();
            builder.Services.AddSingleton<AdminPage>();

            builder.Services.AddTransient<UsersListPageViewModel>();
            builder.Services.AddTransient<UsersListPage>();

         
            builder.Services.AddTransient<AccountPageViewModel>();
            builder.Services.AddTransient<AccountPage>();


            builder.Services.AddTransient<AddChildViewModel>();
            builder.Services.AddTransient<AddChildPage>();
            builder.Services.AddTransient <DollHouseViewModel>();
            builder.Services.AddTransient<DollHousePage>();

            builder.Services.AddTransient<SignUpViewModel>();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            // רישום השירות של פיירבייס
            builder.Services.AddSingleton<Services.FirebaseService>();
            return builder.Build();
        }
    }
}