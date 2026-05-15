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
    public partial class SignUpPageViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly AuthService _authService;

        [ObservableProperty]
        private string _firstName = string.Empty;

        [ObservableProperty]
        private string _lastName = string.Empty;

        [ObservableProperty]
        private string _userEmail = string.Empty;

        [ObservableProperty]
        private string _userPassword = string.Empty;

        [ObservableProperty]
        private string _mobile = string.Empty;

        [ObservableProperty]
        private bool _entryAsPassword = true;

        [ObservableProperty]
        private UserRole _selectedRole;

        [ObservableProperty]
        private bool isBusy;

        // משתני שגיאה
        [ObservableProperty]
        private string _emailError = string.Empty;

        [ObservableProperty]
        private string _passwordError = string.Empty;

        [ObservableProperty]
        private string _mobileError = string.Empty;

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

        // --- לוגיקת בדיקת תקינות (Validation) ---
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

        // --- עדכון מצב הכפתור והשגיאות בכל שינוי ---
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

        public string PasswordImage => EntryAsPassword ? "closeeye.png" : "openeye.png";
        partial void OnEntryAsPasswordChanged(bool value) => OnPropertyChanged(nameof(PasswordImage));

        [RelayCommand]
        private void TogglePassword() => EntryAsPassword = !EntryAsPassword;

        [RelayCommand]
        private async Task GoToSignIn() => await Shell.Current.GoToAsync("//SignInPage");

        // הכפתור יהיה כתום/פעיל רק כשאין שגיאות וכל השדות מלאים
        private bool CanSignUp() =>
            !IsBusy &&
            !string.IsNullOrWhiteSpace(FirstName) &&
            !string.IsNullOrWhiteSpace(LastName) &&
            string.IsNullOrEmpty(EmailError) && !string.IsNullOrWhiteSpace(UserEmail) &&
            string.IsNullOrEmpty(PasswordError) && !string.IsNullOrWhiteSpace(UserPassword) &&
            string.IsNullOrEmpty(MobileError) && !string.IsNullOrWhiteSpace(Mobile);

        [RelayCommand(CanExecute = nameof(CanSignUp))]
        private async Task SignUp()
        {
            try
            {
                IsBusy = true;
                var authResult = await _authService.SignUpAsync(UserEmail, UserPassword);

                if (authResult?.User != null)
                {
                    string firebaseUid = authResult.User.Uid;
                    bool dbSuccess = await _dataService.RegisterUserAsync(
                        firebaseUid, FirstName, LastName, UserEmail, UserPassword, Mobile, SelectedRole);

                    if (dbSuccess)
                    {
                        var user = await _dataService.GetUserAsync(UserEmail, UserPassword);
                        if (user != null && Shell.Current is AppShell appShell)
                            appShell.SetLoggedInState(true, user);
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally { IsBusy = false; }
        }
    }
}