using CareReminderApp.Models;
using CareReminderApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CareReminderApp.Views;

namespace CareReminderApp.ViewModels
{
    public partial class ProfileViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private User _displayUser;

        [ObservableProperty]
        private string _profileTitle;

        [ObservableProperty]
        private bool _isViewingElder;

        [ObservableProperty]
        private bool _isMyPersonalProfile;

        [ObservableProperty]
        private ObservableCollection<Reminder> _reminders;

        public ProfileViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Reminders = new ObservableCollection<Reminder>();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // כאן אנחנו בודקים אם הגענו מהמבוגרים או מהפרופיל האישי
            if (query.TryGetValue("SelectedUser", out var selected) && selected is User elder)
            {
                DisplayUser = elder;
                ProfileTitle = $"{elder.FirstName}'s Profile";
                IsViewingElder = true;
                IsMyPersonalProfile = false;
                await LoadRemindersAsync();
            }
            else
            {
                DisplayUser = App.LoggedInUser;
                ProfileTitle = "My Profile";
                IsViewingElder = false;
                IsMyPersonalProfile = true;
                Reminders.Clear();
            }
        }

        private async Task LoadRemindersAsync()
        {
            if (DisplayUser == null) return;
            var list = await _dataService.GetRemindersByUserIdAsync(DisplayUser.Id);
            Reminders = new ObservableCollection<Reminder>(list);
        }

        [RelayCommand]
        private async Task EditProfile()
        {
            // שליחת המשתמש הנכון לדף העריכה
            var navParam = new Dictionary<string, object> { { "DisplayUser", DisplayUser } };
            await Shell.Current.GoToAsync(nameof(ChangeProfilePage), navParam);
        }

        [RelayCommand]
        private async Task AddReminder()
        {
            if (DisplayUser == null) return;

            // שליחת המבוגר הנבחר לדף הוספת התזכורת
            var navParam = new Dictionary<string, object> { { "SelectedElder", DisplayUser } };
            await Shell.Current.GoToAsync("AddReminderPage", navParam);
        }

        [RelayCommand]
        private async Task SaveChanges() => await Shell.Current.GoToAsync("..");


        [RelayCommand]
        async Task Logout()
        {
            await Shell.Current.GoToAsync("//SignInPage");
        }

        [RelayCommand]
        async Task GoToHome()
        {
            // משתמש במערכת הניווט של Shell כדי לחזור אחורה או לדף הראשי
            await Shell.Current.GoToAsync("..");
        }
    }



}