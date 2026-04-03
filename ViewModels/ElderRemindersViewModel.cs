using CareReminderApp.Models;
using CareReminderApp.Services;
using CareReminderApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareReminderApp.ViewModels
{
    public partial class ElderRemindersViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private User _currentUser;

        [ObservableProperty]
        private string _welcomeMessage = string.Empty;

        [ObservableProperty]
        private string _remindersSummaryText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Reminder> _elderRemindersList = new();

        public ElderRemindersViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("SelectedElder", out var elder))
            {
                CurrentUser = elder as User; // כאן CurrentUser הופך להיות המבוגר (למשל Esther)

                if (CurrentUser != null)
                {
                    WelcomeMessage = $"{CurrentUser.FirstName}'s Profile";
                    _ = LoadRemindersAsync(); // טוען רק את התזכורות של המבוגר הספציפי הזה
                }
            }
        }

        public async Task LoadRemindersAsync()
        {
            if (CurrentUser == null) return;

            var result = await _dataService.GetRemindersAsync(CurrentUser.Id);

            // המשתנה הזה נוצר אוטומטית בזכות ה-partial וה-ObservableProperty
            ElderRemindersList = new ObservableCollection<Reminder>(result);

            var count = ElderRemindersList.Count(r => !r.IsCompleted);
            RemindersSummaryText = $"You have {count} reminders today";
        }

        [RelayCommand]
        private async Task NavigateToTodayReminders() =>
            await Shell.Current.GoToAsync("TodayRemindersPage", new Dictionary<string, object> { { "CurrentUser", CurrentUser } });

        [RelayCommand]
        private async Task NavigateToProfile() =>
            await Shell.Current.GoToAsync("ProfilePage", new Dictionary<string, object> { { "CurrentUser", CurrentUser } });
    }
}