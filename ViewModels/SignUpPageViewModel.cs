using CareReminderApp.Models;
using CareReminderApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CareReminderApp.Views;


namespace CareReminderApp.ViewModels
{
    public partial class SignUpPageViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly AuthService _authService; // שירות האימות החדש

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private string _firstName = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private string _lastName = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private string _userEmail = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private string _userPassword = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private string _mobile = string.Empty;

        [ObservableProperty]
        private bool _entryAsPassword = true;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private UserRole _selectedRole;

        [ObservableProperty]
        private bool isBusy;

        public List<UserRole> RoleOptions { get; } = new List<UserRole>
        {
            UserRole.Senior,
            UserRole.FamilyMember
        };

        // הוספנו את AuthService לבנאי
        public SignUpPageViewModel(IDataService dataService, AuthService authService)
        {
            _dataService = dataService;
            _authService = authService;
        }

        public string PasswordImage => EntryAsPassword ? "closeeye.png" : "openeye.png";

        partial void OnEntryAsPasswordChanged(bool value) => OnPropertyChanged(nameof(PasswordImage));

        [RelayCommand]
        private void TogglePassword() => EntryAsPassword = !EntryAsPassword;

        [RelayCommand]
        private async Task GoToSignIn()
        {
            await Shell.Current.GoToAsync("///SignInPage");
        }

        [RelayCommand(CanExecute = nameof(CanSignUp))]
        private async Task SignUp()
        {
            try
            {
                IsBusy = true;

                // 1. הרשמה מול Firebase
                var authResult = await _authService.SignUpAsync(UserEmail, UserPassword);

                if (authResult != null)
                {
                    // 2. שמירת פרטי המשתמש ב-Database שלנו
                    bool dbSuccess = await _dataService.RegisterUserAsync(
                        FirstName, LastName, UserEmail, UserPassword, Mobile, SelectedRole);

                    if (dbSuccess)
                    {
                        // 3. שליפת המשתמש המלא (כולל ה-ID וה-Role)
                        var user = await _dataService.GetUserAsync(UserEmail, UserPassword);

                        if (user != null)
                        {
                            // 4. הפעלת ה-AppShell לעדכון הטאבים וניווט לדף הבית
                            if (Shell.Current is AppShell appShell)
                            {
                                appShell.SetLoggedInState(true, user);
                            }
                            else
                            {
                                // גיבוי למקרה שה-Shell לא זמין
                                App.LoggedInUser = user;
                                await Shell.Current.GoToAsync("//MainPage");
                            }
                        }
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Registration failed in database", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanSignUp() =>
            !string.IsNullOrWhiteSpace(FirstName) &&
            !string.IsNullOrWhiteSpace(LastName) &&
            !string.IsNullOrWhiteSpace(UserEmail) &&
            !string.IsNullOrWhiteSpace(UserPassword) &&
            UserPassword.Length >= 6 && // Firebase דורש לפחות 6 תווים
            !string.IsNullOrWhiteSpace(Mobile);
    }
}