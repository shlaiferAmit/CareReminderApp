
using CareReminderApp.Models;
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

        private async void OnReminderStatusChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkbox = (CheckBox)sender;
            var reminder = (Reminder)checkbox.BindingContext;

            if (BindingContext is TodayRemindersViewModel vm && reminder != null)
            {
                await vm.UpdateReminderStatusAsync(reminder);
            }
        }
    }
}