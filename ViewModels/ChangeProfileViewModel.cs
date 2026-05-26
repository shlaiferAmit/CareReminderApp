using CareReminderApp.Models;
using CareReminderApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Media;
using Firebase.Storage;
using System.Diagnostics;

namespace CareReminderApp.ViewModels
{
    // מחלקת מודל תצוגה לעריכת פרופיל משתמש במערכת
    // אחראית על שינוי פרטי משתמש, העלאת תמונת פרופיל ושמירת השינויים למסד הנתונים
    public partial class ChangeProfileViewModel : ObservableObject, IQueryAttributable
    {
        // שירות הנתונים של האפליקציה (פיירבייס או שירות מדומה)
        private readonly IDataService _dataService;

        // עותק ניתן לעריכה של פרטי המשתמש
        [ObservableProperty]
        private User editableUser;

        // מקור התמונה המוצגת במסך הפרופיל
        [ObservableProperty]
        private ImageSource profileImageSource;

        public ChangeProfileViewModel(IDataService dataService)
        {
            _dataService = dataService;
            EditableUser = new User();
        }

        // קבלת נתונים מהמסך הקודם והעתקתם למודל עריכה
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("DisplayUser", out var value) && value is User user)
            {
                EditableUser = new User
                {
                    Id = user.Id,
                    LocalId = user.LocalId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserEmail = user.UserEmail,
                    Mobile = user.Mobile,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    Role = user.Role
                };

                SetProfileImage(user.ProfilePictureUrl);
            }
        }

        // קביעת תמונת פרופיל בהתאם לכתובת הקיימת או הצגת תמונת ברירת מחדל
        private void SetProfileImage(string url)
        {
            try
            {
                // אם אין תמונה או שהקישור לא תקין - מציגים תמונת ברירת מחדל
                if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    ProfileImageSource = ImageSource.FromFile("user_placeholder.png");
                }
                else
                {
                    // טעינת תמונה מכתובת אינטרנט
                    ProfileImageSource = ImageSource.FromUri(new Uri(url));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"שגיאת טעינת תמונה: {ex.Message}");
                ProfileImageSource = ImageSource.FromFile("user_placeholder.png");
            }
        }

        // שינוי תמונת פרופיל על ידי בחירה מהמצלמה או מהגלריה
        [RelayCommand]
        public async Task ChangePhoto()
        {
            try
            {
                // פתיחת תפריט בחירה
                string source = await Shell.Current.DisplayActionSheet(
                    "Select Photo Source",
                    "Cancel",
                    null,
                    "Camera",
                    "Gallery");

                FileResult photo = null;

                if (source == "Camera")
                {
                    if (MediaPicker.Default.IsCaptureSupported)
                        photo = await MediaPicker.Default.CapturePhotoAsync();
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Camera not supported", "OK");
                        return;
                    }
                }
                else if (source == "Gallery")
                {
                    // בקשת הרשאה (חשוב במיוחד באנדרואיד)
                    var status = await Permissions.RequestAsync<Permissions.Photos>();
                    if (status != PermissionStatus.Granted)
                        return;

                    photo = await MediaPicker.Default.PickPhotoAsync();
                }

                if (photo == null)
                    return;

                using var stream = await photo.OpenReadAsync();

                // שם קובץ ייחודי כדי למנוע cache ישן
                var fileName = $"{EditableUser.Id}_{DateTime.UtcNow.Ticks}.jpg";

                var storage = new FirebaseStorage("remaindsdb.firebasestorage.app");

                var downloadUrl = await storage
                    .Child("Users")
                    .Child(fileName)
                    .PutAsync(stream);

                // עדכון המשתמש
                EditableUser.ProfilePictureUrl = downloadUrl;

                // עדכון תמונה במסך (עם מניעת cache)
                ProfileImageSource = ImageSource.FromUri(
                    new Uri(downloadUrl + $"?v={DateTime.Now.Ticks}")
                );

                await Shell.Current.DisplayAlert("Success", "Photo updated successfully!", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Upload Error: {ex}");
                await Shell.Current.DisplayAlert("Error", "Failed to update photo", "OK");
            }
        }

        // שמירת שינויים בפרופיל המשתמש
        [RelayCommand]
        public async Task SaveChanges()
        {
            if (EditableUser == null)
                return;

            // בדיקה שהוזנו שם פרטי ושם משפחה
            if (string.IsNullOrWhiteSpace(EditableUser.FirstName) ||
                string.IsNullOrWhiteSpace(EditableUser.LastName))
            {
                await Shell.Current.DisplayAlert("שגיאה", "יש למלא שם פרטי ושם משפחה", "אישור");
                return;
            }

            try
            {
                // שמירת הנתונים במסד הנתונים
                var success = await _dataService.UpdateUserAsync(EditableUser);

                if (success)
                {
                    await Shell.Current.DisplayAlert("הצלחה", "הפרופיל עודכן בהצלחה", "אישור");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"שגיאת שמירה: {ex}");
                await Shell.Current.DisplayAlert("שגיאה", "שמירת השינויים נכשלה", "אישור");
            }
        }
    }
}