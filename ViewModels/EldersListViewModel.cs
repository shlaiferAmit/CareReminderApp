using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CareReminderApp.Services;
using CareReminderApp.Models;

namespace CareReminderApp.ViewModels
{
    public partial class EldersListViewModel : ObservableObject
    {
        public ObservableCollection<User> Elders { get; } = new();

        private readonly IDataService _dataService;

        public EldersListViewModel(IDataService dataService, string id)
        {
            _dataService = dataService;
            LoadElders();
        }

        private async void LoadElders()
        {
            var users = await _dataService.GetUsersAsync();
            Elders.Clear();
            foreach (var user in users.Where(u => u.RoleId == "Elder"))
                Elders.Add(user);
        }
    }
}
