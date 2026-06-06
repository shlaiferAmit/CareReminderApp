using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CareReminderApp.Models;
using CareReminderApp.Views;
using CareReminderApp.Services;
using System.Collections.ObjectModel;
using System;
using System.Linq;

namespace CareReminderApp.ViewModels
{
    public partial class TodayRemindersViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private IDisposable _remindersSubscription; // 👈 המנוי ששומר על הצינור הפתוח

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WelcomeGreeting))]
        private string userFirstName = "User";

        [ObservableProperty]
        private string userId;

        [ObservableProperty]
        private int totalRemindersCount = 0;

        [ObservableProperty]
        private ObservableCollection<Reminder> reminders = new();

        public TodayRemindersViewModel(IDataService dataService)
        {
            _dataService = dataService;

            if (App.LoggedInUser != null)
            {
                UserId = App.LoggedInUser.Id;
                UserFirstName = App.LoggedInUser.FirstName;
            }
        }

        /// <summary>
        /// פותח צינור האזנה בזמן אמת לכל התזכורות של היום
        /// </summary>
        public void StartListeningReminders()
        {
            if (string.IsNullOrEmpty(UserId)) return;

            // ליתר ביטחון, אם יש האזנה קודמת פתוחה - נסגור אותה
            StopListeningReminders();

            _remindersSubscription = _dataService.ListenRemindersForElder(UserId)
                .Subscribe(
                    userReminders =>
                    {
                        // סינון התזכורות כך שיוצגו רק אלו של היום הנוכחי
                        var today = DateTime.Today;
                        var filteredTodayList = userReminders
                            .Where(r => r.DueDate.Date == today)
                            .OrderBy(r => r.DueDate)
                            .ToList();

                        // עדכון ה-UI תמיד על ה-MainThread כדי למנוע קריסות
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Reminders.Clear();
                            foreach (var reminder in filteredTodayList)
                            {
                                Reminders.Add(reminder);
                            }

                            TotalRemindersCount = Reminders.Count;

                            // עדכון ה-Properties המחושבים ב-XAML
                            OnPropertyChanged(nameof(WelcomeGreeting));
                            OnPropertyChanged(nameof(RemindersSummary));
                        });
                    },
                    error =>
                    {
                        System.Diagnostics.Debug.WriteLine($"Realtime Stream Error: {error.Message}");
                    }
                );
        }

        /// <summary>
        /// סוגר את צינור ההאזנה
        /// </summary>
        public void StopListeningReminders()
        {
            if (_remindersSubscription != null)
            {
                _remindersSubscription.Dispose();
                _remindersSubscription = null;
            }
        }

        public string WelcomeGreeting => $"Good Morning, {UserFirstName}";
        public string RemindersSummary => $"You have {TotalRemindersCount} reminders today";

        [RelayCommand]
        public async Task NavigateToReminderDetails(Reminder reminder)
        {
            if (reminder == null) return;

            await Shell.Current.GoToAsync(nameof(ReminderDetailsPage), new Dictionary<string, object>
            {
                { "SelectedReminder", reminder }
            });
        }

        [RelayCommand]
        public async Task UpdateReminderStatusAsync(Reminder reminder)
        {
            if (reminder == null) return;

            // מעדכן את הסטטוס בפיירבייס/מוק - מכיוון שיש האזנה ברקע, 
            // אין צורך לקרוא ל-LoadData ידנית! המסך יתעדכן מעצמו ברגע שהשרת ישתנה
            await _dataService.UpdateReminderAsync(reminder);
        }
    }
}