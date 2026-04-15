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
        private string? profilePicturePath; // סימן שאלה פותר את אזהרה CS8618
    }

    public enum UserRole { Senior, FamilyMember }
}