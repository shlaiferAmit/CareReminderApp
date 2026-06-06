using CareReminderApp.Models;
using CareReminderApp.Services;
using CareReminderApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CareReminderApp.ViewModels
{
    [QueryProperty(nameof(Elder), "Elder")]
    public partial class ElderProfileViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private IDisposable _remindersSubscription; // שומר את הרישום להאזנה בזמן אמת

        [ObservableProperty]
        private string title = "פרופיל מבוגר";

        [ObservableProperty]
        private User elder;

        [ObservableProperty]
        private ObservableCollection<Reminder> reminders;

        public ElderProfileViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Reminders = new ObservableCollection<Reminder>();
        }

        partial void OnElderChanged(User value)
        {
            if (value != null)
            {
                Title = $"פרופיל של {value.FirstName}";
                // ברגע שהקשיש נטען, אנחנו מאתחלים את ההאזנה אם המסך כבר באוויר
                StartListeningReminders();
            }
        }

        /// מתחיל האזנה בזמן אמת לשינויים בתזכורות של הקשיש הנוכחי
        public void StartListeningReminders()
        {
            if (Elder == null) return;

            // מונע כפילות האזנות אם הפונקציה נקראת פעמיים
            StopListeningReminders();

            _remindersSubscription = _dataService.ListenRemindersForElder(Elder.Id)
                .Subscribe(
                    updatedReminders =>
                    {
                        // מעדכן את ממשק המשתמש בחוט הראשי (UI Thread)
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Reminders.Clear();
                            foreach (var reminder in updatedReminders)
                            {
                                Reminders.Add(reminder);
                            }
                        });
                    },
                    ex => System.Diagnostics.Debug.WriteLine($"Error listening to reminders: {ex.Message}")
                );
        }

        /// עוצר את ההאזנה כדי למנוע זליגות זיכרון וצריכת סוללה מיותרת
        public void StopListeningReminders()
        {
            _remindersSubscription?.Dispose();
            _remindersSubscription = null;
        }

        [RelayCommand]
        private async Task AddReminder()
        {
            if (Elder == null) return;

            await Shell.Current.GoToAsync(nameof(AddReminderPage), new Dictionary<string, object>
            {
                { "SelectedElder", Elder }
            });
        }
    }
}