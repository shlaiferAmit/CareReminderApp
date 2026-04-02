using Microsoft.Maui.Controls;
using CareReminderApp.Services;
using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    public partial class FamilyDashboardPage : ContentPage
    {
        public FamilyDashboardPage()
        {
            InitializeComponent();
            BindingContext = new FamilyDashboardViewModel(new MockDataService());
        }
    }
}