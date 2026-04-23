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
                // במקום להגיד לו "קח את מה שזכור לך", נגיד לו "תתרענן מהשרת"
                vm.ApplyQueryAttributes(new Dictionary<string, object>());
            }
        }
    }
}