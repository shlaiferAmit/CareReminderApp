using CareReminderApp.Models;
using CareReminderApp.Services;
using CareReminderApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareReminderApp.ViewModels
{
    public partial class ElderRemindersViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IDataService _dataService;

        // משתני האזנה (Subscriptions) כדי שנוכל לנתק אותם כשהדף נסגר
        private IDisposable? _remindersSubscription;
        private IDisposable? _requestsSubscription;

        [ObservableProperty]
        private User? _currentUser;

        [ObservableProperty]
        private string _welcomeMessage = string.Empty;

        [ObservableProperty]
        private bool _hasPendingRequests;

        [ObservableProperty]
        private double _progressValue;

        [ObservableProperty]
        private string _progressText = string.Empty;

        [ObservableProperty]
        private Reminder? _nextReminder;

        [ObservableProperty]
        private bool _isNextReminderVisible;

        [ObservableProperty]
        private bool _hasReminders;

        // שינוי שמות האוספים כדי לעקוף ולדרוס את ה-Cache הבעייתי של ה-Source Generator
        public ObservableCollection<Reminder> ElderRemindersCollection { get; } = new();
        public ObservableCollection<PendingConnection> PendingRequestsCollection { get; } = new();

        public string RemindersCountMessage =>
            ElderRemindersCollection.Count == 1 ? "You have 1 reminder today" : $"You have {ElderRemindersCollection.Count} reminders today";

        public ElderRemindersViewModel(IDataService dataService)
        {
            _dataService = dataService;

            if (App.LoggedInUser != null)
            {
                CurrentUser = App.LoggedInUser;
                WelcomeMessage = $"Good Morning, {CurrentUser.FirstName}";
                StartRealtimeListeners();
            }
        }

        /// הפעלת צינורות האזנה בזמן אמת עבור תזכורות ובקשות חיבור
        public void StartRealtimeListeners()
        {
            if (CurrentUser == null) return;

            // ניקוי האזנות קודמות למניעת כפילויות בזיכרון
            StopRealtimeListeners();

            // 1. האזנה בזמן אמת לתזכורות
            _remindersSubscription = _dataService.ListenRemindersForElder(CurrentUser.Id)
                .Subscribe(list =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ElderRemindersCollection.Clear();
                        var allToday = list.ToList();

                        foreach (var reminder in allToday)
                        {
                            ElderRemindersCollection.Add(reminder);
                        }

                        // עדכון לוגיקת התקדמות וסטטיסטיקה בזמן אמת
                        int total = allToday.Count;
                        int completed = allToday.Count(r => r.IsCompleted);

                        HasReminders = total > 0;
                        ProgressValue = total > 0 ? (double)completed / total : 0;
                        ProgressText = $"{completed} out of {total} completed today";

                        NextReminder = allToday
                            .Where(r => !r.IsCompleted)
                            .OrderBy(r => r.DueDate)
                            .FirstOrDefault();

                        IsNextReminderVisible = NextReminder != null;

                        // מודיע למערכת שהודעת הכמות השתנתה
                        OnPropertyChanged(nameof(RemindersCountMessage));
                    });
                }, error => System.Diagnostics.Debug.WriteLine($"Reminders Stream error: {error.Message}"));

            // 2. האזנה בזמן אמת לבקשות חיבור ממתינות
            _requestsSubscription = _dataService.ListenPendingConnectionsForElder(CurrentUser.Id)
                .Subscribe(requests =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        PendingRequestsCollection.Clear();
                        foreach (var req in requests)
                        {
                            PendingRequestsCollection.Add(req);
                        }
                        HasPendingRequests = PendingRequestsCollection.Any();
                    });
                }, error => System.Diagnostics.Debug.WriteLine($"Requests Stream error: {error.Message}"));
        }

        /// ניתוק הצינורות כדי לחסוך בסוללה ובמשאבי רשת כשהמשתמש יוצא מהמסך
        public void StopRealtimeListeners()
        {
            _remindersSubscription?.Dispose();
            _remindersSubscription = null;

            _requestsSubscription?.Dispose();
            _requestsSubscription = null;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("CurrentUser", out var user) || query.TryGetValue("SelectedElder", out user))
            {
                if (user is User incomingUser)
                {
                    CurrentUser = incomingUser;
                    WelcomeMessage = $"Good Morning, {CurrentUser.FirstName}";
                    StartRealtimeListeners();
                }
            }
        }

        [RelayCommand]
        private async Task ApproveRequest(PendingConnection request)
        {
            if (request == null) return;
            // ה-Stream יעדכן ויעלים את הכרטיס הצהוב אוטומטית ברגע שהסטטוס ישתנה ב-Firebase
            await _dataService.ApproveConnectionAsync(request);
        }

        [RelayCommand]
        private async Task RejectRequest(PendingConnection request)
        {
            if (request == null) return;
            // ה-Stream יעדכן ויעלים את הכרטיס הצהוב אוטומטית
            await _dataService.RejectConnectionAsync(request);
        }

        [RelayCommand]
        private async Task NavigateToReminderDetails(Reminder reminder)
        {
            if (reminder == null) return;
            await Shell.Current.GoToAsync(nameof(ReminderDetailsPage), new Dictionary<string, object>
            {
                { "SelectedReminder", reminder }
            });
        }
    }
}