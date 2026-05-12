using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    public partial class FamilyDashboardPage : ContentPage
    {
        public FamilyDashboardPage(FamilyDashboardViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // טעינת רשימת המבוגרים בכל פעם שנכנסים לדף
            if (BindingContext is FamilyDashboardViewModel vm)
            {
                await vm.LoadEldersCommand.ExecuteAsync(null);
            }
        }
    }
}