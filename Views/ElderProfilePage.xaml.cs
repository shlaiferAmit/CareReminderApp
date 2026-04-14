using CareReminderApp.ViewModels;

namespace CareReminderApp.Views;

public partial class ElderProfilePage : ContentPage
{
    public ElderProfilePage(ElderProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}