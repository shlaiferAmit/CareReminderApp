using CareReminderApp.Models;
using CareReminderApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Media;
using Firebase.Storage;
using System.Diagnostics;

namespace CareReminderApp.ViewModels;

public partial class ChangeProfileViewModel : ObservableObject, IQueryAttributable
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    private User editableUser;

    [ObservableProperty]
    private ImageSource profileImageSource;

    public ChangeProfileViewModel(IDataService dataService)
    {
        _dataService = dataService;
        EditableUser = new User();
    }

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

    private void SetProfileImage(string url)
    {
        try
        {
            // בדיקה האם הכתובת ריקה או אינה מתחילה ב-http (כלומר אינה URL תקין)
            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                ProfileImageSource = ImageSource.FromFile("user_placeholder.png");
            }
            else
            {
                // ניסיון ליצור URI - אם הפורמט שגוי, ה-catch יטפל בזה
                ProfileImageSource = ImageSource.FromUri(new Uri(url));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Image Error: {ex.Message}");
            ProfileImageSource = ImageSource.FromFile("user_placeholder.png");
        }
    }

    [RelayCommand]
    public async Task ChangePhoto()
    {
        try
        {
            // 1. פתיחת תפריט בחירה
            string source = await Shell.Current.DisplayActionSheet("Select Photo Source", "Cancel", null, "Camera", "Gallery");

            FileResult photo = null;

            if (source == "Camera")
            {
                if (MediaPicker.Default.IsCaptureSupported)
                    photo = await MediaPicker.Default.CapturePhotoAsync();
                else
                    await Shell.Current.DisplayAlert("Error", "Camera not supported on this device", "OK");
            }
            else if (source == "Gallery")
            {
                photo = await MediaPicker.Default.PickPhotoAsync();
            }

            if (photo == null) return;

            // הצגת אינדיקציה למשתמש שהעלאה התחילה
            // אפשר להוסיף כאן IsBusy = true; אם יש לך משתנה כזה

            using var stream = await photo.OpenReadAsync();

            var storage = new FirebaseStorage("remaindsdb.firebasestorage.app");

            var downloadUrl = await storage
                .Child("Users")
                .Child($"{EditableUser.Id}.jpg")
                .PutAsync(stream);

            // עדכון המודל והתמונה במסך
            EditableUser.ProfilePictureUrl = downloadUrl;
            SetProfileImage(downloadUrl);

            await Shell.Current.DisplayAlert("Success", "Photo uploaded! Save to apply changes.", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Upload Error: {ex}");
            await Shell.Current.DisplayAlert("Error", "Failed to process photo", "OK");
        }
    }

    [RelayCommand]
    public async Task SaveChanges()
    {
        if (EditableUser == null)
            return;

        if (string.IsNullOrWhiteSpace(EditableUser.FirstName) ||
            string.IsNullOrWhiteSpace(EditableUser.LastName))
        {
            await Shell.Current.DisplayAlert("Error", "Name fields cannot be empty", "OK");
            return;
        }

        try
        {
            var success = await _dataService.UpdateUserAsync(EditableUser);

            if (success)
            {
                await Shell.Current.DisplayAlert("Success", "Profile updated!", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Save Error: {ex}");
            await Shell.Current.DisplayAlert("Error", "Failed to save changes", "OK");
        }
    }
}