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

        // שים לב: שיניתי ל-UserEmail כדי שיתאים לקריאה ב-Service
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        private string _userEmail = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        private string _userPassword = string.Empty;

        [ObservableProperty]
        private bool _entryAsPassword = true;

        // Command להצגת/הסתרת סיסמה
        [RelayCommand]
        private void ShowPassword() => EntryAsPassword = !EntryAsPassword;

        // Command למעבר לדף הרשמה
        [RelayCommand]
        private async Task GoToSignUp() => await Shell.Current.GoToAsync(nameof(SignUpPage));

        public SignInPageViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        // מאפיין דינמי לתמונה של העין
        public string PasswordImage => EntryAsPassword ? "closeeye.png" : "openeye.png";

        // עדכון התמונה כשהסיסמה משתנה ממוסתרת לגלויה
        partial void OnEntryAsPasswordChanged(bool value) => OnPropertyChanged(nameof(PasswordImage));

        // בדיקה האם ניתן ללחוץ על כפתור ההתחברות
        private bool CanSignIn() => !string.IsNullOrWhiteSpace(UserEmail) && !string.IsNullOrWhiteSpace(UserPassword);

        [RelayCommand(CanExecute = nameof(CanSignIn))]
        private async Task SignIn()
        {
            var user = await _dataService.GetUserAsync(UserEmail, UserPassword);

            if (user != null)
            {
                // יצירת מילון עם נתוני המשתמש להעברה
                var navigationParameter = new Dictionary<string, object>
        {
            { "CurrentUser", user }
        };

                if (user.Role == UserRole.Senior)
                {
                    // העברת הפרמטר בניווט
                    await Shell.Current.GoToAsync(nameof(ElderRemindersPage), navigationParameter);
                }
                else
                {
                    await Shell.Current.GoToAsync(nameof(FamilyDashboardPage), navigationParameter);
                }
            }
        }
    }
}