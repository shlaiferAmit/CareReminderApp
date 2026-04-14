using CareReminderApp.ViewModels;

namespace CareReminderApp.Views;

public partial class EldersListPage : ContentPage
{
    public EldersListPage(EldersListViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is EldersListViewModel vm)
        {
            await vm.LoadEldersCommand.ExecuteAsync(null);
        }
    }
}