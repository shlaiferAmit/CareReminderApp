using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareReminderApp.Models;

using CareReminderApp.Services;

namespace CareReminderApp.ViewModels
{
    [QueryProperty(nameof(SelectedReminder), "SelectedReminder")]
    public partial class ReminderDetailsViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private Reminder? _selectedReminder;

        [ObservableProperty]
        private string _statusText = "Not Done";

        public ReminderDetailsViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        partial void OnSelectedReminderChanged(Reminder? value)
        {
            if (value != null)
            {
                StatusText = value.IsCompleted ? "Done" : "Not Done";
            }
        }

        // --- הפקודות החדשות לפי העיצוב החדש ---

        [RelayCommand]
        private async Task MarkAsDone()
        {
            if (SelectedReminder != null)
            {
                // קביעת הסטטוס ל"בוצע"
                SelectedReminder.IsCompleted = true;

                // עדכון ה-Service כדי שהשינוי יישמר
                await _dataService.UpdateReminderAsync(SelectedReminder);

                // עדכון התצוגה בדף הנוכחי
                StatusText = "Done";
            }
        }

        [RelayCommand]
        private async Task MarkAsNotDone()
        {
            if (SelectedReminder != null)
            {
                // קביעת הסטטוס ל"לא בוצע"
                SelectedReminder.IsCompleted = false;

                // עדכון ה-Service כדי שהשינוי יישמר
                await _dataService.UpdateReminderAsync(SelectedReminder);

                // עדכון התצוגה בדף הנוכחי
                StatusText = "Not Done";
            }
        }

        // --- הפקודות הקודמות (Delete ו-GoHome) נשארות ---

        [RelayCommand]
        private async Task Delete()
        {
            if (SelectedReminder != null)
            {
                // בעתיד נוסיף כאן: await _dataService.DeleteReminderAsync(SelectedReminder);
                await Shell.Current.GoToAsync(".."); // חזרה אחורה אחרי המחיקה
            }
        }

        [RelayCommand]
        private async Task GoHome()
        {
            // חזרה לדף הראשי של המבוגר (וודאי שהנתיב תואם ל-AppShell)
            await Shell.Current.GoToAsync("///TodayRemindersPage");
        }
    }
}