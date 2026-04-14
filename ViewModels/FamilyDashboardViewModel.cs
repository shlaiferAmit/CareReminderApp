using CareReminderApp.Models; // הוספתי את זה
using CareReminderApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareReminderApp.ViewModels
{
    [QueryProperty(nameof(CurrentUser), "CurrentUser")]
    public partial class FamilyDashboardViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private User? currentUser;

        [ObservableProperty]
        private string welcomeMessage = "Hello!";

        [ObservableProperty]
        private bool isBusy;

        public FamilyDashboardViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        partial void OnCurrentUserChanged(User? value)
        {
            if (value != null)
                WelcomeMessage = $"Good morning, {value.FirstName}!";
        }

        [RelayCommand]
        private async Task AddSenior()
        {
            if (IsBusy || CurrentUser == null) return;

            string email = await Shell.Current.DisplayPromptAsync("הוספת מבוגר", "הכנס אימייל של המבוגר:");
            if (string.IsNullOrWhiteSpace(email)) return;

            IsBusy = true;
            try
            {
                var senior = await _dataService.FindSeniorByEmailAsync(email.Trim().ToLower());
                if (senior == null)
                {
                    await Shell.Current.DisplayAlert("שגיאה", "לא נמצא משתמש מבוגר עם אימייל זה", "OK");
                    return;
                }

                await _dataService.InviteElderAsync(CurrentUser.Id, senior.Id);
                await Shell.Current.DisplayAlert("נשלח", "בקשת חיבור נשלחה למבוגר.", "OK");
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("שגיאה", "אירעה שגיאה", "OK");
            }
            finally { IsBusy = false; }
        }
    }
}