using CareReminderApp.Models;
using CareReminderApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Mail;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.Maui.Media;
using Firebase.Storage;

namespace CareReminderApp.ViewModels;

public partial class ChangeProfileViewModel : ObservableObject, IQueryAttributable
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    private User editableUser;

    public ChangeProfileViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    [RelayCommand]
    public async Task ChangePhoto()
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo == null) return;

            using var stream = await photo.OpenReadAsync();

            // העלאה ל-Firebase (שימוש בכתובת הנכונה מה-Console שלך)
            var task = new FirebaseStorage("remaindsdb.firebasestorage.app")
                .Child("Users")
                .Child($"{EditableUser.Id}.jpg")
                .PutAsync(stream);

            var downloadUrl = await task;

            // עדכון ה-URL במודל הזמני (זה יעדכן את התמונה במסך מיד)
            EditableUser.ProfilePictureUrl = downloadUrl;

            await Shell.Current.DisplayAlert("Success", "Photo uploaded! Don't forget to save changes.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("DisplayUser"))
        {
            var user = query["DisplayUser"] as User;

            if (user != null)
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
                    Role = user.Role // <--- כאן נפתרת הבעיה של שינוי ה-Role למבוגר!
                };
            }
        }
    }

    // 🔍 Validation
    private string ValidateUser()
    {
        if (EditableUser == null)
            return "User not loaded";

        if (string.IsNullOrWhiteSpace(EditableUser.FirstName))
            return "First name is required";

        if (string.IsNullOrWhiteSpace(EditableUser.LastName))
            return "Last name is required";

        if (string.IsNullOrWhiteSpace(EditableUser.UserEmail))
            return "Email is required";

        try
        {
            var addr = new MailAddress(EditableUser.UserEmail);
            if (addr.Address != EditableUser.UserEmail)
                return "Invalid email format";
        }
        catch
        {
            return "Invalid email format";
        }

        if (string.IsNullOrWhiteSpace(EditableUser.Mobile) || EditableUser.Mobile.Length < 9)
            return "Invalid phone number";

        return string.Empty;
    }

    [RelayCommand]
    public async Task SaveChanges()
    {
        if (EditableUser == null) return;

        // 1. הרצת בדיקת תקינות (Validation) לפני הכל
        var errorMessage = ValidateUser();
        if (!string.IsNullOrEmpty(errorMessage))
        {
            await Shell.Current.DisplayAlert("Validation Error", errorMessage, "OK");
            return;
        }

        try
        {
            // 2. עדכון המשתמש - כעת ה-EditableUser מכיל את ה-Role הנכון
            var success = await _dataService.UpdateUserAsync(EditableUser);

            if (success)
            {
                await Shell.Current.DisplayAlert("Success", "Profile updated successfully!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to update profile", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

}