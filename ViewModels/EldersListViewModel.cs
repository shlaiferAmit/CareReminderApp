using CareReminderApp.Models;
using CareReminderApp.Services;
using CareReminderApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CareReminderApp.ViewModels
{
    public partial class EldersListViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        public EldersListViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Elders = new ObservableCollection<User>();
        }

        // שימוש במבנה קלאסי שתואם לכל גרסאות C# 12 ומטה
        [ObservableProperty]
        private ObservableCollection<User> elders;

        [ObservableProperty]
        private bool isBusy;

        [RelayCommand]
        public async Task LoadElders()
        {
            if (IsBusy || App.LoggedInUser == null) return;
            IsBusy = true;

            try
            {
                var result = await _dataService.GetEldersForFamilyAsync(App.LoggedInUser.Id);
                Elders.Clear();
                foreach (var elder in result)
                {
                    Elders.Add(elder);
                }
            }
            catch (Exception ex)
            {
                // כאן תוכלי להוסיף הודעת שגיאה למשתמש בעתיד
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToProfile(User elder)
        {
            if (elder == null) return;
            await Shell.Current.GoToAsync(nameof(ElderProfilePage), new Dictionary<string, object>
            {
                { "Elder", elder }
            });
        }
    }
}