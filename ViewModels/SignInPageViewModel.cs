using CareReminderApp.Models;
using CareReminderApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CareReminderApp.Views;



namespace CareReminderApp.ViewModels
{
    public partial class SignInPageViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        private string _userEmail = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        private string _userPassword = string.Empty;

        [ObservableProperty]
        private bool _entryAsPassword = true;

        public SignInPageViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        // תמונה דינמית לעין לפי השמות בתיקייה שלך
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
            var user = await _dataService.GetUserAsync(UserEmail, UserPassword);

            if (user != null)
            {
                App.LoggedInUser = user;

                if (Shell.Current is AppShell appShell)
                {
                    // אנחנו שולחים את המשתמש לפונקציה ב-AppShell
                    // הפונקציה הזו תבנה את הטאבים ותבצע את הניווט הפנימי
                    appShell.SetLoggedInState(true, user);
                }

                // הסרנו את ה-GoToAsync מכאן! אין בו צורך יותר.
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Email or Password", "OK");
            }
        }
    }
}