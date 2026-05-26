// עמוד פתיחה ראשי של האפליקציה
// מציג אפשרויות כניסה והרשמה למשתמש

using Microsoft.Maui.Controls;
using CareReminderApp.Views;
using CareReminderApp.Services;

namespace CareReminderApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // ביטול תפריט צד בעמוד הראשי
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        }

        public MainPage(IDataService dataService)
        {
            InitializeComponent();
        }

        // מעבר לעמוד התחברות
        private async void OnSignInClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SignInPage");
        }

        // מעבר לעמוד הרשמה
        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SignUpPage");
        }
    }
}