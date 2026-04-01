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

namespace CareReminderApp.ViewModels
{
    public partial class ElderRemindersViewModel : ObservableObject
    {
        public ObservableCollection<Reminder> Reminders { get; } = new();

        private readonly IDataService _dataService;
        private string _elderId;

        // 1. ה-Constructor הריק שחייב להיות כאן בשביל ה-XAML (יפתור את XLS0507)
        public ElderRemindersViewModel()
        {
        }

        // 2. ה-Constructor שבו את משתמשת כשאת עוברת לדף מהקוד
        public ElderRemindersViewModel(IDataService dataService, string elderId)
        {
            _dataService = dataService;
            _elderId = elderId;
            LoadReminders();
        }

        private async void LoadReminders()
        {
            // בדיקת בטיחות: אם השירות לא הוגדר (למשל בגלל שימוש ב-Constructor הריק), אל תמשיך
            if (_dataService == null || string.IsNullOrEmpty(_elderId))
                return;

            try
            {
                var reminders = await _dataService.GetRemindersByUserIdAsync(_elderId);

                if (reminders != null)
                {
                    Reminders.Clear();
                    foreach (var r in reminders)
                    {
                        Reminders.Add(r);
                    }
                }
            }
            catch (Exception ex)
            {
                // כאן תוכלי להוסיף הדפסה של השגיאה ל-Debug אם תרצי
                System.Diagnostics.Debug.WriteLine($"Error loading reminders: {ex.Message}");
            }
        }
    }
}