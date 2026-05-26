using CareReminderApp.Models;
using CareReminderApp.Services;
using CareReminderApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CareReminderApp.ViewModels
{
    // המפתח שמתקבל מהדף הקודם
    [QueryProperty(nameof(Elder), "Elder")]

    // מחלקת מודל תצוגה עבור פרופיל של קשיש
    // אחראית על הצגת פרטי הקשיש, טעינת תזכורות שלו והוספת תזכורות חדשות
    public partial class ElderProfileViewModel : ObservableObject
    {
        // שירות הנתונים של האפליקציה (פיירבייס או שירות מדומה)
        private readonly IDataService _dataService;

        // כותרת המסך (מתעדכנת לפי שם הקשיש)
        [ObservableProperty]
        private string title = "פרופיל מבוגר";

        // אובייקט הקשיש שנבחר במסך הקודם
        [ObservableProperty]
        private User elder; // זה האובייקט שמכיל את FirstName, LastName וכו'

        // רשימת התזכורות של הקשיש
        [ObservableProperty]
        private ObservableCollection<Reminder> reminders;

        public ElderProfileViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Reminders = new ObservableCollection<Reminder>();
        }

        // פונקציה שמופעלת אוטומטית כאשר משתנה ערך הקשיש
        partial void OnElderChanged(User value)
        {
            if (value != null)
            {
                Title = $"פרופיל של {value.FirstName}";
                LoadRemindersCommand.Execute(null);
            }
        }

        // טעינת כל התזכורות של הקשיש ממסד הנתונים
        [RelayCommand]
        public async Task LoadReminders()
        {
            if (Elder == null) return;
            try
            {
                var result = await _dataService.GetRemindersAsync(Elder.Id);
                Reminders.Clear();
                foreach (var r in result)
                    Reminders.Add(r);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading reminders: {ex.Message}");
            }
        }

        // מעבר למסך הוספת תזכורת עבור הקשיש הנוכחי
        [RelayCommand]
        private async Task AddReminder()
        {
            if (Elder == null) return;

            await Shell.Current.GoToAsync(nameof(AddReminderPage), new Dictionary<string, object>
            {
                { "SelectedElder", Elder }
            });
        }
    }
}