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
        [NotifyPropertyChangedFor(nameof(RemindersCountMessage))]
        private ObservableCollection<Reminder> _elderRemindersList = new();

        [ObservableProperty]
        private ObservableCollection<PendingConnection> _pendingRequests = new();

        [ObservableProperty]
        private bool _hasPendingRequests;

        // --- בנאי (Constructor) מעודכן ---
        public ElderRemindersViewModel(IDataService dataService)
        {
            _dataService = dataService;

            // בדיקה אם המשתמש כבר מחובר במערכת (מניעת מצב של 0 תזכורות בטעינה ראשונה)
            if (App.LoggedInUser != null)
            {
                CurrentUser = App.LoggedInUser;
                WelcomeMessage = $"Good Morning, {CurrentUser.FirstName}";

                // הרצת טעינת נתונים ראשונית ברקע
                _ = InitializeDataAsync();
            }
        }

        // מאפיין מחושב עבור הטקסט בכרטיסייה
        public string RemindersCountMessage
        {
            get
            {
                int count = ElderRemindersList?.Count ?? 0;
                return count == 1 ? "You have 1 reminder today" : $"You have {count} reminders today";
            }
        }

        // ניהול קבלת נתונים בניווט (נשאר עבור גמישות)
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("CurrentUser", out var user) || query.TryGetValue("SelectedElder", out user))
            {
                var incomingUser = user as User;
                if (incomingUser != null)
                {
                    CurrentUser = incomingUser;
                    WelcomeMessage = $"Good Morning, {CurrentUser.FirstName}";
                    _ = InitializeDataAsync();
                }
            }
        }

        private async Task InitializeDataAsync()
        {
            await LoadRemindersAsync();
            await CheckPendingRequestsAsync();
        }

        // טעינת תזכורות מ-Firebase
        public async Task LoadRemindersAsync()
        {
            if (CurrentUser == null) return;

            try
            {
                // שליפת התזכורות לפי ה-ID של המבוגר המחובר
                var result = await _dataService.GetRemindersAsync(CurrentUser.Id);

                if (result != null)
                {
                    // עדכון הרשימה יפעיל את ה-NotifyPropertyChanged עבור ה-Count
                    ElderRemindersList = new ObservableCollection<Reminder>(result);
                }
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "Could not load reminders", "OK");
            }
        }

        // בדיקה האם יש בקשות הצטרפות ממשפחה שמחכות לאישור
        public async Task CheckPendingRequestsAsync()
        {
            if (CurrentUser == null) return;

            try
            {
                var requests = await _dataService.GetPendingForElderAsync(CurrentUser.Id);
                PendingRequests = new ObservableCollection<PendingConnection>(requests);
                HasPendingRequests = PendingRequests.Any();
            }
            catch
            {
                HasPendingRequests = false;
            }
        }

        [RelayCommand]
        private async Task ApproveRequest(PendingConnection request)
        {
            if (request == null) return;
            await _dataService.ApproveConnectionAsync(request);
            await CheckPendingRequestsAsync();
            await Shell.Current.DisplayAlert("Success", "Connection approved!", "OK");
        }

        [RelayCommand]
        private async Task RejectRequest(PendingConnection request)
        {
            if (request == null) return;
            await _dataService.RejectConnectionAsync(request);
            await CheckPendingRequestsAsync();
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
            // שימוש ב-nameof מונע שגיאות כתיב ב-Route
            await Shell.Current.GoToAsync("ProfilePage", new Dictionary<string, object>
            {
                { "CurrentUser", CurrentUser }
            });
        }

        [RelayCommand]
        private async Task Refresh()
        {
            await LoadRemindersAsync();
            await CheckPendingRequestsAsync();
        }
    }
}