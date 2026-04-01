using Microsoft.Extensions.Logging;
using CareReminderApp.Services;
using CareReminderApp.ViewModels;
using CareReminderApp.Views;



namespace CareReminderApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // 1. רישום השירותים (Services)
            builder.Services.AddSingleton<IDataService, MockDataService>();

            // 2. רישום ה-ViewModels
            builder.Services.AddTransient<SignInPageViewModel>();
            builder.Services.AddTransient<SignUpPageViewModel>(); // שורת התיקון עבור SignUp!
            builder.Services.AddTransient<ElderRemindersViewModel>();
            builder.Services.AddTransient<EldersListViewModel>();

            // 3. רישום הדפים (Views)
            builder.Services.AddTransient<SignInPage>();
            builder.Services.AddTransient<SignUpPage>(); // שורת התיקון עבור SignUp!
            builder.Services.AddTransient<ElderRemindersPage>();
            builder.Services.AddTransient<EldersListPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}