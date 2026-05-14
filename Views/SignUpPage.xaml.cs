using Microsoft.Maui.Controls;
using CareReminderApp.ViewModels;
using CareReminderApp.Services;



namespace CareReminderApp.Views
{
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage(SignUpPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);

        }

    }
}