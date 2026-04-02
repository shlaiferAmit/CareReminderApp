using Microsoft.Maui.Controls;
using CareReminderApp.ViewModels;
using CareReminderApp.Services;

namespace CareReminderApp.Views
{
    public partial class EldersListPage : ContentPage
    {
        public EldersListPage(EldersListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}