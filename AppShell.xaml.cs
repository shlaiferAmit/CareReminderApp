using CareReminderApp.Views;

namespace CareReminderApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // רישום הנתיבים מאפשר לאפליקציה למצוא את הדפים כשאת לוחצת על כפתור
            Routing.RegisterRoute("SignUpPage", typeof(SignUpPage));
            Routing.RegisterRoute("SignInPage", typeof(SignInPage));
            Routing.RegisterRoute("FamilyDashboardPage", typeof(FamilyDashboardPage));
        }
    }
}