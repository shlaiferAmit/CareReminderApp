using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CareReminderApp.Models;
using CareReminderApp.Services;

namespace CareReminderApp.ViewModels
{
    [QueryProperty(nameof(SelectedReminder), "SelectedReminder")]
    // מחלקת מודל תצוגה עבור מסך פרטי תזכורת
    // אחראית על הצגת פרטי תזכורת, שינוי סטטוס (בוצע / לא בוצע) ומחיקה מהמערכת
    public partial class ReminderDetailsViewModel : ObservableObject
    {
        // שירות הנתונים של האפליקציה (פיירבייס או שירות מדומה)
        private readonly IDataService _dataService;

        // התזכורת שנבחרה להצגה
        [ObservableProperty]
        private Reminder? selectedReminder;

        // טקסט המציג את סטטוס התזכורת למשתמש
        [ObservableProperty]
        private string statusText = "Not Done";

        public ReminderDetailsViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        // מופעל אוטומטית כאשר התזכורת הנבחרת משתנה
        partial void OnSelectedReminderChanged(Reminder? value)
        {
            if (value != null)
            {
                UpdateStatusText(value.IsCompleted);
            }
        }

        // עדכון טקסט הסטטוס בהתאם אם התזכורת בוצעה או לא
        private void UpdateStatusText(bool isCompleted)
        {
            StatusText = isCompleted ? "Done" : "Not Done";
        }

        // סימון תזכורת כבוצעה
        [RelayCommand]
        public async Task MarkAsDone()
        {
            if (SelectedReminder == null) return;

            SelectedReminder.IsCompleted = true;
            await _dataService.UpdateReminderAsync(SelectedReminder);
            UpdateStatusText(true);
            await Shell.Current.DisplayAlert("Status", "Reminder marked as done!", "OK");
        }

        // סימון תזכורת כלא בוצעה
        [RelayCommand]
        public async Task MarkAsNotDone()
        {
            if (SelectedReminder == null) return;

            SelectedReminder.IsCompleted = false;
            await _dataService.UpdateReminderAsync(SelectedReminder);
            UpdateStatusText(false);
            await Shell.Current.DisplayAlert("Status", "Reminder marked as not done", "OK");
        }

        // מחיקת תזכורת מהמערכת
        [RelayCommand]
        public async Task Delete()
        {
            if (SelectedReminder == null || string.IsNullOrEmpty(SelectedReminder.Id))
            {
                await Shell.Current.DisplayAlert("Error", "Cannot find reminder ID", "OK");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert("Delete", "Are you sure you want to delete this reminder?", "Yes", "No");

            if (confirm)
            {
                var success = await _dataService.DeleteReminderAsync(SelectedReminder.Id);

                if (success)
                {
                    await Shell.Current.GoToAsync("..");    // חזרה למסך הקודם לאחר מחיקה מוצלחת

                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to delete from server", "OK");
                }
            }
        }
    }
}