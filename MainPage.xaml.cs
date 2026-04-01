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
        }

        public MainPage(IDataService dataService)
        {
            InitializeComponent();
        }

        // ודאי שהשם הזה זהה למה שכתוב ב-XAML ב-Clicked
        private async void OnSignInClicked(object sender, EventArgs e)
        {
            // השתמשי בשם שנרשם ב-AppShell
            await Shell.Current.GoToAsync("SignInPage");
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            // ודאי שהשם כאן זהה בדיוק למה שכתבת ב-Routing.RegisterRoute
            await Shell.Current.GoToAsync("SignUpPage");
        }
    }
}