using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CareReminderApp.Services;
using CareReminderApp.Models;

namespace CareReminderApp.ViewModels
{
    [QueryProperty(nameof(SelectedElder), "SelectedElder")]
    public partial class AddReminderViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        // המשתנים האלו ייצרו אוטומטית את ReminderTitle (בלי קו תחתון) עבור ה-XAML
        [ObservableProperty]
        private User _selectedElder;

        [ObservableProperty]
        private string _reminderTitle = string.Empty;

        [ObservableProperty]
        private string _notes = string.Empty;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Now;

        public AddReminderViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        [RelayCommand]
        private async Task SaveAndSendReminder()
        {
            // בדיקה שהשדות לא ריקים
            if (string.IsNullOrWhiteSpace(ReminderTitle) || SelectedElder == null) return;

            var newReminder = new Reminder
            {
                Title = ReminderTitle,
                Description = Notes,
                DueDate = SelectedDate,
                Time = SelectedDate,
                UserId = SelectedElder.Id,
                IsCompleted = false
            };

            await _dataService.AddReminderAsync(newReminder);
            await Shell.Current.GoToAsync("..");
        }
    }
}