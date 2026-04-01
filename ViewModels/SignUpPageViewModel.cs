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
        private string _mobile = string.Empty; // זה המקור ל-Mobile הציבורי

        [ObservableProperty]
        private bool _entryAsPassword = true;

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
            // כאן אנחנו משתמשים ב-Mobile שה-Toolkit ייצר מה-_mobile
            var success = await _dataService.RegisterUserAsync(FirstName, LastName, UserEmail, UserPassword, Mobile);

            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("הצלחה", "החשבון נוצר!", "אישור");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("שגיאה", "הרישום נכשל", "אישור");
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