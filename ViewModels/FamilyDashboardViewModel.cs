using CareReminderApp.Models;
using CareReminderApp.Services;
using CareReminderApp.Views; 
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareReminderApp.ViewModels
{
    [QueryProperty(nameof(CurrentUser), "CurrentUser")]
    // מחלקת מודל תצוגה עבור לוח הבקרה של בן משפחה
    // אחראית על הצגת הקשישים המקושרים, הוספת קשישים חדשים וניווט למסך פרופיל קשיש
    public partial class FamilyDashboardViewModel : ObservableObject
    {
        // שירות הנתונים של האפליקציה (פיירבייס או שירות מדומה)
        private readonly IDataService _dataService;

        // המשתמש המחובר כרגע
        [ObservableProperty]
        private User? currentUser;

        // הודעת ברכה במסך הבית
        [ObservableProperty]
        private string welcomeMessage = "Hello!";

        // מצב טעינה - מונע פעולות כפולות בזמן ריצה
        [ObservableProperty]
        private bool isBusy;

        // רשימת הקשישים המשויכים לבן המשפחה
        [ObservableProperty]
        private ObservableCollection<User> elders;

        public FamilyDashboardViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Elders = new ObservableCollection<User>();

            // טעינת משתמש מחובר אם קיים
            if (CurrentUser == null && App.LoggedInUser != null)
            {
                CurrentUser = App.LoggedInUser;
            }
        }

        // עדכון הודעת ברכה כאשר המשתמש משתנה
        partial void OnCurrentUserChanged(User? value)
        {
            if (value != null)
                WelcomeMessage = $"Good morning, {value.FirstName}!";
        }

        // טעינת רשימת הקשישים המשויכים למשתמש
        [RelayCommand]
        public async Task LoadElders()
        {
            var activeUser = CurrentUser ?? App.LoggedInUser;
            if (IsBusy || activeUser == null) return;

            IsBusy = true;
            try
            {
                var result = await _dataService.GetEldersForFamilyAsync(activeUser.Id);
                Elders.Clear();
                foreach (var elder in result)
                {
                    Elders.Add(elder);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading elders: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // מעבר למסך פרופיל קשיש
        [RelayCommand]
        private async Task GoToProfile(User elder)
        {
            if (elder == null) return;
            await Shell.Current.GoToAsync(nameof(ElderProfilePage), new Dictionary<string, object>
            {
                { "Elder", elder }
            });
        }

        // הוספת קשיש חדש למערכת באמצעות אימייל
        [RelayCommand]
        private async Task AddSenior()
        {
            if (IsBusy) return;
            var activeUser = CurrentUser ?? App.LoggedInUser;

            if (activeUser == null)
            {
                await Shell.Current.DisplayAlert("Error", "User not identified.", "OK");
                return;
            }

            string emailInput = await Shell.Current.DisplayPromptAsync(
                "Add Senior",
                "Enter the senior's email address:",
                "Add",
                "Cancel");

            if (string.IsNullOrWhiteSpace(emailInput)) return;

            IsBusy = true;
            try
            {
                var allUsers = await _dataService.GetUsersAsync();
                var emailToFind = emailInput.Trim().ToLower();

                var senior = allUsers.FirstOrDefault(u =>
                    u.UserEmail?.ToLower() == emailToFind &&
                    u.Role == UserRole.Senior);

                if (senior == null)
                {
                    await Shell.Current.DisplayAlert("Not Found", "No senior found with this email.", "OK");
                    return;
                }

                if (senior.Id == activeUser.Id)
                {
                    await Shell.Current.DisplayAlert("Error", "You cannot add yourself.", "OK");
                    return;
                }

                await _dataService.InviteElderAsync(activeUser.Id, senior.Id);
                await Shell.Current.DisplayAlert("Success", $"Request sent to {senior.FirstName}.", "Great");

                // רענון רשימת הקשישים
                await LoadElders();
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