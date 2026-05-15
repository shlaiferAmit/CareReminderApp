using CareReminderApp.Models;
using CareReminderApp.Services;
using CareReminderApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics;

namespace CareReminderApp.ViewModels
{
    public partial class ProfileViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private User _displayUser;

        [ObservableProperty]
        private string _profileTitle;

        [ObservableProperty]
        private bool _isViewingElder;

        [ObservableProperty]
        private bool _isMyPersonalProfile;

        [ObservableProperty]
        private ObservableCollection<Reminder> _reminders;

        [ObservableProperty]
        private ImageSource _profileImageSource = "user_placeholder.png";

        public ProfileViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Reminders = new ObservableCollection<Reminder>();
        }
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // ודאי שהמשתמש המחובר קיים
            if (App.LoggedInUser != null)
            {
                try
                {
                    Debug.WriteLine($"LoggedInUser ID: {App.LoggedInUser?.Id}");
                    Debug.WriteLine($"LoggedInUser Email: {App.LoggedInUser?.UserEmail}");
                    Debug.WriteLine($"LoggedInUser Role: {App.LoggedInUser?.Role}");
                    // שליפת המידע הכי עדכני מה-Firebase לפי ה-ID
                    var userFromServer = await _dataService.GetUserByIdAsync(App.LoggedInUser.Id);

                    if (userFromServer == null)
                    {
                        await Shell.Current.DisplayAlert(
                            "שגיאה",
                            "המשתמש לא נמצא בשרת",
                            "OK");

                        return;
                    }

                    if (userFromServer != null)
                    {
                        // 1. עדכון המשתמש לתצוגה במסך הפרופיל
                        DisplayUser = userFromServer;

                        // 2. עדכון המשתמש הגלובלי ב-App - זה הפתרון לבעיה שלך!
                        App.LoggedInUser = userFromServer;

                        // 3. עדכון אלמנטים ויזואליים
                        UpdateProfileImage();
                        ProfileTitle = "My Profile";
                        IsMyPersonalProfile = true;
                    }
                }
                catch (Exception ex)
                {
                    // זה ידפיס לך בחלון ה-Output של Visual Studio את הסיבה המדויקת
                    Debug.WriteLine($"Full Error: {ex}");

                    // זה יציג לך את הודעת השגיאה האמיתית על המסך של הטלפון
                    await Shell.Current.DisplayAlert("אבחון שגיאה", ex.Message, "הבנתי");
                }
            }
        }

        private void UpdateProfileImage()
        {
            try
            {
                // 1. בדיקה שהשדה לא ריק ומתחיל ב-http (סימן שזה URL מהענן)
                if (DisplayUser != null &&
                    !string.IsNullOrWhiteSpace(DisplayUser.ProfilePictureUrl) &&
                    Uri.IsWellFormedUriString(DisplayUser.ProfilePictureUrl, UriKind.Absolute))
                {
                    ProfileImageSource = ImageSource.FromUri(new Uri(DisplayUser.ProfilePictureUrl));
                }
                // 2. בדיקה אם יש נתיב מקומי
                else if (DisplayUser != null && !string.IsNullOrWhiteSpace(DisplayUser.ProfilePicturePath))
                {
                    ProfileImageSource = ImageSource.FromFile(DisplayUser.ProfilePicturePath);
                }
                // 3. ברירת מחדל
                else
                {
                    ProfileImageSource = "user_placeholder.png";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting image source: {ex.Message}");
                ProfileImageSource = "user_placeholder.png"; // הגנה אחרונה
            }
        }


        [RelayCommand]
        private async Task LoadRemindersAsync()
        {
            if (DisplayUser == null) return;
            var list = await _dataService.GetRemindersByUserIdAsync(DisplayUser.Id);
            Reminders = new ObservableCollection<Reminder>(list);
        }

        [RelayCommand]
        private async Task EditProfile()
        {
            var navParam = new Dictionary<string, object> { { "DisplayUser", DisplayUser } };
            await Shell.Current.GoToAsync(nameof(ChangeProfilePage), navParam);
        }

        [RelayCommand]
        private async Task AddReminder()
        {
            if (DisplayUser == null) return;
            var navParam = new Dictionary<string, object> { { "SelectedElder", DisplayUser } };
            await Shell.Current.GoToAsync("AddReminderPage", navParam);
        }

        [RelayCommand]
        private async Task GoToHome()
        {
            if (App.LoggedInUser == null)
            {
                await Shell.Current.GoToAsync("//MainPage");
                return;
            }

            if (App.LoggedInUser.Role == UserRole.Senior)
                await Shell.Current.GoToAsync("//ElderRemindersPage");
            else
                await Shell.Current.GoToAsync("//FamilyDashboardPage");
        }



        [RelayCommand]
        private async Task GoToSeniors()
        {
            await Shell.Current.GoToAsync("//EldersListPage");
        }

        [RelayCommand]
        private async Task GoToProfile()
        {
            // אנחנו כבר כאן, אז אולי רק רענון או כלום
        }
    }
}