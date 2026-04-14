using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CareReminderApp.Models;
using CareReminderApp.Views;
using CareReminderApp.Services;
using System.Collections.ObjectModel;

namespace CareReminderApp.ViewModels
{
    public partial class TodayRemindersViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

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

            // שליפת המשתמש המחובר מהאפליקציה
            if (App.LoggedInUser != null)
            {
                UserId = App.LoggedInUser.Id;
                UserFirstName = App.LoggedInUser.FirstName;
            }

            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(UserId)) return;

                var userReminders = await _dataService.GetRemindersByUserIdAsync(UserId);

                if (userReminders != null)
                {
                    Reminders = new ObservableCollection<Reminder>(userReminders);
                    TotalRemindersCount = Reminders.Count;
                }

                OnPropertyChanged(nameof(WelcomeGreeting));
                OnPropertyChanged(nameof(RemindersSummary));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
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
            await _dataService.UpdateReminderAsync(reminder);
            await LoadDataAsync();
        }
    }
}