using CareReminderApp.Models;
using CareReminderApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareReminderApp.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        [ObservableProperty]
        private User _currentUser;

        public ProfileViewModel()
        {
            // אתחול המשתמש עם השמות המדויקים מהמודל שלך (image_7b4963.png)
            CurrentUser = new User
            {
                FirstName = "Israel",
                LastName = "Israeli",
                UserEmail = "israel@example.com",
                Mobile = "050-0000000"
            };
        }

        [RelayCommand]
        private async Task EditProfile()
        {
            // ניווט לדף העריכה החדש
            await Shell.Current.GoToAsync("ChangeProfilePage");
        }

        [RelayCommand]
        private async Task SaveChanges()
        {
            // כאן תבוא בעתיד השמירה ל-Firebase
            await Shell.Current.DisplayAlert("Success", "Profile updated successfully", "OK");
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task GoHome()
        {
            await Shell.Current.GoToAsync("///TodayRemindersPage");
        }

        [RelayCommand]
        private async Task Logout()
        {
            await Shell.Current.GoToAsync("//SignInPage");
        }
    }
}