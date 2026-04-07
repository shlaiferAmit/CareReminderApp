using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CareReminderApp.Services;
using CareReminderApp.Models;
using CareReminderApp.Views;

namespace CareReminderApp.ViewModels
{
    [QueryProperty(nameof(CurrentUser), "CurrentUser")]
    public partial class FamilyDashboardViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private User? _currentUser;

        [ObservableProperty]
        private string _welcomeMessage = "שלום!";

        public FamilyDashboardViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        partial void OnCurrentUserChanged(User? value)
        {
            if (value != null)
            {
                // עדכון הודעת הפתיחה כשמתקבל משתמש
                WelcomeMessage = $"בוקר טוב, {value.FirstName}!";
            }
        }

        [RelayCommand]
        private async Task AddSenior()
        {
            // ניווט לדף הוספת קשיש
            await Shell.Current.GoToAsync(nameof(EldersListPage), new Dictionary<string, object>
            {
                { "CurrentUser", CurrentUser! }
            });
        }

        [RelayCommand]
        private async Task NavigateToProfile()
        {
            await Shell.Current.GoToAsync(nameof(ProfilePage), new Dictionary<string, object>
            {
                { "CurrentUser", CurrentUser! }
            });
        }

        [RelayCommand]
        private async Task NavigateToSeniors()
        {
            if (CurrentUser == null) return;

            await Shell.Current.GoToAsync(nameof(EldersListPage), new Dictionary<string, object>
    {
        { "CurrentUser", CurrentUser }
    });
        }
    }
}