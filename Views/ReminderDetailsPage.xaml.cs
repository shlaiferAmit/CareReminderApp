using CareReminderApp.ViewModels;

namespace CareReminderApp.Views;

public partial class ReminderDetailsPage : ContentPage
{
    public ReminderDetailsPage(ReminderDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}