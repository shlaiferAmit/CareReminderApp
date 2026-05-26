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
    // מחלקת מודל תצוגה עבור מסך התזכורות של קשיש
    // אחראית על טעינת תזכורות, הצגת סטטוס התקדמות, טיפול בבקשות חיבור וניהול ניווט למסכים נוספים
    public partial class ElderRemindersViewModel : ObservableObject, IQueryAttributable
    {
        // שירות הנתונים של האפליקציה (פיירבייס או שירות מדומה)
        private readonly IDataService _dataService;

        // המשתמש המחובר כרגע
        [ObservableProperty]
        private User? _currentUser;

        // הודעת ברכה למשתמש
        [ObservableProperty]
        private string _welcomeMessage = string.Empty;

        // רשימת כל התזכורות של הקשיש
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RemindersCountMessage))]
        private ObservableCollection<Reminder> _elderRemindersList = new();

        // רשימת בקשות חיבור ממתינות
        [ObservableProperty]
        private ObservableCollection<PendingConnection> _pendingRequests = new();

        // האם יש בקשות חיבור ממתינות
        [ObservableProperty]
        private bool _hasPendingRequests;

        // ערך התקדמות (כמה תזכורות הושלמו מתוך כלל התזכורות)
        [ObservableProperty]
        private double _progressValue;

        // טקסט המציג את מצב ההתקדמות
        [ObservableProperty]
        private string _progressText = string.Empty;

        // התזכורת הבאה שעדיין לא בוצעה
        [ObservableProperty]
        private Reminder? _nextReminder;

        // האם להציג את התזכורת הבאה
        [ObservableProperty]
        private bool _isNextReminderVisible;

        // האם קיימות תזכורות כלל
        [ObservableProperty]
        private bool _hasReminders;

        public ElderRemindersViewModel(IDataService dataService)
        {
            _dataService = dataService;

            // אם יש משתמש מחובר, טוענים את הנתונים שלו
            if (App.LoggedInUser != null)
            {
                CurrentUser = App.LoggedInUser;
                WelcomeMessage = $"Good Morning, {CurrentUser.FirstName}";
                _ = InitializeDataAsync();
            }
        }

        // טעינת כל התזכורות של המשתמש וחישוב סטטיסטיקות
        public async Task LoadRemindersAsync()
        {
            if (CurrentUser == null) return;

            try
            {
                var result = await _dataService.GetRemindersAsync(CurrentUser.Id);

                if (result != null)
                {
                    var allToday = result.ToList();
                    ElderRemindersList = new ObservableCollection<Reminder>(allToday);

                    int total = allToday.Count;
                    int completed = allToday.Count(r => r.IsCompleted);

                    HasReminders = total > 0;
                    ProgressValue = total > 0 ? (double)completed / total : 0;
                    ProgressText = $"{completed} out of {total} completed today";

                    // מציאת התזכורת הקרובה ביותר שעדיין לא הושלמה
                    NextReminder = allToday
                        .Where(r => !r.IsCompleted)
                        .OrderBy(r => r.DueDate)
                        .FirstOrDefault();

                    // קביעה האם להציג את התזכורת הבאה
                    IsNextReminderVisible = NextReminder != null;
                }
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "Could not load reminders", "OK");
            }
        }

        // הודעה המציגה כמה תזכורות קיימות
        public string RemindersCountMessage
        {
            get
            {
                int count = ElderRemindersList?.Count ?? 0;
                return count == 1 ? "You have 1 reminder today" : $"You have {count} reminders today";
            }
        }

        // קבלת פרמטרים מהמסך הקודם
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("CurrentUser", out var user) || query.TryGetValue("SelectedElder", out user))
            {
                if (user is User incomingUser)
                {
                    CurrentUser = incomingUser;
                    WelcomeMessage = $"Good Morning, {CurrentUser.FirstName}";
                    _ = InitializeDataAsync();
                }
            }
        }

        // אתחול כל הנתונים במסך
        private async Task InitializeDataAsync()
        {
            await LoadRemindersAsync();
            await CheckPendingRequestsAsync();
        }

        // בדיקת בקשות חיבור ממתינות
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

        // אישור בקשת חיבור
        [RelayCommand]
        private async Task ApproveRequest(PendingConnection request)
        {
            if (request == null) return;
            await _dataService.ApproveConnectionAsync(request);
            await CheckPendingRequestsAsync();
        }

        // דחיית בקשת חיבור
        [RelayCommand]
        private async Task RejectRequest(PendingConnection request)
        {
            if (request == null) return;
            await _dataService.RejectConnectionAsync(request);
            await CheckPendingRequestsAsync();
        }

        // מעבר למסך פרטי תזכורת
        [RelayCommand]
        private async Task NavigateToReminderDetails(Reminder reminder)
        {
            if (reminder == null) return;
            await Shell.Current.GoToAsync(nameof(ReminderDetailsPage), new Dictionary<string, object>
            {
                { "SelectedReminder", reminder }
            });
        }

        // רענון נתוני המסך
        [RelayCommand]
        private async Task Refresh()
        {
            await LoadRemindersAsync();
            await CheckPendingRequestsAsync();
        }
    }
}