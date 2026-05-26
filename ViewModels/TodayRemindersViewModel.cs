using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CareReminderApp.Models;
using CareReminderApp.Views;
using CareReminderApp.Services;
using System.Collections.ObjectModel;

namespace CareReminderApp.ViewModels
{
    // מחלקת מודל תצוגה עבור מסך תזכורות יומיות
    // אחראית על הצגת תזכורות של המשתמש להיום, עדכון סטטוס וניווט לפרטי תזכורת
    public partial class TodayRemindersViewModel : ObservableObject
    {
        // שירות הנתונים של האפליקציה (פיירבייס או שירות מדומה)
        private readonly IDataService _dataService;

        // שם פרטי של המשתמש להצגת ברכה
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WelcomeGreeting))]
        private string userFirstName = "User";

        // מזהה המשתמש המחובר
        [ObservableProperty]
        private string userId;

        // כמות התזכורות הכוללת להיום
        [ObservableProperty]
        private int totalRemindersCount = 0;

        // רשימת כל התזכורות של היום
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

        // טעינת כל התזכורות של המשתמש מהשרת
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

                // עדכון תצוגת ברכה וסיכום
                OnPropertyChanged(nameof(WelcomeGreeting));
                OnPropertyChanged(nameof(RemindersSummary));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        // ברכת פתיחה למשתמש
        public string WelcomeGreeting => $"Good Morning, {UserFirstName}";

        // סיכום מספר התזכורות
        public string RemindersSummary => $"You have {TotalRemindersCount} reminders today";

        // מעבר למסך פרטי תזכורת
        [RelayCommand]
        public async Task NavigateToReminderDetails(Reminder reminder)
        {
            if (reminder == null) return;

            await Shell.Current.GoToAsync(nameof(ReminderDetailsPage), new Dictionary<string, object>
            {
                { "SelectedReminder", reminder }
            });
        }

        // עדכון סטטוס תזכורת ורענון רשימה
        [RelayCommand]
        public async Task UpdateReminderStatusAsync(Reminder reminder)
        {
            if (reminder == null) return;
            await _dataService.UpdateReminderAsync(reminder);
            await LoadDataAsync();
        }
    }
}