using CareReminderApp.Models;
using CareReminderApp.Services;
using CareReminderApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using System.Threading.Tasks;

namespace CareReminderApp.ViewModels
{
    public partial class SignInPageViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly AuthService _authService; // הוספנו את ה-AuthService

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        private string _userEmail = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        private string _userPassword = string.Empty;

        [ObservableProperty]
        private bool _entryAsPassword = true;

        // הזרקת השירותים בבנאי
        public SignInPageViewModel(IDataService dataService, AuthService authService)
        {
            _dataService = dataService;
            _authService = authService;
        }

        public string PasswordImage => EntryAsPassword ? "closeeye.png" : "openeye.png";

        partial void OnEntryAsPasswordChanged(bool value) => OnPropertyChanged(nameof(PasswordImage));

        [RelayCommand]
        private void ShowPassword() => EntryAsPassword = !EntryAsPassword;

        [RelayCommand]
        private async Task GoToSignUp() => await Shell.Current.GoToAsync(nameof(SignUpPage));

        private bool CanSignIn() => !string.IsNullOrWhiteSpace(UserEmail) && !string.IsNullOrWhiteSpace(UserPassword);

        [RelayCommand(CanExecute = nameof(CanSignIn))]
        private async Task SignIn()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserEmail) || string.IsNullOrWhiteSpace(UserPassword))
                {
                    await Shell.Current.DisplayAlert("Error", "Enter email and password", "OK");
                    return;
                }

                // שלב 1: התחברות ל-Firebase Auth
                var userCredential = await _authService.SignInAsync(UserEmail, UserPassword);

                if (userCredential == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Login failed", "OK");
                    return;
                }

                // שלב 2: שליפת נתוני המשתמש מה-Database שלנו
                // שימי לב: המשתנה נקרא כאן 'user' כדי להתאים לשאר הקוד
                var user = await _dataService.GetUserAsync(UserEmail, UserPassword);

                if (user == null || string.IsNullOrEmpty(user.Id))
                {
                    await Shell.Current.DisplayAlert("Error", "User not found in database", "OK");
                    return;
                }

                // שמירה גלובלית
                App.LoggedInUser = user;

                if (Shell.Current is AppShell appShell)
                {
                    appShell.SetLoggedInState(true, user);
                }

                // ניתוב חכם לפי תפקיד (Role)
                string route = user.Role == UserRole.Senior
                               ? "//ElderRemindersPage"
                               : "//FamilyDashboardPage";

                await Shell.Current.GoToAsync(route, new Dictionary<string, object>
        {
            { "CurrentUser", user }
        });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Login error: {ex.Message}", "OK");
            }
        }
    }
}