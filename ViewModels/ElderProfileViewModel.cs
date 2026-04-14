using CareReminderApp.Models;
using CareReminderApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CareReminderApp.Views;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareReminderApp.ViewModels
{
    [QueryProperty(nameof(Elder), "Elder")]
    public partial class ElderProfileViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        public ElderProfileViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Reminders = new ObservableCollection<Reminder>();
        }

        [ObservableProperty]
        private User elder;

        [ObservableProperty]
        private ObservableCollection<Reminder> reminders;

        partial void OnElderChanged(User value)
        {
            if (value != null)
            {
                LoadRemindersCommand.Execute(null);
            }
        }

        [RelayCommand]
        public async Task LoadReminders()
        {
            var result = await _dataService.GetRemindersAsync(Elder.Id);

            Reminders.Clear();
            foreach (var r in result)
                Reminders.Add(r);
        }

        [RelayCommand]
        private async Task AddReminder()
        {
            await Shell.Current.GoToAsync(nameof(AddReminderPage), new Dictionary<string, object>
            {
                { "UserId", Elder.Id }
            });
        }
    }
}