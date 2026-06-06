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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is TodayRemindersViewModel vm)
            {
                // 🚀 הפעלת ההאזנה הריאלטימית מיד עם פתיחת המסך
                vm.StartListeningReminders();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (BindingContext is TodayRemindersViewModel vm)
            {
                // 🛑 סגירת הצינור כשעוברים למסך אחר כדי לחסוך משאבים
                vm.StopListeningReminders();
            }
        }
    }
}