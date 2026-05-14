using CareReminderApp.Models;
using CareReminderApp.Services;
using CareReminderApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace CareReminderApp.ViewModels
{
    public partial class SignInPageViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly AuthService _authService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        private string _userEmail = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        private string _userPassword = string.Empty;

        [ObservableProperty]
        private bool _isRememberMeSelected;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PasswordImage))]
        private bool _entryAsPassword = true;

        [ObservableProperty]
        private bool isBusy;

        public SignInPageViewModel(IDataService dataService, AuthService authService)
        {
            _dataService = dataService;
            _authService = authService;
        }

        public string PasswordImage => EntryAsPassword ? "closeeye.png" : "openeye.png";

        [RelayCommand]
        private void ShowPassword() => EntryAsPassword = !EntryAsPassword;

        [RelayCommand]
        private async Task GoToSignUp() => await Shell.Current.GoToAsync("//SignUpPage");

        private bool CanSignIn() => !string.IsNullOrWhiteSpace(UserEmail) && !string.IsNullOrWhiteSpace(UserPassword);

        [RelayCommand(CanExecute = nameof(CanSignIn))]
        private async Task SignIn()
        {
            try
            {
                IsBusy = true;
                string cleanedEmail = UserEmail?.Trim().ToLower() ?? string.Empty;
                string password = UserPassword?.Trim() ?? string.Empty;

                var userCredential = await _authService.SignInAsync(cleanedEmail, password);
                if (userCredential == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Login failed", "OK");
                    return;
                }

                var user = await _dataService.GetUserAsync(cleanedEmail, password);
                if (user != null && Shell.Current is AppShell appShell)
                {
                    if (IsRememberMeSelected)
                    {
                        Preferences.Default.Set("UserEmail", cleanedEmail);
                        Preferences.Default.Set("UserPassword", password);
                        Preferences.Default.Set("IsRemembered", true);
                    }

                    // שימוש בשיטה הדינמית שלך מה-AppShell
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
    }
}