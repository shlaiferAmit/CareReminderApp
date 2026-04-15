using CareReminderApp.Models; // חובה להוסיף כדי שהמחשב יכיר את המילה User
using CareReminderApp.Services;
using CareReminderApp.Views;

namespace CareReminderApp
{
    public partial class App : Application
    {
        // הוספת ה-? מאפשרת למשתמש להיות ריק בזמן הלוגאאוט
        public static User? LoggedInUser { get; set; }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}