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
                // 1. בדיקת תקינות קלט ראשונית
                if (string.IsNullOrWhiteSpace(UserEmail) || string.IsNullOrWhiteSpace(UserPassword))
                {
                    await Shell.Current.DisplayAlert("Error", "Please enter both email and password", "OK");
                    return;
                }

                // 2. הכנת הנתונים (נרמול) - מונע בעיות של רווחים או אותיות גדולות
                string cleanedEmail = UserEmail.Trim().ToLower();
                string password = UserPassword.Trim();

                // 3. שלב א': אימות מול Firebase Auth
                var userCredential = await _authService.SignInAsync(cleanedEmail, password);

                if (userCredential == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Login failed. Please check your credentials.", "OK");
                    return;
                }

                // 4. שלב ב': שליפת נתוני המשתמש המלאים מה-Realtime Database
                // חשוב: משתמשים בערכים הנקיים כדי לוודא התאמה לנתונים בענן
                var user = await _dataService.GetUserAsync(cleanedEmail, password);

                if (user == null || string.IsNullOrEmpty(user.Id))
                {
                    await Shell.Current.DisplayAlert("Error", "User not found in database", "OK");
                    return;
                }

                // 5. שמירת המשתמש בזיכרון הגלובלי של האפליקציה
                App.LoggedInUser = user;

                // 6. עדכון מצב התפריט (Shell) במידה וקיים
                if (Shell.Current is AppShell appShell)
                {
                    appShell.SetLoggedInState(true, user);
                }

                // 7. ניתוב חכם לפי תפקיד המשתמש (Senior או FamilyMember)
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
                // תפיסת שגיאות בלתי צפויות (כמו בעיות תקשורת)
                await Shell.Current.DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            }
        }
    }
}