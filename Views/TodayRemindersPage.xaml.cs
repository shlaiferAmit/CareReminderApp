using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    public partial class TodayRemindersPage : ContentPage
    {
        public TodayRemindersPage(TodayRemindersViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is TodayRemindersViewModel vm)
            {
                await vm.LoadDataAsync();
            }
        }
    }
}