using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareReminderApp.Models;
using CareReminderApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CareReminderApp.Views;

namespace CareReminderApp.ViewModels
{
    [QueryProperty(nameof(CurrentUser), "CurrentUser")]
    public partial class TodayRemindersViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private User _currentUser;

        public ObservableCollection<Reminder> Reminders { get; } = new();

        public TodayRemindersViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("CurrentUser"))
            {
                CurrentUser = query["CurrentUser"] as User;
                await LoadRemindersAsync();
            }
        }

        private async Task LoadRemindersAsync()
        {
            var data = await _dataService.GetRemindersAsync(CurrentUser.Id);
            Reminders.Clear();
            foreach (var item in data)
            {
                Reminders.Add(item);
            }
        }

        // פונקציה לעדכון סטטוס תזכורת
        public async Task UpdateReminderStatusAsync(Reminder reminder)
        {
            await _dataService.UpdateReminderAsync(reminder);
            // כאן אפשר להוסיף הודעה "כל הכבוד!" אם הוא סימן בוצע
        }
    }
}
