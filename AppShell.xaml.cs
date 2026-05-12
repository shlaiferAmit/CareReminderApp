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

            // התחלה בעמוד הבית (MainPage - עמוד הלוגין/הרשמה)
            // ודאי שקיים ShellContent ב-XAML עם x:Name="MainPageContent"
            this.CurrentItem = MainPageContent;
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
            Routing.RegisterRoute(nameof(SignInPage), typeof(SignInPage));
            Routing.RegisterRoute(nameof(AddReminderPage), typeof(AddReminderPage));
            Routing.RegisterRoute(nameof(ReminderDetailsPage), typeof(ReminderDetailsPage));
            Routing.RegisterRoute(nameof(ChangeProfilePage), typeof(ChangeProfilePage));
            Routing.RegisterRoute(nameof(ElderProfilePage), typeof(ElderProfilePage));
            // הוספת דף הבית של המשפחה ל-Routes
            Routing.RegisterRoute(nameof(FamilyDashboardPage), typeof(FamilyDashboardPage));
        }

        private void BuildTabs(User currentUser)
        {
            // 1. הסרת כל TabBar קיים
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

                // הוסר הטאב של EldersListPage - הרשימה כבר נמצאת בתוך ה-Dashboard
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

            BuildTabs(currentUser);

            if (isLoggedIn && currentUser != null)
            {
                await Task.Delay(100);
                // ניווט לטאב הראשון שנוצר ב-TabBar החדש
                if (this.Items.FirstOrDefault(i => i is TabBar) is TabBar mainBar && mainBar.Items.Count > 0)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        this.CurrentItem = mainBar.Items[0];
                    });
                }
            }
            else
            {
                // חזרה לעמוד הראשי בניתוק
                this.CurrentItem = MainPageContent;
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