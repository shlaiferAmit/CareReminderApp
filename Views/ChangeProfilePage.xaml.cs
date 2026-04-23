using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    public partial class ChangeProfilePage : ContentPage
    {
        // התיקון: החלפת ProfileViewModel ב-ChangeProfileViewModel
        public ChangeProfilePage(ChangeProfileViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}