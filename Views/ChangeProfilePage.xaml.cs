using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    public partial class ChangeProfilePage : ContentPage
    {
        public ChangeProfilePage(ChangeProfileViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}