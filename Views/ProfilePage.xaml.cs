using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage(ProfileViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is ProfileViewModel vm)
            {
                vm.ApplyQueryAttributes(new Dictionary<string, object>());
            }
        }
    }
}