using Microsoft.Maui.Controls;
using CareReminderApp.ViewModels;
using CareReminderApp.Services;

namespace CareReminderApp.Views
{
    public partial class AddReminderPage : ContentPage
    {
        public AddReminderPage(AddReminderViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}