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
    }
}