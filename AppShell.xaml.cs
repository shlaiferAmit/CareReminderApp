using CareReminderApp.Views;
using Microsoft.Maui.Controls;
using CareReminderApp.Models;

namespace CareReminderApp
{
    public partial class AppShell : Shell
    {
        public bool IsUserLoggedIn { get; private set; }

        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;

            RegisterRoutes();


        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(AddReminderPage), typeof(AddReminderPage));
            Routing.RegisterRoute(nameof(ReminderDetailsPage), typeof(ReminderDetailsPage));
            Routing.RegisterRoute(nameof(ChangeProfilePage), typeof(ChangeProfilePage));
            Routing.RegisterRoute(nameof(ElderProfilePage), typeof(ElderProfilePage));
        }

        private void BuildTabs(User currentUser)
        {
            for (int i = this.Items.Count - 1; i >= 0; i--)
            {
                if (this.Items[i] is TabBar)
                    this.Items.RemoveAt(i);
            }

            if (currentUser == null)
                return;

            TabBar mainTabBar = new TabBar();

            // --- תפריט למשתמש משפחה ---
            if (currentUser.Role == UserRole.FamilyMember)
            {
                var familyTab = new Tab { Title = "Home", Icon = "home.png" };
                familyTab.Items.Add(new ShellContent
                {
                    ContentTemplate = new DataTemplate(typeof(FamilyDashboardPage)),
                    Route = "FamilyDashboardPage"
                });
                mainTabBar.Items.Add(familyTab);

            }
            // --- תפריט למשתמש מבוגר ---
            else if (currentUser.Role == UserRole.Senior)
            {
                var elderTab = new Tab { Title = "Home", Icon = "home.png" };
                elderTab.Items.Add(new ShellContent
                {
                    ContentTemplate = new DataTemplate(typeof(ElderRemindersPage)),
                    Route = "ElderRemindersPage"
                });
                mainTabBar.Items.Add(elderTab);

                var todayTab = new Tab { Title = "Today", Icon = "list_icon.png" };
                todayTab.Items.Add(new ShellContent
                {
                    ContentTemplate = new DataTemplate(typeof(TodayRemindersPage)),
                    Route = "TodayRemindersPage"
                });
                mainTabBar.Items.Add(todayTab);
            }

            // --- טאב פרופיל משותף לכולם ---
            var profileTab = new Tab { Title = "Profile", Icon = "profile_icon.png" };
            profileTab.Items.Add(new ShellContent
            {
                ContentTemplate = new DataTemplate(typeof(ProfilePage)),
                Route = "ProfilePage"
            });
            mainTabBar.Items.Add(profileTab);

            this.Items.Add(mainTabBar);
        }

        public async void SetLoggedInState(bool isLoggedIn, User? currentUser = null)
        {
            IsUserLoggedIn = isLoggedIn;
            App.LoggedInUser = currentUser;

            this.FlyoutBehavior = isLoggedIn
    ? FlyoutBehavior.Flyout
    : FlyoutBehavior.Disabled;


            BuildTabs(currentUser);

            if (isLoggedIn && currentUser != null)
            {
                await Task.Delay(100);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (this.Items.FirstOrDefault(i => i is TabBar) is TabBar mainBar)
                    {
                        this.CurrentItem = mainBar.Items[0];
                    }
                });
            }
            else
            {
                await Shell.Current.GoToAsync("//MainPage");
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            this.FlyoutIsPresented = false;
            Preferences.Default.Clear();
            App.LoggedInUser = null;

            SetLoggedInState(false, null);

            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}