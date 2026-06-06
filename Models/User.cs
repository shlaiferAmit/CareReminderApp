using CommunityToolkit.Mvvm.ComponentModel;

namespace CareReminderApp.Models
{
    // מחלקה זו מייצגת משתמש במערכת
    // המחלקה יורשת ממחלקת בסיס שמאפשרת עדכון אוטומטי של הנתונים בממשק המשתמש בהתאם לעקרון מודל תצוגה מודל תצוגה מודל
    // כך שכל שינוי בערכים מתעדכן באופן מיידי במסך ללא צורך בקוד נוסף
    public partial class User : ObservableObject
    {
        // מזהה ייחודי של המשתמש במערכת
        [ObservableProperty]
        private string id = string.Empty;

        // מזהה מקומי של המשתמש מתוך Firebase Authentication
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

        // כתובת תמונת הפרופיל של המשתמש
        // במידה ואין תמונה, תוצג תמונת ברירת מחדל
        [ObservableProperty]
        private string? profilePictureUrl = "default_user.png";

        // נתיב מקומי של תמונת הפרופיל במכשיר
        [ObservableProperty]
        private string? profilePicturePath;
    }

    // Enum המגדיר את סוגי המשתמשים במערכת
    public enum UserRole
    {
        Senior,
        FamilyMember
    }
}