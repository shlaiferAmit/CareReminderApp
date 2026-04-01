using Microsoft.Maui.Controls;
using CareReminderApp.ViewModels;
using CareReminderApp.Services;

namespace CareReminderApp.Views
{
    public partial class EldersListPage : ContentPage
    {
        public EldersListPage(IDataService dataService, string userId)
        {
            InitializeComponent();
            BindingContext = new EldersListViewModel(dataService, userId);
        }
    }
}