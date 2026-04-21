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
                    // שליפת המידע מהפיירבייס בצורה אסינכרונית
                    var userFromServer = await _dataService.GetUserByIdAsync(App.LoggedInUser.Id);

                    if (userFromServer != null)
                    {
                        // כאן קורה הקסם - ברגע שזה מתעדכן, המסך מתמלא
                        DisplayUser = userFromServer;

                        // עדכון תמונה וכותרת
                        UpdateProfileImage();
                        ProfileTitle = "My Profile";
                        IsMyPersonalProfile = true;
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("שגיאה", "לא הצלחנו לטעון נתונים", "אוקיי");
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
            // הדפסה לחלונית ה-Output לבדיקה
            System.Diagnostics.Debug.WriteLine(">>> ChangePhoto Command Executed");

            if (DisplayUser == null)
            {
                await Shell.Current.DisplayAlert("Error", "User data not loaded", "OK");
                return;
            }

            try
            {
                // 1. בחירת תמונה מהגלריה
                var photo = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Please select a photo"
                });

                if (photo == null) return;

                // 2. עדכון ויזואלי מיידי
                var streamForDisplay = await photo.OpenReadAsync();
                ProfileImageSource = ImageSource.FromStream(() => streamForDisplay);

                // 3. העלאה ל-Firebase (מומלץ להוסיף Loading indicator אם יש לך)
                using (var streamForUpload = await photo.OpenReadAsync())
                {
                    string firebaseUrl = await _dataService.UploadUserImageAsync(streamForUpload, DisplayUser.Id);

                    if (!string.IsNullOrEmpty(firebaseUrl))
                    {
                        // עדכון המודל
                        DisplayUser.ProfilePictureUrl = firebaseUrl;

                        // 4. שמירה בבסיס הנתונים
                        await _dataService.UpdateUserAsync(DisplayUser);

                        await Shell.Current.DisplayAlert("Success", "Profile photo updated!", "Great");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($">>> Photo Error: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to upload: " + ex.Message, "OK");
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
        private async Task SaveChanges()
        {
            // כאן תבוא לוגיקת השמירה (למשל ל-Firebase)
            await Shell.Current.GoToAsync("..");
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