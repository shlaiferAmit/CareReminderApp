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
            // 1. בדיקת בטיחות - מניעת לחיצות כפולות
            if (IsBusy) return;

            // 2. הגנה במקרה שהמשתמש לא עבר כראוי ב-QueryProperty
            if (CurrentUser == null)
            {
                CurrentUser = App.LoggedInUser;
            }

            // 3. אם עדיין אין משתמש, אי אפשר להמשיך (חייבים מזהה שולח)
            if (CurrentUser == null)
            {
                await Shell.Current.DisplayAlert("שגיאה", "לא ניתן לזהות את המשתמש המחובר. נסה להתחבר מחדש.", "אוקיי");
                return;
            }

            // 4. פתיחת חלונית קלט לקבלת אימייל המבוגר
            string email = await Shell.Current.DisplayPromptAsync(
                "הוספת מבוגר",
                "הכניסי את כתובת האימייל של המבוגר שאליו תרצי להתחבר:",
                "הוספה",
                "ביטול"
                );

            // 5. בדיקה שהמשתמש לא לחץ ביטול או השאיר ריק
            if (string.IsNullOrWhiteSpace(email)) return;

            IsBusy = true;
            try
            {
                // 6. חיפוש המבוגר ב-Database (נרמול האימייל למניעת טעויות)
                var senior = await _dataService.FindSeniorByEmailAsync(email.Trim().ToLower());

                if (senior == null)
                {
                    await Shell.Current.DisplayAlert("לא נמצא", "לא נמצא משתמש מבוגר עם אימייל זה במערכת.", "הבנתי");
                    return;
                }

                // 7. בדיקה עצמית - שלא יוסיף את עצמו
                if (senior.Id == CurrentUser.Id)
                {
                    await Shell.Current.DisplayAlert("פעולה לא חוקית", "לא ניתן להוסיף את עצמך כמבוגר.", "אוקיי");
                    return;
                }

                // 8. שליחת בקשת החיבור ב-Firebase
                await _dataService.InviteElderAsync(CurrentUser.Id, senior.Id);

                await Shell.Current.DisplayAlert("נשלח בהצלחה", $"בקשת חיבור נשלחה אל {senior.FirstName}. עליו לאשר אותה באפליקציה שלו.", "מעולה");
            }
            catch (Exception ex)
            {
                // 9. טיפול בשגיאות תקשורת או Firebase
                await Shell.Current.DisplayAlert("שגיאה", $"אירעה שגיאה בתהליך: {ex.Message}", "אוקיי");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}