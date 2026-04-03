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
        private string userFirstName = "Amit";

        [ObservableProperty]
        private string userId = "101";

        [ObservableProperty]
        private int totalRemindersCount = 0;

        [ObservableProperty]
        private ObservableCollection<Reminder> reminders = new();

        [ObservableProperty]
        private Reminder? upcomingReminder;

        public TodayRemindersViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            try
            {
                // שימוש ב-UserId (U גדולה) שנוצר מה-Toolkit
                var userReminders = await _dataService.GetRemindersByUserIdAsync(UserId);

                if (userReminders != null)
                {
                    Reminders = new ObservableCollection<Reminder>(userReminders);
                    TotalRemindersCount = Reminders.Count;
                    UpcomingReminder = Reminders.FirstOrDefault();
                }

                OnPropertyChanged(nameof(WelcomeGreeting));
                OnPropertyChanged(nameof(RemindersSummary));
            }
            catch (Exception ex)
            {
                // כאן אפשר להוסיף לוג לשגיאות במידת הצורך
            }
        }

        public string WelcomeGreeting
        {
            get
            {
                var hour = DateTime.Now.Hour;
                string greeting = hour switch
                {
                    >= 5 and < 12 => "Good Morning",
                    >= 12 and < 18 => "Good Afternoon",
                    >= 18 and < 22 => "Good Evening",
                    _ => "Good Night"
                };
                return $"{greeting}, {UserFirstName}";
            }
        }

        public string RemindersSummary => $"You have {TotalRemindersCount} reminds today";

        [RelayCommand]
        public async Task UpdateReminderStatusAsync(Reminder reminder)
        {
            if (reminder == null) return;
            await _dataService.UpdateReminderAsync(reminder);
            await LoadDataAsync(); // ריענון הרשימה לאחר העדכון
        }

        [RelayCommand]
        public async Task NavigateToSeniors() => await Shell.Current.GoToAsync("EldersListPage");

        [RelayCommand]
        public async Task NavigateToProfile() => await Shell.Current.GoToAsync("ProfilePage");

        [RelayCommand]
        public async Task NavigateToReminderDetails(Reminder reminder)
        {
            if (reminder == null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                { "SelectedReminder", reminder }
            };

            // ודאי שהשם כאן תואם לרישום ב-AppShell
            await Shell.Current.GoToAsync("ReminderDetailsPage", navigationParameter);
        }

        [RelayCommand]
        public async Task NavigateToTodayReminders()
        {
            await Shell.Current.GoToAsync("///TodayRemindersPage");
        }
    }
}