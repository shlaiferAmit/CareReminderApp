using CareReminderApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CareReminderApp.ViewModels
{
    // מחלקת מודל תצוגה עבור מסך התחברות
    // אחראית על ביצוע התחברות, ולידציה של שדות, שמירת משתמש במכשיר וניווט לאחר התחברות
    public partial class SignInPageViewModel : ObservableObject
    {
        // שירות הנתונים של האפליקציה (פיירבייס או שירות מדומה)
        private readonly IDataService _dataService;

        // שירות אימות משתמשים מול פיירבייס
        private readonly AuthService _authService;

        // כתובת אימייל שהמשתמש מזין
        [ObservableProperty]
        private string userEmail = string.Empty;

        // סיסמה שהמשתמש מזין
        [ObservableProperty]
        private string userPassword = string.Empty;

        // מצב טעינה בזמן התחברות
        [ObservableProperty]
        private bool isBusy;

        // האם לזכור את המשתמש במכשיר
        [ObservableProperty]
        private bool isRememberMeSelected;

        // האם להציג סיסמה כטקסט מוסתר
        [ObservableProperty]
        private bool entryAsPassword = true;

        // הודעת שגיאה עבור אימייל
        [ObservableProperty]
        private string emailError = string.Empty;

        // הודעת שגיאה עבור סיסמה
        [ObservableProperty]
        private string passwordError = string.Empty;

        public SignInPageViewModel(IDataService dataService, AuthService authService)
        {
            _dataService = dataService;
            _authService = authService;

            LoadRememberedUser();
        }

        // טעינת משתמש שנשמר קודם במכשיר
        private void LoadRememberedUser()
        {
            if (Preferences.Default.Get("IsRemembered", false))
            {
                UserEmail = Preferences.Default.Get("UserEmail", "");
                UserPassword = Preferences.Default.Get("UserPassword", "");
                IsRememberMeSelected = true;
            }
        }

        // בדיקה האם ניתן לבצע התחברות
        public bool CanSignIn =>
            !IsBusy &&
            string.IsNullOrWhiteSpace(EmailError) &&
            string.IsNullOrWhiteSpace(PasswordError) &&
            !string.IsNullOrWhiteSpace(UserEmail) &&
            !string.IsNullOrWhiteSpace(UserPassword);

        // מופעל כאשר משתנה האימייל (לביצוע בדיקות תקינות)
        partial void OnUserEmailChanged(string value)
        {
            Validate();
            SignInCommand.NotifyCanExecuteChanged();
        }

        // מופעל כאשר משתנה הסיסמה (לביצוע בדיקות תקינות)
        partial void OnUserPasswordChanged(string value)
        {
            Validate();
            SignInCommand.NotifyCanExecuteChanged();
        }

        // מופעל כאשר משתנה מצב טעינה
        partial void OnIsBusyChanged(bool value)
        {
            SignInCommand.NotifyCanExecuteChanged();
        }

        // בדיקת תקינות של אימייל וסיסמה
        private void Validate()
        {
            // EMAIL
            if (string.IsNullOrWhiteSpace(UserEmail))
                EmailError = "Email is required";
            else if (!Regex.IsMatch(UserEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                EmailError = "Invalid email format";
            else
                EmailError = string.Empty;

            // PASSWORD
            if (string.IsNullOrWhiteSpace(UserPassword))
                PasswordError = "Password is required";
            else if (UserPassword.Length < 6)
                PasswordError = "Password must be at least 6 characters";
            else
                PasswordError = string.Empty;
        }

        // פעולת התחברות למערכת
        [RelayCommand(CanExecute = nameof(CanSignIn))]
        private async Task SignIn()
        {
            try
            {
                IsBusy = true;

                var email = UserEmail.Trim().ToLower();
                var password = UserPassword.Trim();

                var result = await _authService.SignInAsync(email, password);

                if (result == null)
                {
                    await Shell.Current.DisplayAlert("Login Error", "Incorrect email or password.", "OK");
                    return;
                }

                // שליפת המשתמש ממסד הנתונים
                var user = await _dataService.GetUserAsync(email, password);

                if (user != null && Shell.Current is AppShell appShell)
                {
                    // שמירת משתמש במכשיר אם נבחר
                    if (IsRememberMeSelected)
                    {
                        Preferences.Default.Set("UserEmail", email);
                        Preferences.Default.Set("UserPassword", password);
                        Preferences.Default.Set("IsRemembered", true);
                    }
                    else
                    {
                        Preferences.Default.Remove("UserEmail");
                        Preferences.Default.Remove("UserPassword");
                        Preferences.Default.Set("IsRemembered", false);
                    }
                    // עדכון מצב התחברות באפליקציה
                    appShell.SetLoggedInState(true, user);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;

                // טיפול בשגיאות התחברות נפוצות
                if (errorMessage.Contains("INVALID_LOGIN_CREDENTIALS") ||
                    errorMessage.Contains("INVALID_PASSWORD") ||
                    errorMessage.Contains("EMAIL_NOT_FOUND") ||
                    errorMessage.Contains("USER_NOT_FOUND"))
                {
                    await Shell.Current.DisplayAlert("Error", "Incorrect email or password.", "OK");
                }
                else if (errorMessage.Contains("TOO_MANY_ATTEMPTS_TRY_LATER"))
                {
                    await Shell.Current.DisplayAlert("Error", "Access to this account has been temporarily disabled due to many failed login attempts. Please try again later.", "OK");
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

        // הצגת / הסתרת סיסמה
        [RelayCommand]
        private void ShowPassword()
        {
            EntryAsPassword = !EntryAsPassword;
        }

        // תמונה בהתאם למצב הצגת סיסמה
        public string PasswordImage =>
            EntryAsPassword ? "closeeye.png" : "openeye.png";

        // עדכון אוטומטי של האייקון כשהמצב משתנה
        partial void OnEntryAsPasswordChanged(bool value)
        {
            OnPropertyChanged(nameof(PasswordImage));
        }

        // מעבר למסך הרשמה
        [RelayCommand]
        private async Task GoToSignUp()
        {
            await Shell.Current.GoToAsync("//SignUpPage");
        }
    }
}