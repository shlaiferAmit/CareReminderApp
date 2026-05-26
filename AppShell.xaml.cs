// מחלקת ה-Shell הראשית של האפליקציה
// אחראית על ניווט, בניית טאבים וניהול מצב התחברות משתמש

using CareReminderApp.Views;
using Microsoft.Maui.Controls;
using CareReminderApp.Models;

namespace CareReminderApp
{
    public partial class AppShell : Shell
    {
        // מציין האם המשתמש מחובר למערכת
        public bool IsUserLoggedIn { get; private set; }

        public AppShell()
        {
            InitializeComponent();

            // קביעת ה-BindingContext של ה-Shell לעצמו
            BindingContext = this;

            // רישום נתיבי ניווט בין דפים באפליקציה
            RegisterRoutes();
        }

        // רישום כל המסלולים לניווט פנימי באפליקציה
        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(AddReminderPage), typeof(AddReminderPage));
            Routing.RegisterRoute(nameof(ReminderDetailsPage), typeof(ReminderDetailsPage));
            Routing.RegisterRoute(nameof(ChangeProfilePage), typeof(ChangeProfilePage));
            Routing.RegisterRoute(nameof(ElderProfilePage), typeof(ElderProfilePage));
        }

        // בניית תפריט טאבים לפי סוג המשתמש
        private void BuildTabs(User currentUser)
        {
            // מחיקת טאבים קיימים לפני בנייה מחדש
            for (int i = this.Items.Count - 1; i >= 0; i--)
            {
                if (this.Items[i] is TabBar)
                    this.Items.RemoveAt(i);
            }

            // אם אין משתמש מחובר - לא בונים תפריט
            if (currentUser == null)
                return;

            TabBar mainTabBar = new TabBar();

            // תפריט למשתמש מסוג בן משפחה
            if (currentUser.Role == UserRole.FamilyMember)
            {
                var familyTab = new Tab { Title = "בית", Icon = "home.png" };

                familyTab.Items.Add(new ShellContent
                {
                    ContentTemplate = new DataTemplate(typeof(FamilyDashboardPage)),
                    Route = "FamilyDashboardPage"
                });

                mainTabBar.Items.Add(familyTab);
            }
            // תפריט למשתמש מסוג מבוגר
            else if (currentUser.Role == UserRole.Senior)
            {
                var elderTab = new Tab { Title = "בית", Icon = "home.png" };

                elderTab.Items.Add(new ShellContent
                {
                    ContentTemplate = new DataTemplate(typeof(ElderRemindersPage)),
                    Route = "ElderRemindersPage"
                });

                mainTabBar.Items.Add(elderTab);

                var todayTab = new Tab { Title = "היום", Icon = "list_icon.png" };

                todayTab.Items.Add(new ShellContent
                {
                    ContentTemplate = new DataTemplate(typeof(TodayRemindersPage)),
                    Route = "TodayRemindersPage"
                });

                mainTabBar.Items.Add(todayTab);
            }

            // טאב פרופיל משותף לכל המשתמשים
            var profileTab = new Tab { Title = "פרופיל", Icon = "profile_icon.png" };

            profileTab.Items.Add(new ShellContent
            {
                ContentTemplate = new DataTemplate(typeof(ProfilePage)),
                Route = "ProfilePage"
            });

            mainTabBar.Items.Add(profileTab);

            this.Items.Add(mainTabBar);
        }

        // עדכון מצב התחברות משתמש (כניסה / יציאה)
        public async void SetLoggedInState(bool isLoggedIn, User? currentUser = null)
        {
            IsUserLoggedIn = isLoggedIn;
            App.LoggedInUser = currentUser;

            // שינוי מצב תפריט צד לפי התחברות
            this.FlyoutBehavior = isLoggedIn
                ? FlyoutBehavior.Flyout
                : FlyoutBehavior.Disabled;

            // בניית טאבים מחדש לפי המשתמש
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

        // טיפול בלחיצת יציאה מהמערכת
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            this.FlyoutIsPresented = false;

            // ניקוי נתוני התחברות שמורים
            Preferences.Default.Clear();

            App.LoggedInUser = null;

            SetLoggedInState(false, null);

            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}