using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    public partial class FamilyDashboardPage : ContentPage
    {
        private readonly FamilyDashboardViewModel _viewModel;

        public FamilyDashboardPage(FamilyDashboardViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        // בכל פעם שהמסך עולה - מתחילים להקשיב בזמן אמת ל-Firebase
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel?.StartListeningElders();
        }

        // בכל פעם שהמשתמש עוזב את המסך - עוצרים את ההאזנה
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel?.StopListeningElders();
        }
    }
}