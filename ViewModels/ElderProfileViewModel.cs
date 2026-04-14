using CareReminderApp.Models;
using CareReminderApp.Services;
using CareReminderApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CareReminderApp.ViewModels
{
    // המפתח שמתקבל מהדף הקודם
    [QueryProperty(nameof(Elder), "Elder")]
    public partial class ElderProfileViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private string title = "פרופיל מבוגר";

        [ObservableProperty]
        private User elder; // זה האובייקט שמכיל את FirstName, LastName וכו'

        [ObservableProperty]
        private ObservableCollection<Reminder> reminders;

        public ElderProfileViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Reminders = new ObservableCollection<Reminder>();
        }

        // פונקציה שרצה אוטומטית כשהמבוגר נטען
        partial void OnElderChanged(User value)
        {
            if (value != null)
            {
                Title = $"פרופיל של {value.FirstName}";
                LoadRemindersCommand.Execute(null);
            }
        }

        [RelayCommand]
        public async Task LoadReminders()
        {
            if (Elder == null) return;
            try
            {
                var result = await _dataService.GetRemindersAsync(Elder.Id);
                Reminders.Clear();
                foreach (var r in result)
                    Reminders.Add(r);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading reminders: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task AddReminder()
        {
            if (Elder == null) return;

            // כאן אנחנו שולחים את המבוגר לדף הבא תחת המפתח ש-AddReminderViewModel מצפה לו
            await Shell.Current.GoToAsync(nameof(AddReminderPage), new Dictionary<string, object>
            {
                { "SelectedElder", Elder }
            });
        }
    }
}