using CareReminderApp.Models; // חובה להוסיף כדי שהמחשב יכיר את המילה User
using CareReminderApp.Services;
using CareReminderApp.Views;

namespace CareReminderApp
{
    public partial class App : Application
    {
        // השאירי רק את זה
        public static Models.User LoggedInUser { get; set; }

        public App(IDataService dataService)
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}