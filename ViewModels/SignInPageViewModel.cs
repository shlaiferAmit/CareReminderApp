using CareReminderApp.Models;
using CareReminderApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CareReminderApp.Views;
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
                // שלב 1: אימות מול Firebase Auth
                var userCredential = await _authService.SignInAsync(UserEmail, UserPassword);

                if (userCredential != null)
                {
                    // שלב 2: משיכת נתוני המשתמש מה-Database שלנו (למשל שם, תפקיד וכו')
                    // אנחנו משתמשים באימייל כדי למצוא את המשתמש המתאים
                    var user = await _dataService.GetUserAsync(UserEmail, UserPassword);

                    if (user != null)
                    {
                        App.LoggedInUser = user;

                        if (Shell.Current is AppShell appShell)
                        {
                            // מעבר לדף הבית ועדכון הסטטוס
                            appShell.SetLoggedInState(true, user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // אם הפרטים לא נכונים, Firebase יזרוק שגיאה שנתפוס כאן
                await Shell.Current.DisplayAlert("Error", "התחברות נכשלה: אימייל או סיסמה לא נכונים", "OK");
            }
        }
    }
}