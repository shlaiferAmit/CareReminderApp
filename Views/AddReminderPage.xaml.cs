using Microsoft.Maui.Controls;
using CareReminderApp.ViewModels;
using CareReminderApp.Services;

namespace CareReminderApp.Views
{
    public partial class AddReminderPage : ContentPage
    {
        public AddReminderPage(IDataService dataService, string elderId)
        {
            InitializeComponent(); // ??? ????
            BindingContext = new AddReminderViewModel(dataService, elderId);
        }
    }
}