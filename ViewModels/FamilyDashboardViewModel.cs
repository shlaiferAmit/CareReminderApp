using CareReminderApp.Models;
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

            // גיבוי: אם CurrentUser לא הגיע דרך הניווט, ננסה לקחת אותו מהסטייט הגלובלי
            if (CurrentUser == null && App.LoggedInUser != null)
            {
                CurrentUser = App.LoggedInUser;
            }
        }

        partial void OnCurrentUserChanged(User? value)
        {
            if (value != null)
                WelcomeMessage = $"Good morning, {value.FirstName}!";
        }

        [RelayCommand]
        private async Task AddSenior()
        {
            if (IsBusy) return;

            // בדיקת אבטחה למשתמש המחובר
            var activeUser = CurrentUser ?? App.LoggedInUser;

            if (activeUser == null)
            {
                await Shell.Current.DisplayAlert("שגיאה", "לא ניתן לזהות את המשתמש המחובר. נסה להתחבר מחדש.", "אוקיי");
                return;
            }

            string emailInput = await Shell.Current.DisplayPromptAsync(
                "הוספת מבוגר",
                "הכניסי את כתובת האימייל של המבוגר שאליו תרצי להתחבר:",
                "הוספה",
                "ביטול"
                );

            if (string.IsNullOrWhiteSpace(emailInput)) return;

            IsBusy = true;
            try
            {
                // הבאת כל המשתמשים וסינון לפי אימייל ותפקיד "Senior"
                var allUsers = await _dataService.GetUsersAsync();
                var emailToFind = emailInput.Trim().ToLower();

                var senior = allUsers.FirstOrDefault(u =>
                    u.UserEmail?.ToLower() == emailToFind &&
                    u.Role == UserRole.Senior);

                if (senior == null)
                {
                    await Shell.Current.DisplayAlert("לא נמצא", "לא נמצא משתמש מבוגר עם אימייל זה במערכת.", "הבנתי");
                    return;
                }

                // מניעת חיבור של משתמש לעצמו
                if (senior.Id == activeUser.Id)
                {
                    await Shell.Current.DisplayAlert("פעולה לא חוקית", "לא ניתן להוסיף את עצמך כמבוגר.", "אוקיי");
                    return;
                }

                // שליחת בקשת חיבור ל-Firebase
                await _dataService.InviteElderAsync(activeUser.Id, senior.Id);

                await Shell.Current.DisplayAlert("נשלח בהצלחה", $"בקשת חיבור נשלחה אל {senior.FirstName}. עליו לאשר אותה באפליקציה שלו.", "מעולה");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("שגיאה", $"אירעה שגיאה בתהליך: {ex.Message}", "אוקיי");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}