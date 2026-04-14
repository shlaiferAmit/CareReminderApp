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
            try
            {
                // בדיקה שהנתונים לא ריקים
                if (string.IsNullOrWhiteSpace(ReminderTitle) || SelectedElder == null)
                {
                    await Shell.Current.DisplayAlert("חסרים פרטים", "נא למלא כותרת ולבחור מבוגר", "אוקיי");
                    return;
                }

                var newReminder = new Reminder
                {
                    Title = ReminderTitle,
                    Description = Notes,
                    DueDate = SelectedDate,
                    UserId = SelectedElder.Id,
                    IsCompleted = false
                };

                // זה ידפיס לך הודעה ב-Visual Studio ברגע הלחיצה!
                System.Diagnostics.Debug.WriteLine("נסיונית לשמור תזכורת ל-Firebase...");

                await _dataService.SaveReminderAsync(newReminder);

                await Shell.Current.DisplayAlert("הצלחה", "התזכורת נשמרה בהצלחה!", "מעולה");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"שגיאה בשמירה: {ex.Message}");
                await Shell.Current.DisplayAlert("שגיאה", "השמירה נכשלה: " + ex.Message, "אוקיי");
            }
        }
    }
}