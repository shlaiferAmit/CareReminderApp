using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Auth.Providers;


// מחלקה זו אחראית על ניהול התחברות והרשמה של משתמשים במערכת באמצעות פיירבייס אוטנטיקציה

public class AuthService
{
    private readonly FirebaseAuthClient _client;

    public AuthService()
    {
        // הגדרת תצורה לשירות האימות של פיירבייס כולל מפתח אפיאיי וספקי התחברות
        var config = new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyCJIwRq4lAEC6zWkjM-A-e6fsELljIyeWc",
            AuthDomain = "remaindsdb.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        };

        // יצירת לקוח התחברות מול פיירבייס לפי התצורה שהוגדרה
        _client = new FirebaseAuthClient(config);
    }

    // פונקציה להרשמת משתמש חדש למערכת באמצעות אימייל וסיסמה
    public async Task<UserCredential> SignUpAsync(string email, string password)
        => await _client.CreateUserWithEmailAndPasswordAsync(email, password);

    // פונקציה להתחברות משתמש קיים למערכת באמצעות אימייל וסיסמה
    public async Task<UserCredential> SignInAsync(string email, string password)
        => await _client.SignInWithEmailAndPasswordAsync(email, password);

    // החזרת מזהה המשתמש המחובר כרגע במערכת
    public string GetCurrentUserId()
        => _client.User?.Uid;
}