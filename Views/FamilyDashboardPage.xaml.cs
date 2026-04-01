using Microsoft.Maui.Controls;
using CareReminderApp.Services;
using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    public partial class FamilyDashboardPage : ContentPage
    {
        public FamilyDashboardPage(IDataService dataService)
        {
            InitializeComponent();
            // כאן אנחנו מחברים את ה-ViewModel ומעבירים לו את ה-Service
            BindingContext = new FamilyDashboardViewModel(dataService);
        }
    }
}