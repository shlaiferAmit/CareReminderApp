using CareReminderApp.Models;
using CareReminderApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CareReminderApp.Views;

namespace CareReminderApp.ViewModels
{
    public partial class ProfileViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private User _displayUser;

        [ObservableProperty]
        private string _profileTitle;

        [ObservableProperty]
        private bool _isViewingElder;

        [ObservableProperty]
        private bool _isMyPersonalProfile;

        [ObservableProperty]
        private ObservableCollection<Reminder> _reminders;

        [ObservableProperty]
        private ImageSource _profileImageSource = "user_placeholder.png";

        public ProfileViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Reminders = new ObservableCollection<Reminder>();
        }
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // ודאי שהמשתמש המחובר קיים
            if (App.LoggedInUser != null)
            {
                try
                {
                    // שליפת המידע הכי עדכני מה-Firebase לפי ה-ID
                    var userFromServer = await _dataService.GetUserByIdAsync(App.LoggedInUser.Id);

                    if (userFromServer != null)
                    {
                        // 1. עדכון המשתמש לתצוגה במסך הפרופיל
                        DisplayUser = userFromServer;

                        // 2. עדכון המשתמש הגלובלי ב-App - זה הפתרון לבעיה שלך!
                        App.LoggedInUser = userFromServer;

                        // 3. עדכון אלמנטים ויזואליים
                        UpdateProfileImage();
                        ProfileTitle = "My Profile";
                        IsMyPersonalProfile = true;
                    }
                }
                catch (Exception ex)
                {
                    // הדפסה ללוג לצורכי ניפוי שגיאות
                    System.Diagnostics.Debug.WriteLine($">>> Profile Load Error: {ex.Message}");
                    await Shell.Current.DisplayAlert("שגיאה", "לא הצלחנו לרענן את נתוני הפרופיל מהשרת", "אוקיי");
                }
            }
        }

        private void UpdateProfileImage()
        {
            // קודם בודקים אם יש כתובת אינטרנטית (Firebase)
            if (DisplayUser != null && !string.IsNullOrEmpty(DisplayUser.ProfilePictureUrl))
            {
                ProfileImageSource = ImageSource.FromUri(new Uri(DisplayUser.ProfilePictureUrl));
            }
            // אם אין, בודקים נתיב מקומי (למקרה של אופליין או פיתוח)
            else if (DisplayUser != null && !string.IsNullOrEmpty(DisplayUser.ProfilePicturePath))
            {
                ProfileImageSource = ImageSource.FromFile(DisplayUser.ProfilePicturePath);
            }
            // ברירת מחדל
            else
            {
                ProfileImageSource = "user_placeholder.png";
            }
        }
        [RelayCommand]
        private async Task ChangePhoto()
        {
            if (DisplayUser == null)
                return;

            try
            {
                var action = await Shell.Current.DisplayActionSheet(
                    "Select Photo Source",
                    "Cancel",
                    null,
                    "Gallery",
                    "Camera");

                FileResult photo = null;

                if (action == "Gallery")
                    photo = await MediaPicker.Default.PickPhotoAsync();

                else if (action == "Camera")
                    photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo == null)
                    return;

                // 📥 פותחים stream אחד בלבד
                using var stream = await photo.OpenReadAsync();

                // 📤 מעלים לפיירבייס
                string firebaseUrl = await _dataService.UploadUserImageAsync(stream, DisplayUser.Id);

                if (!string.IsNullOrEmpty(firebaseUrl))
                {
                    // 💾 עדכון משתמש
                    DisplayUser.ProfilePictureUrl = firebaseUrl;
                    await _dataService.UpdateUserAsync(DisplayUser);

                    // 🔄 עדכון UI
                    ProfileImageSource = ImageSource.FromUri(new Uri(firebaseUrl));

                    await Shell.Current.DisplayAlert("Success", "Profile photo updated!", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task LoadRemindersAsync()
        {
            if (DisplayUser == null) return;
            var list = await _dataService.GetRemindersByUserIdAsync(DisplayUser.Id);
            Reminders = new ObservableCollection<Reminder>(list);
        }

        [RelayCommand]
        private async Task EditProfile()
        {
            var navParam = new Dictionary<string, object> { { "DisplayUser", DisplayUser } };
            await Shell.Current.GoToAsync(nameof(ChangeProfilePage), navParam);
        }

        [RelayCommand]
        private async Task AddReminder()
        {
            if (DisplayUser == null) return;
            var navParam = new Dictionary<string, object> { { "SelectedElder", DisplayUser } };
            await Shell.Current.GoToAsync("AddReminderPage", navParam);
        }

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



        [RelayCommand]
        private async Task GoToSeniors()
        {
            await Shell.Current.GoToAsync("//EldersListPage");
        }

        [RelayCommand]
        private async Task GoToProfile()
        {
            // אנחנו כבר כאן, אז אולי רק רענון או כלום
        }
    }
}