using Microsoft.Maui.Controls;
using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    public partial class SignInPage : ContentPage
    {
        // אנחנו מקבלים את ה-ViewModel מוכן דרך הבנאי
        public SignInPage(SignInPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}