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
using CareReminderApp.Views;


namespace CareReminderApp.ViewModels
{
    public partial class SignUpPageViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly AuthService _authService; // שירות האימות החדש

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
        private string _mobile = string.Empty;

        [ObservableProperty]
        private bool _entryAsPassword = true;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private UserRole _selectedRole;

        public List<UserRole> RoleOptions { get; } = new List<UserRole>
        {
            UserRole.Senior,
            UserRole.FamilyMember
        };

        // הוספנו את AuthService לבנאי
        public SignUpPageViewModel(IDataService dataService, AuthService authService)
        {
            _dataService = dataService;
            _authService = authService;
        }

        public string PasswordImage => EntryAsPassword ? "closeeye.png" : "openeye.png";

        partial void OnEntryAsPasswordChanged(bool value) => OnPropertyChanged(nameof(PasswordImage));

        [RelayCommand]
        private void TogglePassword() => EntryAsPassword = !EntryAsPassword;

        [RelayCommand]
        private async Task GoToSignIn()
        {
            await Shell.Current.GoToAsync("///SignInPage");
        }

        [RelayCommand(CanExecute = nameof(CanSignUp))]
        private async Task SignUp()
        {
            try
            {
                var authResult = await _authService.SignUpAsync(UserEmail, UserPassword);

                if (authResult != null)
                {
                    bool dbSuccess = await _dataService.RegisterUserAsync(FirstName, LastName, UserEmail, UserPassword, Mobile, SelectedRole);

                    if (dbSuccess)
                    {
                        var user = await _dataService.GetUserAsync(UserEmail, UserPassword);

                        if (user != null && !string.IsNullOrEmpty(user.Id))
                        {
                            App.LoggedInUser = user;

                            if (App.Current?.MainPage != null)
                            {
                                await App.Current.MainPage.DisplayAlert("DEBUG", $"Signed up with ID: {user.Id}", "OK");
                            }

                            if (Shell.Current is AppShell appShell)
                            {
                                appShell.SetLoggedInState(true, user);
                            }
                        }
                        else
                        {
                            if (App.Current?.MainPage != null)
                                await App.Current.MainPage.DisplayAlert("Error", "User saved but not found in DB", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (App.Current?.MainPage != null)
                {
                    await App.Current.MainPage.DisplayAlert("Error", $"Registration failed: {ex.Message}", "OK");
                }
            }
        }

        private bool CanSignUp() =>
            !string.IsNullOrWhiteSpace(FirstName) &&
            !string.IsNullOrWhiteSpace(LastName) &&
            !string.IsNullOrWhiteSpace(UserEmail) &&
            !string.IsNullOrWhiteSpace(UserPassword) &&
            UserPassword.Length >= 6 && // Firebase דורש לפחות 6 תווים
            !string.IsNullOrWhiteSpace(Mobile);
    }
}