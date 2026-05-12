using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    public partial class ElderRemindersPage : ContentPage
    {
        public ElderRemindersPage(ElderRemindersViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ElderRemindersViewModel vm)
            {
                // רענון אוטומטי של הנתונים כדי לעדכן את IsNextReminderVisible
                await vm.LoadRemindersAsync();
                await vm.CheckPendingRequestsAsync();
            }
        }
    }
}