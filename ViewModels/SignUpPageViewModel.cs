using CareReminderApp.Models;
using CareReminderApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CareReminderApp.ViewModels
{
    // מחלקת מודל תצוגה עבור מסך הרשמה
    // אחראית על יצירת משתמש חדש, בדיקות תקינות של שדות, והתחברות לאחר הרשמה
    public partial class SignUpPageViewModel : ObservableObject
    {
        // שירות הנתונים של האפליקציה (פיירבייס או שירות מדומה)
        private readonly IDataService _dataService;

        // שירות אימות משתמשים מול פיירבייס
        private readonly AuthService _authService;

        // שם פרטי של המשתמש
        [ObservableProperty]
        private string _firstName = string.Empty;

        // שם משפחה של המשתמש
        [ObservableProperty]
        private string _lastName = string.Empty;

        // אימייל של המשתמש
        [ObservableProperty]
        private string _userEmail = string.Empty;

        // סיסמה של המשתמש
        [ObservableProperty]
        private string _userPassword = string.Empty;

        // מספר טלפון של המשתמש
        [ObservableProperty]
        private string _mobile = string.Empty;

        // האם להציג סיסמה מוסתרת
        [ObservableProperty]
        private bool _entryAsPassword = true;

        // תפקיד המשתמש (קשיש או בן משפחה)
        [ObservableProperty]
        private UserRole _selectedRole;

        // מצב טעינה בזמן הרשמה
        [ObservableProperty]
        private bool isBusy;

        // הודעות שגיאה עבור שדות הטופס
        [ObservableProperty]
        private string _emailError = string.Empty;

        [ObservableProperty]
        private string _passwordError = string.Empty;

        [ObservableProperty]
        private string _mobileError = string.Empty;

        // אפשרויות בחירת תפקיד
        public List<UserRole> RoleOptions { get; } = new List<UserRole>
        {
            UserRole.Senior,
            UserRole.FamilyMember
        };

        public SignUpPageViewModel(IDataService dataService, AuthService authService)
        {
            _dataService = dataService;
            _authService = authService;
        }

        // בדיקת תקינות של כל השדות בטופס
        private void Validate()
        {
            // בדיקת אימייל
            if (string.IsNullOrWhiteSpace(UserEmail))
                EmailError = "";
            else if (!Regex.IsMatch(UserEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                EmailError = "Invalid email format";
            else
                EmailError = "";

            // בדיקת סיסמה
            if (string.IsNullOrWhiteSpace(UserPassword))
                PasswordError = "";
            else if (UserPassword.Length < 6)
                PasswordError = "Must be at least 6 characters";
            else
                PasswordError = "";

            // בדיקת טלפון - רק מספרים ו-10 ספרות
            if (string.IsNullOrWhiteSpace(Mobile))
                MobileError = "";
            else if (!Regex.IsMatch(Mobile, @"^\d+$"))
                MobileError = "Numbers only";
            else if (Mobile.Length != 10)
                MobileError = "Must be exactly 10 digits";
            else
                MobileError = "";
        }

        // עדכון מצב כפתור ושגיאות בעת שינוי שדות
        partial void OnFirstNameChanged(string value) => SignUpCommand.NotifyCanExecuteChanged();
        partial void OnLastNameChanged(string value) => SignUpCommand.NotifyCanExecuteChanged();

        partial void OnUserEmailChanged(string value)
        {
            Validate();
            SignUpCommand.NotifyCanExecuteChanged();
        }

        partial void OnUserPasswordChanged(string value)
        {
            Validate();
            SignUpCommand.NotifyCanExecuteChanged();
        }

        partial void OnMobileChanged(string value)
        {
            Validate();
            SignUpCommand.NotifyCanExecuteChanged();
        }

        // אייקון סיסמה לפי מצב הצגה
        public string PasswordImage => EntryAsPassword ? "closeeye.png" : "openeye.png";

        partial void OnEntryAsPasswordChanged(bool value) => OnPropertyChanged(nameof(PasswordImage));

        // החלפת מצב הצגת סיסמה
        [RelayCommand]
        private void TogglePassword() => EntryAsPassword = !EntryAsPassword;

        // מעבר למסך התחברות

        [RelayCommand]
        private async Task GoToSignIn() => await Shell.Current.GoToAsync("//SignInPage");

        // בדיקה האם ניתן לבצע הרשמה
        private bool CanSignUp() =>
            !IsBusy &&
            !string.IsNullOrWhiteSpace(FirstName) &&
            !string.IsNullOrWhiteSpace(LastName) &&
            string.IsNullOrEmpty(EmailError) && !string.IsNullOrWhiteSpace(UserEmail) &&
            string.IsNullOrEmpty(PasswordError) && !string.IsNullOrWhiteSpace(UserPassword) &&
            string.IsNullOrEmpty(MobileError) && !string.IsNullOrWhiteSpace(Mobile);

        // פעולת הרשמה למערכת
        [RelayCommand(CanExecute = nameof(CanSignUp))]
        private async Task SignUp()
        {
            try
            {
                IsBusy = true;

                // יצירת משתמש בפיירבייס
                var authResult = await _authService.SignUpAsync(UserEmail, UserPassword);

                if (authResult?.User != null)
                {
                    string firebaseUid = authResult.User.Uid;

                    // שמירת המשתמש במסד הנתונים
                    bool dbSuccess = await _dataService.RegisterUserAsync(
                        firebaseUid, FirstName, LastName, UserEmail, UserPassword, Mobile, SelectedRole);

                    if (dbSuccess)
                    {
                        var user = await _dataService.GetUserAsync(UserEmail, UserPassword);
                        if (user != null && Shell.Current is AppShell appShell)
                            appShell.SetLoggedInState(true, user);
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Registration Error", "Failed to save user data. Please try again.", "OK");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Registration Error", "Sign up failed. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;

                if (errorMessage.Contains("EMAIL_EXISTS") || errorMessage.Contains("email already in use"))
                {
                    await Shell.Current.DisplayAlert("Error", "This email address is already registered.", "OK");
                }
                else if (errorMessage.Contains("WEAK_PASSWORD") || errorMessage.Contains("password should be at least"))
                {
                    await Shell.Current.DisplayAlert("Error", "The password is too weak. Please use at least 6 characters.", "OK");
                }
                else if (errorMessage.Contains("INVALID_EMAIL"))
                {
                    await Shell.Current.DisplayAlert("Error", "The email address is badly formatted.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Connection error. Please check your internet and try again.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}