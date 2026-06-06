using CareReminderApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CareReminderApp.Views
{
    public partial class ElderProfilePage : ContentPage
    {
        private readonly ElderProfileViewModel _viewModel;

        public ElderProfilePage(ElderProfileViewModel vm)
        {
            InitializeComponent();
            BindingContext = _viewModel = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // הפעלת הזרמת הנתונים בזמן אמת כשהמסך עולה למסך
            _viewModel?.StartListeningReminders();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // כיבוי הצינור מיד כשהמשתמש מנווט החוצה
            _viewModel?.StopListeningReminders();
        }
    }
}