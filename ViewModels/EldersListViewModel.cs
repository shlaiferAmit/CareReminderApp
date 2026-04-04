using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CareReminderApp.Services;
using CareReminderApp.Models;
using CareReminderApp.Views;

namespace CareReminderApp.ViewModels
{
    public partial class EldersListViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private ObservableCollection<User> _elders = new();

        // זה המשתנה שהיה חסר וגרם לשגיאות!
        [ObservableProperty]
        private User _currentUser;

        public EldersListViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("CurrentUser", out var user))
            {
                CurrentUser = user as User;
                _ = LoadEldersAsync();
            }
        }

        public async Task LoadEldersAsync()
        {
            if (CurrentUser == null) return;

            // עכשיו הפקודה הזו תעבוד כי הוספנו אותה ל-IDataService
            var result = await _dataService.GetEldersForFamilyAsync(CurrentUser.Id);
            Elders = new ObservableCollection<User>(result);
        }

   

        [RelayCommand]
        private async Task GoToProfile(User selectedElder)
        {
            if (selectedElder == null) return;

            var navParam = new Dictionary<string, object>
    {
        { "SelectedUser", selectedElder } // המפתח הזה חייב להיות זהה למה שב-ProfileViewModel
    };
            await Shell.Current.GoToAsync(nameof(ProfilePage), navParam);
        }

        [RelayCommand]
        async Task Logout()
        {
            await Shell.Current.GoToAsync("//SignInPage");
        }

        [RelayCommand]
        async Task GoToHome()
        {
            // ניווט חזרה למסך הראשי (Family Dashboard)
            await Shell.Current.GoToAsync("//FamilyDashboardPage");
        }
    }
}