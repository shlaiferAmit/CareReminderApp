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
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        }

        public MainPage(IDataService dataService)
        {
            InitializeComponent();
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SignInPage");
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SignUpPage");
        }
    }
}