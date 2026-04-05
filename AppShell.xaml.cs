using CareReminderApp.Views;
using System.ComponentModel;

namespace CareReminderApp
{
    public partial class AppShell : Shell, INotifyPropertyChanged
    {
        private bool _isUserLoggedIn;
        public bool IsUserLoggedIn
        {
            get => _isUserLoggedIn;
            set
            {
                _isUserLoggedIn = value;
                OnPropertyChanged(nameof(IsUserLoggedIn));
            }
        }

        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;
            RegisterRoutes();

            // Force the app to start on the Main Page
            this.CurrentItem = MainPageContent;
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
            Routing.RegisterRoute(nameof(SignInPage), typeof(SignInPage));
            Routing.RegisterRoute(nameof(FamilyDashboardPage), typeof(FamilyDashboardPage));
            Routing.RegisterRoute(nameof(EldersListPage), typeof(EldersListPage));
            Routing.RegisterRoute(nameof(AddReminderPage), typeof(AddReminderPage));
            Routing.RegisterRoute(nameof(ElderRemindersPage), typeof(ElderRemindersPage));
            Routing.RegisterRoute(nameof(TodayRemindersPage), typeof(TodayRemindersPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(ReminderDetailsPage), typeof(ReminderDetailsPage));
            Routing.RegisterRoute(nameof(ChangeProfilePage), typeof(ChangeProfilePage));
        }

        public void SetLoggedInState(bool isLoggedIn)
        {
            MainThread.BeginInvokeOnMainThread(() => {
                // Flyout - מציג את ה-3 קווים ומאפשר לחיצה
                // Disabled - מסתיר את הכל (למסך התחברות)
                this.FlyoutBehavior = isLoggedIn ? FlyoutBehavior.Flyout : FlyoutBehavior.Disabled;

                // מוודא שהסרגל למעלה גלוי כדי שיהיה על מה ללחוץ
                Shell.SetNavBarIsVisible(this.CurrentPage, isLoggedIn);
            });
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            // 1. Reset login state and hide menus
            SetLoggedInState(false);

            // 2. Clear stored user session
            Preferences.Default.Clear();
            App.LoggedInUser = null;

            // 3. Navigate to Main and reset the navigation stack
            // Using // ensures the UI refreshes and removes tabs/sidebars
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}