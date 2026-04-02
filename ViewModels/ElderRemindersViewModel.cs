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
using System.Collections.Generic;
using CareReminderApp.Views;

namespace CareReminderApp.ViewModels
{
    [QueryProperty(nameof(CurrentUser), "CurrentUser")]
    public partial class ElderRemindersViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private User _currentUser;

        [ObservableProperty]
        private string _welcomeMessage;

        [ObservableProperty]
        private int _remindersCount;

        [ObservableProperty]
        private string _remindersSummaryText;

        public ElderRemindersViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("CurrentUser"))
            {
                CurrentUser = query["CurrentUser"] as User;
                if (CurrentUser != null)
                {
                    WelcomeMessage = $"Good Morning, {CurrentUser.FirstName}";
                    // הרצה של טעינת הנתונים
                    _ = LoadRemindersAsync();
                }
            }
        }

        // פונקציה ציבורית כדי שנוכל לקרוא לה גם מה-Page
        public async Task LoadRemindersAsync()
        {
            if (CurrentUser == null) return;

            var reminders = await _dataService.GetRemindersAsync(CurrentUser.Id);

            // סופר רק תזכורות שעוד לא סומנו כבוצעו
            RemindersCount = reminders.Count(r => !r.IsCompleted);

            RemindersSummaryText = $"You have {RemindersCount} reminders today";
        }

        [RelayCommand]
        private async Task NavigateToTodayReminders()
        {
            await Shell.Current.GoToAsync(nameof(TodayRemindersPage), new Dictionary<string, object>
            {
                { "CurrentUser", CurrentUser }
            });
        }

        [RelayCommand]
        private async Task NavigateToProfile()
        {
            await Shell.Current.GoToAsync(nameof(ProfilePage), new Dictionary<string, object>
            {
                { "CurrentUser", CurrentUser }
            });
        }
    }
}