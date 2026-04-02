using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CareReminderApp.Models;


namespace CareReminderApp.ViewModels
{
    [QueryProperty(nameof(CurrentUser), "CurrentUser")]
    public partial class ProfileViewModel : ObservableObject
    {
        [ObservableProperty]
        private User _currentUser;

        // מחזיר את סוג המשתמש כטקסט ידידותי
        public string RoleName => CurrentUser?.Role == UserRole.Senior ? "Senior" : "Family Member";

        [RelayCommand]
        private async Task Logout()
        {
            // חזרה לדף ההתחברות ואיפוס ה-Stack
            await Shell.Current.GoToAsync("//SignInPage");
        }
    }
}