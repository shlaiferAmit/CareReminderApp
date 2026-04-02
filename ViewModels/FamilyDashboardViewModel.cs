using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private string _welcomeMessage = "Good Morning!";

        public FamilyDashboardViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        // המילה partial כאן פותרת את שגיאה CS0759
        partial void OnCurrentUserChanged(User? value)
        {
            if (value != null)
            {
                WelcomeMessage = $"Good Morning, {value.FirstName}";
            }
        }

        [RelayCommand]
        private async Task NavigateToSeniors()
        {
            // שיניתי ל-EldersListPage כדי שיתאים לקובץ שלך
            await Shell.Current.GoToAsync("EldersListPage", new Dictionary<string, object>
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
    }
}