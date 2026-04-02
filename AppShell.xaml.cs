using CareReminderApp.Views;

namespace CareReminderApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // רישום נתיבים עבור דפי הרישום והכניסה
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
            Routing.RegisterRoute(nameof(SignInPage), typeof(SignInPage));

            // רישום נתיבים עבור ממשק המשפחה
            Routing.RegisterRoute(nameof(FamilyDashboardPage), typeof(FamilyDashboardPage));
            Routing.RegisterRoute(nameof(EldersListPage), typeof(EldersListPage));
            Routing.RegisterRoute(nameof(AddReminderPage), typeof(AddReminderPage));

            // רישום נתיבים עבור ממשק המבוגר
            Routing.RegisterRoute(nameof(ElderRemindersPage), typeof(ElderRemindersPage));

            // תיקון: הוספת הנתיבים החסרים שגרמו לשגיאות
            Routing.RegisterRoute(nameof(TodayRemindersPage), typeof(TodayRemindersPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));



        }
    }
}