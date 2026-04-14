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

        [ObservableProperty]
        private ObservableCollection<PendingConnection> _pendingRequests = new();

        [ObservableProperty]
        private bool _hasPendingRequests;

        public ElderRemindersViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        // ניהול קבלת נתונים בניווט
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // בדיקה האם הגענו עם CurrentUser (בדרך כלל מהתחברות) או SelectedElder
            if (query.TryGetValue("CurrentUser", out var user) || query.TryGetValue("SelectedElder", out user))
            {
                CurrentUser = user as User;

                if (CurrentUser != null)
                {
                    WelcomeMessage = $"שלום, {CurrentUser.FirstName}";

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
                    ElderRemindersList = new ObservableCollection<Reminder>(result);

                    var count = ElderRemindersList.Count(r => !r.IsCompleted);
                    RemindersSummaryText = count == 1
                        ? "You have 1 reminder today"
                        : $"You have {count} reminders today";
                }
            }
            catch (Exception ex)
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
            await _dataService.ApproveConnectionAsync(request);
            await CheckPendingRequestsAsync(); // רענון הרשימה לאחר אישור
            await Shell.Current.DisplayAlert("הצלחה", "החיבור אושר בהצלחה", "OK");
        }

        [RelayCommand]
        private async Task RejectRequest(PendingConnection request)
        {
            await _dataService.RejectConnectionAsync(request);
            await CheckPendingRequestsAsync(); // רענון הרשימה לאחר דחייה
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

        [RelayCommand]
        private async Task Refresh()
        {
            await LoadRemindersAsync();
            await CheckPendingRequestsAsync();
        }
    }
}