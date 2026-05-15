using CareReminderApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CareReminderApp.ViewModels
{
    public partial class SignInPageViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly AuthService _authService;

        [ObservableProperty]
        private string userEmail = string.Empty;

        [ObservableProperty]
        private string userPassword = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isRememberMeSelected;

        [ObservableProperty]
        private bool entryAsPassword = true;

        [ObservableProperty]
        private string emailError = string.Empty;

        [ObservableProperty]
        private string passwordError = string.Empty;

        public SignInPageViewModel(IDataService dataService, AuthService authService)
        {
            _dataService = dataService;
            _authService = authService;

            LoadRememberedUser();
        }

        private void LoadRememberedUser()
        {
            if (Preferences.Default.Get("IsRemembered", false))
            {
                UserEmail = Preferences.Default.Get("UserEmail", "");
                UserPassword = Preferences.Default.Get("UserPassword", "");
                IsRememberMeSelected = true;
            }
        }

        public bool CanSignIn =>
            !IsBusy &&
            string.IsNullOrWhiteSpace(EmailError) &&
            string.IsNullOrWhiteSpace(PasswordError) &&
            !string.IsNullOrWhiteSpace(UserEmail) &&
            !string.IsNullOrWhiteSpace(UserPassword);

        partial void OnUserEmailChanged(string value)
        {
            Validate();
            SignInCommand.NotifyCanExecuteChanged();
        }

        partial void OnUserPasswordChanged(string value)
        {
            Validate();
            SignInCommand.NotifyCanExecuteChanged();
        }

        partial void OnIsBusyChanged(bool value)
        {
            SignInCommand.NotifyCanExecuteChanged();
        }

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
                    await Shell.Current.DisplayAlert("Error", "Login failed", "OK");
                    return;
                }

                var user = await _dataService.GetUserAsync(email, password);

                if (user != null && Shell.Current is AppShell appShell)
                {
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

                    appShell.SetLoggedInState(true, user);
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

        [RelayCommand]
        private void ShowPassword()
        {
            EntryAsPassword = !EntryAsPassword;
        }

        public string PasswordImage =>
            EntryAsPassword ? "closeeye.png" : "openeye.png";

        partial void OnEntryAsPasswordChanged(bool value)
        {
            OnPropertyChanged(nameof(PasswordImage));
        }

        [RelayCommand]
        private async Task GoToSignUp()
        {
            await Shell.Current.GoToAsync("//SignUpPage");
        }
    }
}