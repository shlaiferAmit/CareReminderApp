using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Auth.Providers;



public class AuthService
{
    private readonly FirebaseAuthClient _client;

    public AuthService()
    {
        var config = new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyCJIwRq4lAEC6zWkjM-A-e6fsELljIyeWc",
            AuthDomain = "remaindsdb.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        };

        _client = new FirebaseAuthClient(config);
    }

    public async Task<UserCredential> SignUpAsync(string email, string password)
        => await _client.CreateUserWithEmailAndPasswordAsync(email, password);

    public async Task<UserCredential> SignInAsync(string email, string password)
        => await _client.SignInWithEmailAndPasswordAsync(email, password);

    public string GetCurrentUserId()
        => _client.User?.Uid;
}