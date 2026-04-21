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

        [ObservableProperty]
        private bool isBusy;

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
                IsBusy = true;

                // בדיקת קלט בסיסית
                if (string.IsNullOrWhiteSpace(UserEmail) || string.IsNullOrWhiteSpace(UserPassword))
                {
                    await Shell.Current.DisplayAlert("Error", "Please enter both email and password", "OK");
                    return;
                }

                string cleanedEmail = UserEmail.Trim().ToLower();
                string password = UserPassword.Trim();

                // 1. התחברות מול Firebase AuthService
                var userCredential = await _authService.SignInAsync(cleanedEmail, password);

                if (userCredential == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Login failed. Please check your credentials.", "OK");
                    return;
                }

                // 2. שליפת נתוני המשתמש (Role וכו') מה-Database
                var user = await _dataService.GetUserAsync(cleanedEmail, password);

                if (user == null || string.IsNullOrEmpty(user.Id))
                {
                    await Shell.Current.DisplayAlert("Error", "User details not found in database", "OK");
                    return;
                }

                // 3. עדכון הסטייט הגלובלי והפעלת הלוגיקה של ה-AppShell
                if (Shell.Current is AppShell appShell)
                {
                    // הפונקציה הזו בונה את הטאבים לפי ה-Role ומנווטת פנימה
                    appShell.SetLoggedInState(true, user);
                }
                else
                {
                    // מקרה קצה אם Shell.Current לא זמין - ניווט מסורתי כגיבוי
                    App.LoggedInUser = user;
                    string route = user.Role == UserRole.Senior ? "//ElderRemindersPage" : "//FamilyDashboardPage";
                    await Shell.Current.GoToAsync(route);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}