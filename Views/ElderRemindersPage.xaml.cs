using CareReminderApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CareReminderApp.Views
{
    public partial class ElderRemindersPage : ContentPage
    {
        public ElderRemindersPage(ElderRemindersViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // הדלקת הצינור בזמן אמת כשהמסך עולה
            if (BindingContext is ElderRemindersViewModel vm)
            {
                vm.StartRealtimeListeners();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // כיבוי הצינור כשהמשתמש יוצא מהדף כדי לחסוך סוללה ומשאבים
            if (BindingContext is ElderRemindersViewModel vm)
            {
                vm.StopRealtimeListeners();
            }
        }
    }
}