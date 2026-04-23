using CommunityToolkit.Mvvm.ComponentModel;

namespace CareReminderApp.Models
{
    public partial class User : ObservableObject
    {
        [ObservableProperty]
        private string id = string.Empty;

        // הוספת השדה הזה כי ה-ViewModel מחפש אותו
        [ObservableProperty]
        private string localId = string.Empty;

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

        // הוספת השדה הזה כדי לפתור את השגיאה ב-ProfileViewModel
        [ObservableProperty]
        private string? profilePicturePath;
    }

    public enum UserRole { Senior, FamilyMember }
}