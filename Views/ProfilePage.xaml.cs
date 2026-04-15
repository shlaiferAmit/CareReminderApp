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

            // בכל פעם שהדף עולה, אנחנו מוודאים שה-ViewModel מעודכן במשתמש הנוכחי
            if (BindingContext is ProfileViewModel vm)
            {
                // כאן אנחנו מוודאים שהפרטים נמשכים מהאובייקט הגלובלי ששמרנו בלוגין
                vm.DisplayUser = App.LoggedInUser;
            }
        }
    }
}