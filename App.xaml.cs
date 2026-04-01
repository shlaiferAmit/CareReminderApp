using CareReminderApp.Models; // חובה להוסיף כדי שהמחשב יכיר את המילה User
using CareReminderApp.Services;
using CareReminderApp.Views;

namespace CareReminderApp
{
    public partial class App : Application
    {
        public User CurrentUser { get; set; }

        public App(IDataService dataService)
        {
            InitializeComponent();
            // שימוש ב-AppShell כפי שמופיע במבנה התיקיות שלך
            MainPage = new AppShell();
        }
    }
}