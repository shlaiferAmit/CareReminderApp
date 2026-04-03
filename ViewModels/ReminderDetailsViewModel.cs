using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareReminderApp.Models;
using CareReminderApp.Services;

namespace CareReminderApp.ViewModels
{
    [QueryProperty(nameof(SelectedReminder), "SelectedReminder")]
    public partial class ReminderDetailsViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        // הוסר הקו התחתון כדי למנוע שגיאות Generator
        [ObservableProperty]
        private Reminder? selectedReminder;

        [ObservableProperty]
        private string statusText = "Not Done";

        public ReminderDetailsViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        // פונקציה זו רצה אוטומטית כשהתזכורת מתקבלת מהדף הקודם
        partial void OnSelectedReminderChanged(Reminder? value)
        {
            if (value != null)
            {
                StatusText = value.IsCompleted ? "Done" : "Not Done";
            }
        }

        [RelayCommand]
        public async Task MarkAsDone()
        {
            if (SelectedReminder != null)
            {
                SelectedReminder.IsCompleted = true;
                await _dataService.UpdateReminderAsync(SelectedReminder);
                StatusText = "Done"; // מעדכן את הבועה הירוקה בעיצוב
            }
        }

        [RelayCommand]
        public async Task MarkAsNotDone()
        {
            if (SelectedReminder != null)
            {
                SelectedReminder.IsCompleted = false;
                await _dataService.UpdateReminderAsync(SelectedReminder);
                StatusText = "Not Done";
            }
        }

        [RelayCommand]
        public async Task Delete()
        {
            if (SelectedReminder != null)
            {
                // כאן תבוא לוגיקת המחיקה מה-Service בעתיד
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        public async Task GoHome()
        {
            // ודאי שהנתיב "TodayRemindersPage" רשום ב-AppShell
            await Shell.Current.GoToAsync("///TodayRemindersPage");
        }
    }
}