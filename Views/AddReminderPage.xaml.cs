using Microsoft.Maui.Controls;
using CareReminderApp.ViewModels;
using CareReminderApp.Services;

namespace CareReminderApp.Views
{
    [QueryProperty(nameof(UserId), "UserId")]
    public partial class AddReminderPage : ContentPage
    {
        public string UserId { get; set; }

        public AddReminderPage()
        {
            InitializeComponent();
        }
    }
}