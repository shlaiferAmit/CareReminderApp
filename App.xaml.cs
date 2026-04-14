using CareReminderApp.Models; // חובה להוסיף כדי שהמחשב יכיר את המילה User
using CareReminderApp.Services;
using CareReminderApp.Views;

namespace CareReminderApp
{
    public partial class App : Application
    {
        // 🔥 משתמש מחובר גלובלי
        public static User LoggedInUser { get; set; }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}