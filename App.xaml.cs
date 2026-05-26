// מחלקת האפליקציה הראשית
// אחראית על אתחול האפליקציה וניהול המשתמש המחובר באופן גלובלי

using CareReminderApp.Models;
using CareReminderApp.Services;
using CareReminderApp.Views;

namespace CareReminderApp
{
    public partial class App : Application
    {
        // משתמש מחובר שמוגדר לכל האפליקציה
        public static User? LoggedInUser { get; set; }

        public App(IDataService dataService, AuthService authService)
        {
            InitializeComponent();

            // קביעת דף ראשי של האפליקציה
            MainPage = new AppShell();

            // בדיקה האם יש משתמש שמור בכניסה אוטומטית
            CheckRememberedUser(dataService, authService);
        }

        // בדיקה האם המשתמש בחר "זכור אותי" והתחברות אוטומטית
        private async void CheckRememberedUser(IDataService dataService, AuthService authService)
        {
            try
            {
                // בדיקה האם יש שמירת התחברות
                bool isRemembered = Preferences.Default.Get("IsRemembered", false);

                if (!isRemembered)
                    return;

                // שליפת פרטי התחברות שמורים
                string email = Preferences.Default.Get("UserEmail", string.Empty);
                string password = Preferences.Default.Get("UserPassword", string.Empty);

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    return;

                // ניסיון למצוא את המשתמש במסד הנתונים
                var user = await dataService.GetUserAsync(email, password);

                if (user == null)
                    return;

                // שמירת המשתמש כמשתמש מחובר גלובלי
                App.LoggedInUser = user;

                // עדכון ממשק האפליקציה למצב מחובר
                if (Shell.Current is AppShell appShell)
                {
                    appShell.SetLoggedInState(true, user);
                }
            }
            catch (Exception ex)
            {
                // הצגת שגיאה במקרה של כשל בתהליך
                await Shell.Current.DisplayAlert("שגיאה", ex.Message, "אישור");
            }
        }
    }
}