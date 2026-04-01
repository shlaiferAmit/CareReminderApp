using Microsoft.Maui.Controls;
using CareReminderApp.ViewModels;
using CareReminderApp.Services;

namespace CareReminderApp.Views
{
    public partial class SignInPage : ContentPage
    {
        public SignInPage(IDataService dataService)
        {
            InitializeComponent();
            BindingContext = new SignInPageViewModel(dataService);
        }
    }
}