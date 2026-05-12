using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CareReminderApp.Services;
using CareReminderApp.Models;
using System;
using System.Threading.Tasks;

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

        [ObservableProperty]
        private TimeSpan _selectedTime = DateTime.Now.TimeOfDay; // שדה חדש לבחירת שעה

        public AddReminderViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        [RelayCommand]
        private async Task SaveAndSendReminder()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ReminderTitle) || SelectedElder == null)
                {
                    await Shell.Current.DisplayAlert("חסרים פרטים", "נא למלא כותרת ולבחור מבוגר", "אוקיי");
                    return;
                }

                // חיבור התאריך והשעה לאובייקט DateTime אחד
                DateTime finalDueDate = SelectedDate.Date + SelectedTime;

                // בדיקה שהזמן לא עבר כבר
                if (finalDueDate < DateTime.Now)
                {
                    await Shell.Current.DisplayAlert("זמן לא תקין", "לא ניתן לקבוע תזכורת לזמן שעבר", "אוקיי");
                    return;
                }

                var newReminder = new Reminder
                {
                    Id = Guid.NewGuid().ToString(), // הוספת מזהה ייחודי
                    Title = ReminderTitle,
                    Description = Notes,
                    DueDate = finalDueDate, // הזמן המשולב
                    UserId = SelectedElder.Id,
                    IsCompleted = false
                };

                System.Diagnostics.Debug.WriteLine($"שומר תזכורת לתאריך: {newReminder.DueDate}");

                await _dataService.SaveReminderAsync(newReminder);

                await Shell.Current.DisplayAlert("הצלחה", "התזכורת נשמרה וסונכרנה!", "מעולה");
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