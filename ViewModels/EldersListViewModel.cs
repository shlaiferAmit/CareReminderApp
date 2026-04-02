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

        // תיקון: הורדנו את ה-string id מה-Constructor כי הוא מפריע להרצה
        public EldersListViewModel(IDataService dataService)
        {
            _dataService = dataService;
            // קריאה לפונקציית הטעינה
            _ = LoadElders();
        }

        private async Task LoadElders()
        {
            var users = await _dataService.GetUsersAsync();
            Elders.Clear();

            // תיקון שגיאה CS0019: השוואה בין int ל-Enum במקום למחרוזת
            // אנחנו בודקים אם ה-RoleId תואם לערך המספרי של Senior
            foreach (var user in users.Where(u => u.RoleId == (int)UserRole.Senior))
            {
                Elders.Add(user);
            }
        }
    }
}
