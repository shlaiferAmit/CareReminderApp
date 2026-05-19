using CareReminderApp.Models; 
using CareReminderApp.Services;
using CareReminderApp.Views;

namespace CareReminderApp
{
    public partial class App : Application
    {
        public static User? LoggedInUser { get; set; }

        public App(IDataService dataService, AuthService authService)
        {
            InitializeComponent();
            MainPage = new AppShell();

            CheckRememberedUser(dataService, authService);
        }

        private async void CheckRememberedUser(IDataService dataService, AuthService authService)
        {
            try
            {
                bool isRemembered = Preferences.Default.Get("IsRemembered", false);

                if (!isRemembered)
                    return;

                string email = Preferences.Default.Get("UserEmail", string.Empty);
                string password = Preferences.Default.Get("UserPassword", string.Empty);

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    return;

                var user = await dataService.GetUserAsync(email, password);

                if (user == null)
                    return;

                App.LoggedInUser = user;

                if (Shell.Current is AppShell appShell)
                {
                    appShell.SetLoggedInState(true, user);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    
    }
}