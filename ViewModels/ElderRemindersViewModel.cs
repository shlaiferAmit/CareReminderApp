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
        [NotifyPropertyChangedFor(nameof(RemindersCountMessage))] // מעדכן את הטקסט בכל פעם שהרשימה משתנה
        private ObservableCollection<Reminder> _elderRemindersList = new();

        [ObservableProperty]
        private ObservableCollection<PendingConnection> _pendingRequests = new();

        [ObservableProperty]
        private bool _hasPendingRequests;

        public ElderRemindersViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        // מאפיין מחושב עבור הטקסט בכרטיסייה הכתומה
        public string RemindersCountMessage
        {
            get
            {
                int count = ElderRemindersList?.Count ?? 0;
                return count == 1 ? "You have 1 reminder today" : $"You have {count} reminders today";
            }
        }

        // ניהול קבלת נתונים בניווט
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("CurrentUser", out var user) || query.TryGetValue("SelectedElder", out user))
            {
                CurrentUser = user as User;

                if (CurrentUser != null)
                {
                    WelcomeMessage = $"Good Morning, {CurrentUser.FirstName}";

                    // רענון נתונים ראשוני
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await LoadRemindersAsync();
                        await CheckPendingRequestsAsync();
                    });
                }
            }
        }

        // טעינת תזכורות מ-Firebase
        public async Task LoadRemindersAsync()
        {
            if (CurrentUser == null) return;

            try
            {
                var result = await _dataService.GetRemindersAsync(CurrentUser.Id);

                if (result != null)
                {
                    // עדכון הרשימה יפעיל אוטומטית את עדכון ה-RemindersCountMessage
                    ElderRemindersList = new ObservableCollection<Reminder>(result);
                }
            }
            catch (Exception)
            {
                // הסרת ex כדי למנוע אזהרת משתנה לא בשימוש
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

        // פקודה למעבר לדף פרטי תזכורת ספציפית
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