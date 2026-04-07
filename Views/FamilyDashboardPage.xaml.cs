using CareReminderApp.Services;
using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    public partial class FamilyDashboardPage : ContentPage
    {
        public FamilyDashboardPage()
        {
            InitializeComponent();
            // הזרקת השירות ל-ViewModel
            BindingContext = new FamilyDashboardViewModel(new MockDataService());
        }
    }
}