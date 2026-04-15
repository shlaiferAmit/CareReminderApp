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
            // ודאי שהמשתמש המחובר קיים
            if (App.LoggedInUser != null)
            {
                try
                {
                    // שליפת המידע מהפיירבייס בצורה אסינכרונית
                    var userFromServer = await _dataService.GetUserByIdAsync(App.LoggedInUser.Id);

                    if (userFromServer != null)
                    {
                        // כאן קורה הקסם - ברגע שזה מתעדכן, המסך מתמלא
                        DisplayUser = userFromServer;

                        // עדכון תמונה וכותרת
                        UpdateProfileImage();
                        ProfileTitle = "My Profile";
                        IsMyPersonalProfile = true;
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("שגיאה", "לא הצלחנו לטעון נתונים", "אוקיי");
                }
            }
        }

        private void UpdateProfileImage()
        {
            if (DisplayUser != null && !string.IsNullOrEmpty(DisplayUser.ProfilePicturePath))
            {
                ProfileImageSource = ImageSource.FromFile(DisplayUser.ProfilePicturePath);
            }
            else
            {
                ProfileImageSource = "user_placeholder.png";
            }
        }

        [RelayCommand]
        private async Task ChangePhoto()
        {
            if (!IsMyPersonalProfile) return;

            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo != null)
                {
                    string localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);

                    using (Stream sourceStream = await photo.OpenReadAsync())
                    using (FileStream localFileStream = File.OpenWrite(localFilePath))
                    {
                        await sourceStream.CopyToAsync(localFileStream);
                    }

                    ProfileImageSource = ImageSource.FromFile(localFilePath);

                    if (DisplayUser != null)
                    {
                        DisplayUser.ProfilePicturePath = localFilePath;
                        // כאן מומלץ להוסיף קריאה לשירות נתונים כדי לשמור את הנתיב ב-DB
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("שגיאה", "לא ניתן היה לעדכן תמונה: " + ex.Message, "אוקיי");
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
        private async Task SaveChanges()
        {
            // כאן תבוא לוגיקת השמירה (למשל ל-Firebase)
            await Shell.Current.GoToAsync("..");
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