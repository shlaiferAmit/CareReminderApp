using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    public partial class ChangeProfilePage : ContentPage
    {
        public ChangeProfilePage(ProfileViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}