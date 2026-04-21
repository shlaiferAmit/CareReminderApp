using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace CareReminderApp.Models
{
    public partial class User : ObservableObject
    {
        [ObservableProperty]
        private string id = string.Empty;

        [ObservableProperty]
        private string firstName = string.Empty;

        [ObservableProperty]
        private string lastName = string.Empty;

        [ObservableProperty]
        private string userEmail = string.Empty;

        [ObservableProperty]
        private string userPassword = string.Empty;

        [ObservableProperty]
        private string mobile = string.Empty;

        [ObservableProperty]
        private UserRole role;

        [ObservableProperty]
        private string? profilePictureUrl;

        // הוסיפי את השורה הזו:
        [ObservableProperty]
        private string? profilePicturePath;
    }

    public enum UserRole { Senior, FamilyMember }
}