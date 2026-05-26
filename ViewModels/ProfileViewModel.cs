using CareReminderApp.Models;
using CareReminderApp.Services;
using CareReminderApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics;

namespace CareReminderApp.ViewModels
{
    // מחלקת מודל תצוגה עבור מסך פרופיל משתמש
    // אחראית על הצגת פרטי המשתמש, טעינת תזכורות, עדכון תמונת פרופיל וניווט בין מסכים
    public partial class ProfileViewModel : ObservableObject, IQueryAttributable
    {
        // שירות הנתונים של האפליקציה (פיירבייס או שירות מדומה)
        private readonly IDataService _dataService;

        // המשתמש שמוצג במסך הפרופיל
        [ObservableProperty]
        private User _displayUser;

        // כותרת המסך
        [ObservableProperty]
        private string _profileTitle;

        // האם המשתמש הנצפה הוא קשיש
        [ObservableProperty]
        private bool _isViewingElder;

        // האם זה הפרופיל האישי של המשתמש המחובר
        [ObservableProperty]
        private bool _isMyPersonalProfile;

        // רשימת התזכורות של המשתמש
        [ObservableProperty]
        private ObservableCollection<Reminder> _reminders;

        // תמונת הפרופיל המוצגת במסך
        [ObservableProperty]
        private ImageSource _profileImageSource = "user_placeholder.png";

        public ProfileViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Reminders = new ObservableCollection<Reminder>();
        }

        // קבלת פרמטרים מהמסך הקודם וטעינת נתוני המשתמש
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // בדיקה שמשתמש מחובר קיים במערכת
            if (App.LoggedInUser != null)
            {
                try
                {
                    Debug.WriteLine($"LoggedInUser ID: {App.LoggedInUser?.Id}");
                    Debug.WriteLine($"LoggedInUser Email: {App.LoggedInUser?.UserEmail}");
                    Debug.WriteLine($"LoggedInUser Role: {App.LoggedInUser?.Role}");

                    // שליפת נתוני המשתמש המעודכנים מהשרת
                    var userFromServer = await _dataService.GetUserByIdAsync(App.LoggedInUser.Id);

                    if (userFromServer == null)
                    {
                        await Shell.Current.DisplayAlert(
                            "שגיאה",
                            "המשתמש לא נמצא בשרת",
                            "OK");

                        return;
                    }

                    if (userFromServer != null)
                    {
                        // עדכון המשתמש המוצג במסך
                        DisplayUser = userFromServer;

                        // עדכון המשתמש הגלובלי באפליקציה
                        App.LoggedInUser = userFromServer;

                        // עדכון תצוגת הפרופיל
                        UpdateProfileImage();
                        ProfileTitle = "My Profile";
                        IsMyPersonalProfile = true;
                    }
                }
                catch (Exception ex)
                {
                    // הדפסת שגיאה לצורך איתור בעיות
                    Debug.WriteLine($"Full Error: {ex}");

                    await Shell.Current.DisplayAlert("אבחון שגיאה", ex.Message, "הבנתי");
                }
            }
        }

        // עדכון תמונת פרופיל לפי מקור הנתונים
        private void UpdateProfileImage()
        {
            try
            {
                // אם קיימת כתובת תמונה תקינה מהשרת
                if (DisplayUser != null &&
                    !string.IsNullOrWhiteSpace(DisplayUser.ProfilePictureUrl) &&
                    Uri.IsWellFormedUriString(DisplayUser.ProfilePictureUrl, UriKind.Absolute))
                {
                    ProfileImageSource = ImageSource.FromUri(new Uri(DisplayUser.ProfilePictureUrl));
                }
                // אם יש קובץ מקומי
                else if (DisplayUser != null && !string.IsNullOrWhiteSpace(DisplayUser.ProfilePicturePath))
                {
                    ProfileImageSource = ImageSource.FromFile(DisplayUser.ProfilePicturePath);
                }
                // תמונת ברירת מחדל
                else
                {
                    ProfileImageSource = "user_placeholder.png";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting image source: {ex.Message}");
                ProfileImageSource = "user_placeholder.png"; 
            }
        }

        // טעינת תזכורות של המשתמש
        [RelayCommand]
        private async Task LoadRemindersAsync()
        {
            if (DisplayUser == null) return;
            var list = await _dataService.GetRemindersByUserIdAsync(DisplayUser.Id);
            Reminders = new ObservableCollection<Reminder>(list);
        }

        // מעבר למסך עריכת פרופיל
        [RelayCommand]
        private async Task EditProfile()
        {
            var navParam = new Dictionary<string, object> { { "DisplayUser", DisplayUser } };
            await Shell.Current.GoToAsync(nameof(ChangeProfilePage), navParam);
        }

        // מעבר למסך הוספת תזכורת
        [RelayCommand]
        private async Task AddReminder()
        {
            if (DisplayUser == null) return;
            var navParam = new Dictionary<string, object> { { "SelectedElder", DisplayUser } };
            await Shell.Current.GoToAsync("AddReminderPage", navParam);
        }

        // מעבר למסך הבית לפי סוג המשתמש
        [RelayCommand]
        private async Task GoToHome()
        {
            if (App.LoggedInUser == null)
            {
                await Shell.Current.GoToAsync("//MainPage");
                return;
            }

            if (App.LoggedInUser.Role == UserRole.Senior)
                await Shell.Current.GoToAsync("//ElderRemindersPage");
            else
                await Shell.Current.GoToAsync("//FamilyDashboardPage");
        }

        // מעבר לרשימת הקשישים
        [RelayCommand]
        private async Task GoToSeniors()
        {
            await Shell.Current.GoToAsync("//EldersListPage");
        }

        [RelayCommand]
        private async Task GoToProfile()
        {
        }
    }
}