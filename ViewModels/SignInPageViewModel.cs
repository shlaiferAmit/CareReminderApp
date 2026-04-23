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

                // ניקוי קלט - קריטי למניעת שגיאות התחברות
                string cleanedEmail = UserEmail?.Trim().ToLower() ?? string.Empty;
                string password = UserPassword?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(cleanedEmail) || string.IsNullOrEmpty(password))
                {
                    await Shell.Current.DisplayAlert("שגיאה", "אנא הזיני אימייל וסיסמה", "OK");
                    return;
                }

                // 1. התחברות מול Firebase Auth
                var userCredential = await _authService.SignInAsync(cleanedEmail, password);

                if (userCredential == null)
                {
                    await Shell.Current.DisplayAlert("שגיאה", "התחברות נכשלה. בדקי את פרטי ההתחברות.", "OK");
                    return;
                }

                // 2. שליפת הנתונים המשלימים מה-Database (כמו תפקיד המשתמש)
                var user = await _dataService.GetUserAsync(cleanedEmail, password);

                if (user == null)
                {
                    await Shell.Current.DisplayAlert("שגיאה", "נתוני משתמש לא נמצאו במסד הנתונים", "OK");
                    return;
                }

                // 3. עדכון Shell וניווט
                if (Shell.Current is AppShell appShell)
                {
                    appShell.SetLoggedInState(true, user);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("שגיאה", $"אירעה שגיאה: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}