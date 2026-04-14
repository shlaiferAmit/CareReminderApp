using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CareReminderApp.Models;
using CareReminderApp.Services;

namespace CareReminderApp.ViewModels
{
    [QueryProperty(nameof(SelectedReminder), "SelectedReminder")]
    public partial class ReminderDetailsViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private Reminder? selectedReminder;

        [ObservableProperty]
        private string statusText = "Not Done";

        public ReminderDetailsViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        partial void OnSelectedReminderChanged(Reminder? value)
        {
            if (value != null)
            {
                UpdateStatusText(value.IsCompleted);
            }
        }

        private void UpdateStatusText(bool isCompleted)
        {
            StatusText = isCompleted ? "Done" : "Not Done";
        }

        [RelayCommand]
        public async Task MarkAsDone()
        {
            if (SelectedReminder == null) return;

            SelectedReminder.IsCompleted = true;
            await _dataService.UpdateReminderAsync(SelectedReminder);
            UpdateStatusText(true);
            await Shell.Current.DisplayAlert("Status", "Reminder marked as done!", "OK");
        }

        [RelayCommand]
        public async Task MarkAsNotDone()
        {
            if (SelectedReminder == null) return;

            SelectedReminder.IsCompleted = false;
            await _dataService.UpdateReminderAsync(SelectedReminder);
            UpdateStatusText(false);
            await Shell.Current.DisplayAlert("Status", "Reminder marked as not done", "OK");
        }

        [RelayCommand]
        public async Task Delete()
        {
            if (SelectedReminder == null) return;

            bool confirm = await Shell.Current.DisplayAlert("Delete", "Are you sure you want to delete this reminder permanently?", "Yes", "No");
            if (confirm)
            {
                // קריאה לפונקציית המחיקה האמיתית ב-Service
                // הערה: ודאי שב-IDataService קיימת מתודה DeleteReminderAsync(string id)
                var success = await _dataService.DeleteReminderAsync(SelectedReminder.Id);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Deleted", "Reminder removed successfully", "OK");
                    await Shell.Current.GoToAsync(".."); // חזרה לדף הרשימה
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Could not delete from Firebase", "OK");
                }
            }
        }
    }
}