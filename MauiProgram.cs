using Microsoft.Extensions.Logging;
using CareReminderApp.Services;
using CareReminderApp.ViewModels;
using CareReminderApp.Views;
using CommunityToolkit.Maui;

namespace CareReminderApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit() // שורה קריטית לפתרון השגיאה!
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // רישום השירותים
            builder.Services.AddSingleton<IDataService, FirebaseDataService>();
            builder.Services.AddSingleton<AuthService>();

            // רישום ה-ViewModels (Transient יוצר מופע חדש בכל כניסה לדף)
            builder.Services.AddTransient<SignInPageViewModel>();
            builder.Services.AddTransient<SignUpPageViewModel>();
            builder.Services.AddTransient<ElderRemindersViewModel>();
            builder.Services.AddTransient<EldersListViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<TodayRemindersViewModel>();
            builder.Services.AddTransient<FamilyDashboardViewModel>();
            builder.Services.AddTransient<ReminderDetailsViewModel>();
            builder.Services.AddTransient<AddReminderViewModel>();
            builder.Services.AddTransient<ElderProfileViewModel>();

            // רישום הדפים
            builder.Services.AddTransient<SignInPage>();
            builder.Services.AddTransient<SignUpPage>();
            builder.Services.AddTransient<ElderRemindersPage>();
            builder.Services.AddTransient<EldersListPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<TodayRemindersPage>();
            builder.Services.AddTransient<FamilyDashboardPage>();
            builder.Services.AddTransient<ReminderDetailsPage>();
            builder.Services.AddTransient<ChangeProfilePage>();
            builder.Services.AddTransient<AddReminderPage>();
            builder.Services.AddTransient<ElderProfilePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}