using CareReminderApp.Models;
using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    // ודאי שהשם כאן תואם לקובץ ה-XAML שאת עובדת עליו
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

            // פותר את שגיאה CS1061 על ידי קריאה למתודה הציבורית שיצרנו
            if (BindingContext is TodayRemindersViewModel vm && reminder != null)
            {
                await vm.UpdateReminderStatusAsync(reminder);
            }
        }
    }
}