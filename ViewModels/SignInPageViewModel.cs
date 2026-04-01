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


namespace CareReminderApp.ViewModels
{
    // השימוש ב-partial וב-ObservableObject פותר את שגיאות ה-CS0103 וה-CS0246
    public partial class SignInPageViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        private string _userName = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        private string _userPassword = string.Empty;

        [ObservableProperty]
        private bool _entryAsPassword = true;

        public ICommand ShowPasswordCommand { get; }
        public ICommand GoToSignUpCommand { get; }

        public SignInPageViewModel(IDataService dataService)
        {
            _dataService = dataService;

            ShowPasswordCommand = new RelayCommand(() => EntryAsPassword = !EntryAsPassword);

            GoToSignUpCommand = new AsyncRelayCommand(async () =>
                await Shell.Current.GoToAsync("SignUpPage"));
        }

        public string PasswordImage => EntryAsPassword ? "closeeye.png" : "openeye.png";

        partial void OnEntryAsPasswordChanged(bool value) => OnPropertyChanged(nameof(PasswordImage));

        [RelayCommand(CanExecute = nameof(CanSignIn))]
        private async Task SignIn()
        {
            // ודאי שהמתודה ב-Service שלך היא GetUserAsync
            var user = await _dataService.GetUserAsync(UserName, UserPassword);

            if (user != null)
            {
                if (Application.Current is App app)
                {
                    app.CurrentUser = user;
                }
                await Shell.Current.GoToAsync("//FamilyDashboardPage");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Email or password incorrect", "OK");
            }
        }

        private bool CanSignIn() => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(UserPassword);
    }
}