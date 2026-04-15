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

            // התחלה בעמוד הבית
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
        }

        // --- פונקציה לבניית ה-Tabs דינמית לפי סוג המשתמש ---
        private void BuildTabs(User currentUser)
        {
            // ✅ הסרת כל TabBar קיים
            for (int i = this.Items.Count - 1; i >= 0; i--)
            {
                if (this.Items[i] is TabBar)
                    this.Items.RemoveAt(i);
            }

            if (currentUser == null)
                return; // אין משתמש – לא יוצרים Tabs

            TabBar mainTabBar = new TabBar();

            if (currentUser.Role == UserRole.FamilyMember)
            {
                var familyTab = new Tab { Title = "Family Home", Icon = "home.png" };
                familyTab.Items.Add(new ShellContent
                {
                    ContentTemplate = new DataTemplate(typeof(FamilyDashboardPage)),
                    Route = "FamilyDashboardPage"
                });
                mainTabBar.Items.Add(familyTab);

                var elderListTab = new Tab { Title = "My Seniors", Icon = "list_icon.png" };
                elderListTab.Items.Add(new ShellContent
                {
                    ContentTemplate = new DataTemplate(typeof(EldersListPage)),
                    Route = "EldersListPage"
                });
                mainTabBar.Items.Add(elderListTab);
            }
            else if (currentUser.Role == UserRole.Senior)
            {
                var elderTab = new Tab { Title = "Elder Home", Icon = "home.png" };
                elderTab.Items.Add(new ShellContent
                {
                    ContentTemplate = new DataTemplate(typeof(ElderRemindersPage)),
                    Route = "ElderRemindersPage"
                });
                mainTabBar.Items.Add(elderTab);

                var todayTab = new Tab { Title = "Today’s Reminders", Icon = "list_icon.png" };
                todayTab.Items.Add(new ShellContent
                {
                    ContentTemplate = new DataTemplate(typeof(TodayRemindersPage)),
                    Route = "TodayRemindersPage"
                });
                mainTabBar.Items.Add(todayTab);
            }

            var profileTab = new Tab { Title = "My Profile", Icon = "profile_icon.png" };
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
            App.LoggedInUser = currentUser; // עכשיו זה תקין

            BuildTabs(currentUser);

            if (isLoggedIn && currentUser != null)
            {
                await Task.Delay(100);
                if (this.Items.LastOrDefault() is TabBar mainBar && mainBar.Items.Count > 0)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        this.CurrentItem = mainBar.Items[0];
                    });
                }
            }
            else
            {
                // ודאי ש-MainPageContent מוגדר ב-XAML שלך
                // this.CurrentItem = MainPageContent; 
            }
        }

        // --- Logout ---
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            // 1. סגירת התפריט הצידי באופן מיידי כדי שלא "יתקע" על המסך
            this.FlyoutIsPresented = false;

            // 2. ניקוי נתוני המשתמש מהזיכרון
            Preferences.Default.Clear();
            App.LoggedInUser = null;

            // 3. עדכון מצב ההתחברות ל-false
            // זה יגרום ל-BuildTabs למחוק את הטאבים ול-CurrentItem להפוך ל-MainPageContent
            SetLoggedInState(false, null);

            // 4. ניווט מפורש לדף ה-MainPage (ליתר ביטחון כדי לאפס את ה-Stack)
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}