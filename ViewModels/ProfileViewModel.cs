using CareReminderApp.Models;
using CareReminderApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CareReminderApp.Views;

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
            if (query.TryGetValue("SelectedUser", out var selected) && selected is User elder)
            {
                DisplayUser = elder;
                ProfileTitle = $"{elder.FirstName}'s Profile";
                IsViewingElder = true;
                IsMyPersonalProfile = false;
                await LoadRemindersAsync();
            }
            else
            {
                DisplayUser = App.LoggedInUser;
                ProfileTitle = "My Profile";
                IsViewingElder = false;
                IsMyPersonalProfile = true;
                Reminders.Clear();
            }
        }

        [RelayCommand]
        private async Task ChangePhoto()
        {
            // בדיקה שזה אכן הפרופיל שלי לפני שמאפשרים שינוי
            if (!IsMyPersonalProfile) return;

            try
            {
                // פתיחת הגלריה
                var photo = await MediaPicker.Default.PickPhotoAsync();

                if (photo != null)
                {
                    // 1. יצירת נתיב קבוע בתיקיית האפליקציה (כדי שהתמונה לא תימחק)
                    string localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);

                    // 2. העתקת הקובץ מהתיקייה הזמנית לתיקייה הקבועה
                    using (Stream sourceStream = await photo.OpenReadAsync())
                    using (FileStream localFileStream = File.OpenWrite(localFilePath))
                    {
                        await sourceStream.CopyToAsync(localFileStream);
                    }

                    // 3. עדכון ה-UI
                    ProfileImageSource = ImageSource.FromFile(localFilePath);

                    // 4. עדכון אובייקט המשתמש (כדי שיוכל להישמר ב-DB בעתיד)
                    if (DisplayUser != null)
                    {
                        // כאן תעדכן את השדה הרלוונטי במודל User שלך
                        // DisplayUser.ProfilePicturePath = localFilePath;
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("שגיאה", "לא ניתן היה לבחור תמונה: " + ex.Message, "אוקיי");
            }
        }

        // הפקודה שחוסרת את השגיאה ב-ChangeProfilePage
        [RelayCommand]
        private async Task SaveChanges()
        {
            // כאן אפשר להוסיף שמירה ל-Firebase בעתיד
            await Shell.Current.GoToAsync("..");
        }

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

            // בדיקה לפי ה-Enum שהגדרת במודל
            if (App.LoggedInUser.Role == UserRole.Senior)
            {
                // אם הוא מבוגר - הולך לדף התזכורות שלו
                await Shell.Current.GoToAsync("//ElderRemindersPage");
            }
            else
            {
                // אם הוא בן משפחה - הולך לדשבורד
                await Shell.Current.GoToAsync("//FamilyDashboardPage");
            }
        }

    }
}