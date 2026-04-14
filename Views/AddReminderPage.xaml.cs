using CareReminderApp.ViewModels;

namespace CareReminderApp.Views;

public partial class AddReminderPage : ContentPage
{
    public AddReminderPage(AddReminderViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}