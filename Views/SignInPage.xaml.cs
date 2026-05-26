using CareReminderApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CareReminderApp.Views
{
    public partial class SignInPage : ContentPage
    {
        public SignInPage(SignInPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        }

        // חשוב: מאפס את השדות בכל פעם שנכנסים למסך
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is SignInPageViewModel vm)
            {
                vm.UserEmail = string.Empty;
                vm.UserPassword = string.Empty;
                vm.IsRememberMeSelected = false;
            }
        }
    }
}