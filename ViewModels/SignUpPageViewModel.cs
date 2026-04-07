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

        // הוספת בחירת תפקיד - חשוב מאוד לניתוב
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private UserRole _selectedRole;

        // רשימה שתציג את האופציות ב-Picker ב-XAML
        public List<UserRole> RoleOptions { get; } = new List<UserRole>
        {
            UserRole.Senior,
            UserRole.FamilyMember
        };

        public SignUpPageViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public string PasswordImage => EntryAsPassword ? "closeeye.png" : "openeye.png";

        partial void OnEntryAsPasswordChanged(bool value) => OnPropertyChanged(nameof(PasswordImage));

        [RelayCommand]
        private void TogglePassword() => EntryAsPassword = !EntryAsPassword;

        [RelayCommand]
        private async Task GoToSignIn()
        {
            await Shell.Current.GoToAsync("SignInPage");
        }

        [RelayCommand(CanExecute = nameof(CanSignUp))]
        private async Task SignUp()
        {
            // 1. הרשמה ב-Service
            bool success = await _dataService.RegisterUserAsync(FirstName, LastName, UserEmail, UserPassword, Mobile, SelectedRole);

            if (success)
            {
                // 2. יצירת אובייקט המשתמש החדש כדי לעדכן את ה-Shell
                var newUser = new User
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    UserEmail = UserEmail,
                    Role = SelectedRole
                };

                App.LoggedInUser = newUser;

                // 3. עדכון ה-Shell (בניית טאבים וניווט אוטומטי)
                if (Shell.Current is AppShell appShell)
                {
                    appShell.SetLoggedInState(true, newUser);
                }

                // הסרנו את ה-GoToAsync המקורי שגרם לשגיאה!
            }
            else
            {
                if (App.Current.MainPage != null)
                {
                    await App.Current.MainPage.DisplayAlert("שגיאה", "ההרשמה נכשלה. ייתכן שהאימייל כבר קיים במערכת.", "אישור");
                }
            }
        }

        private bool CanSignUp() =>
            !string.IsNullOrWhiteSpace(FirstName) &&
            !string.IsNullOrWhiteSpace(LastName) &&
            !string.IsNullOrWhiteSpace(UserEmail) &&
            !string.IsNullOrWhiteSpace(UserPassword) &&
            !string.IsNullOrWhiteSpace(Mobile);
    }
}