using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CareReminderApp.Models;
using CareReminderApp.Views;
using System.Collections.ObjectModel;

namespace CareReminderApp.ViewModels
{
    public partial class TodayRemindersViewModel : ObservableObject
    {
        // עדכנתי את השם ל-"Amit" כפי שמופיע בטלפון שלך
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WelcomeGreeting))] // מוודא שהברכה תתעדכן כשהשם משתנה
        private string userFirstName = "Amit";

        [ObservableProperty]
        private int totalRemindersCount = 3;

        [ObservableProperty]
        private ObservableCollection<Reminder> reminders = new();

        [ObservableProperty]
        private Reminder upcomingReminder = new Reminder { Title = "Take medication", Time = DateTime.Today.AddHours(8) };

        // המאפיין שיוצג בשני הדפים (מבוגר ומשפחה)
        public string WelcomeGreeting
        {
            get
            {
                var hour = DateTime.Now.Hour;
                string greeting;

                if (hour >= 5 && hour < 12)
                    greeting = "Good Morning";
                else if (hour >= 12 && hour < 18)
                    greeting = "Good Afternoon";
                else if (hour >= 18 && hour < 22)
                    greeting = "Good Evening";
                else
                    greeting = "Good Night";

                return $"{greeting}, {UserFirstName}";
            }
        }

        public string RemindersSummary => $"You have {TotalRemindersCount} reminds today";

        [RelayCommand]
        public async Task NavigateToSeniors()
        {
            await Shell.Current.GoToAsync("EldersListPage");
        }

        [RelayCommand]
        public async Task UpdateReminderStatusAsync(Reminder reminder)
        {
            if (reminder == null) return;
            await Task.CompletedTask;
        }

        [RelayCommand]
        public async Task NavigateToTodayReminders() => await Shell.Current.GoToAsync(nameof(TodayRemindersPage));

        [RelayCommand]
        public async Task NavigateToProfile() => await Shell.Current.GoToAsync(nameof(ProfilePage));
    }
}