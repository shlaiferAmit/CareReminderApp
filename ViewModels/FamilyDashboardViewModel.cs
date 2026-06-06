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
    public partial class FamilyDashboardViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private IDisposable? _eldersSubscription;

        [ObservableProperty]
        private User? currentUser;

        [ObservableProperty]
        private string welcomeMessage = "Hello!";

        [ObservableProperty]
        private bool isBusy;

        // הגדרה נקייה ויחידה של הרשימה - פותר את שגיאת ה-Ambiguity
        public ObservableCollection<User> Elders { get; } = new ObservableCollection<User>();

        public FamilyDashboardViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// נקודת הכניסה האמיתית בזמן אמת - נקרא לה מקוד המסך (View Behind) בכל פעם שהמסך עולה
        /// </summary>
        public void StartListeningElders()
        {
            // לוקח את המשתמש הנוכחי שעבר ב-Query או את המשתמש הגלובלי באפליקציה
            var activeUser = CurrentUser ?? App.LoggedInUser;
            if (activeUser == null) return;

            // ניקוי האזנה קודמת אם הייתה קיימת כדי למנוע כפילויות בזיכרון
            StopListeningElders();

            // האזנה אקטיבית ורציפה לשינויים ב-Firebase
            _eldersSubscription = _dataService.ListenEldersForFamily(activeUser.Id)
                .Subscribe(list =>
                {
                    // העברת העדכון לחוט הריצה הראשי (UI Thread) כדי שהמסך יתעדכן ויקפוץ מיד
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Elders.Clear();
                        foreach (var elder in list)
                        {
                            Elders.Add(elder);
                        }
                    });
                }, error =>
                {
                    // טיפול בשגיאות זרימה במידה ויש בעיית תקשורת
                    System.Diagnostics.Debug.WriteLine($"Stream error: {error.Message}");
                });
        }

        /// <summary>
        /// עצירת ההאזנה ברקע כשהמשתמש יוצא מהמסך (חוסך סוללה ומשאבי אינטרנט)
        /// </summary>
        public void StopListeningElders()
        {
            _eldersSubscription?.Dispose();
            _eldersSubscription = null;
        }

        // עדכון הודעת ברכה ברגע שהמשתמש נקלט מה-QueryProperty
        partial void OnCurrentUserChanged(User? value)
        {
            if (value != null)
            {
                WelcomeMessage = $"Good morning, {value.FirstName}!";
                // אם המשתמש הגיע באיחור מהניווט, נתניע את ההאזנה מחדש עבורו
                StartListeningElders();
            }
        }

        [RelayCommand]
        private async Task GoToProfile(User elder)
        {
            if (elder == null) return;
            await Shell.Current.GoToAsync(nameof(ElderProfilePage), new Dictionary<string, object>
            {
                { "Elder", elder }
            });
        }

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

                // 🔥 תיקון הגנה: שימוש ב-UserEmail? כדי למנוע קריסה של NullReferenceException
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
        private async Task RemoveConnection(User elder)
        {
            var activeUser = CurrentUser ?? App.LoggedInUser;
            if (elder == null || activeUser == null) return;

            await _dataService.RemoveUserConnectionAsync(activeUser.Id, elder.Id);
        }
    }
}