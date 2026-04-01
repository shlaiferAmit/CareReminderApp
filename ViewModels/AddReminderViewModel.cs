using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CareReminderApp.Services;
using CareReminderApp.Models;

namespace CareReminderApp.ViewModels
{
    public partial class AddReminderViewModel : ObservableObject
    {
        [ObservableProperty] private string title;
        [ObservableProperty] private string description;
        [ObservableProperty] private DateTime reminderDate = DateTime.Now;

        private readonly IDataService _dataService;
        private string _userId;

        public AddReminderViewModel(IDataService dataService, string userId)
        {
            _dataService = dataService;
            _userId = userId;
        }



        [RelayCommand]
        private async Task AddReminderAsync()
        {
            var reminder = new Reminder
            {
                Id = Guid.NewGuid().ToString(),
                UserId = _userId,
                Title = this.Title,
                Description = this.Description,
                ReminderDate = this.ReminderDate,
                IsCompleted = false
            };
            await _dataService.AddReminderAsync(reminder);
        }
    }
}
