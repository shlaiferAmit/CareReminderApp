using Microsoft.Maui.Controls;
using CareReminderApp.ViewModels;
using CareReminderApp.Services;

namespace CareReminderApp.Views
{
    public partial class ElderRemindersPage : ContentPage
    {
        public ElderRemindersPage(IDataService dataService, string elderId)
        {
            InitializeComponent(); // חייב להיות אחד בלבד
            BindingContext = new ElderRemindersViewModel(dataService, elderId);
        }
    }
}