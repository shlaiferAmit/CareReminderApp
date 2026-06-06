using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CareReminderApp.Services;
using CareReminderApp.Models;
using System;
using System.Threading.Tasks;

namespace CareReminderApp.ViewModels
// מחלקת מודל תצוגה להוספת תזכורת חדשה למערכת
// אחראית על קבלת נתונים מהמשתמש ושמירת תזכורת דרך שכבת השירותים
{
    [QueryProperty(nameof(SelectedElder), "SelectedElder")]
    public partial class AddReminderViewModel : ObservableObject
    {
        // שירות הנתונים של האפליקציה - פיירבייס או מדמה
        private readonly IDataService _dataService;

        // המשתמש (קשיש) שאליו משויכת התזכורת
        [ObservableProperty]
        private User _selectedElder;

        // כותרת התזכורת
        [ObservableProperty]
        private string _reminderTitle = string.Empty;

        // הערות נוספות לתזכורת
        [ObservableProperty]
        private string _notes = string.Empty;

        // תאריך שנבחר לתזכורת
        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Now;

        // שעה שנבחרה לתזכורת
        [ObservableProperty]
        private TimeSpan _selectedTime = DateTime.Now.TimeOfDay;

        public AddReminderViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        // פונקציה לשמירה ושליחה של תזכורת חדשה
        [RelayCommand]
        private async Task SaveAndSendReminder()
        {
            try
            {
                // בדיקה שהוזנו כותרת ותבחר משתמש יעד
                if (string.IsNullOrWhiteSpace(ReminderTitle) || SelectedElder == null)
                {
                    // 👈 שונה לאנגלית
                    await Shell.Current.DisplayAlert("Missing Details", "Please enter a title and select a senior.", "OK");
                    return;
                }

                // חיבור בין תאריך ושעה ליצירת זמן יעד מלא
                DateTime finalDueDate = SelectedDate.Date + SelectedTime;

                // בדיקה שהתאריך שנבחר לא עבר כבר
                if (finalDueDate < DateTime.Now)
                {
                    // 👈 שונה לאנגלית
                    await Shell.Current.DisplayAlert("Invalid Time", "Cannot set a reminder for a past date or time.", "OK");
                    return;
                }

                // יצירת אובייקט תזכורת חדש
                var newReminder = new Reminder
                {
                    Id = Guid.NewGuid().ToString(), // הוספת מזהה ייחודי
                    Title = ReminderTitle,
                    Description = Notes,
                    DueDate = finalDueDate, // הזמן המשולב
                    UserId = SelectedElder.Id,
                    IsCompleted = false
                };

                // הדפסת מידע לצורך בדיקה
                System.Diagnostics.Debug.WriteLine($"שומר תזכורת לתאריך: {newReminder.DueDate}");

                // שמירת התזכורת במסד הנתונים
                await _dataService.SaveReminderAsync(newReminder);

                // הודעת הצלחה וחזרה למסך הקודם
                // 👈 שונה לאנגלית
                await Shell.Current.DisplayAlert("Success", "The reminder has been saved and synchronized successfully!", "Awesome");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                // טיפול בשגיאות בזמן שמירת התזכורת
                System.Diagnostics.Debug.WriteLine($"Save Error: {ex.Message}");             
                await Shell.Current.DisplayAlert("Error", "Failed to save the reminder: " + ex.Message, "OK");
            }
        }
    }
}