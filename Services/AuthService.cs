using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Auth.Providers;

namespace CareReminderApp.Services
{
    public class AuthService
    {
        private const string ApiKey = "AIzaSyCJIwRq4lAEC6zWkjM-A-e6fsELljIyeWc";

        private const string ProjectId = "remaindsdb";
        private readonly FirebaseAuthClient _client;

        public AuthService()
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = "AIzaSyCJIwRq4lAEC6zWkjM-A-e6fsELljIyeWc",
                AuthDomain = "remaindsdb.firebaseapp.com",
                Providers = new Firebase.Auth.Providers.FirebaseAuthProvider[]
                {
                    new Firebase.Auth.Providers.EmailProvider()
                }
            };

            _client = new FirebaseAuthClient(config);
        }

        public async Task<UserCredential> SignUpAsync(string email, string password)
        {
            return await _client.CreateUserWithEmailAndPasswordAsync(email, password);
        }

        public async Task<UserCredential> SignInAsync(string email, string password)
        {
            return await _client.SignInWithEmailAndPasswordAsync(email, password);
        }
    }
}