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
    public partial class FamilyDashboardViewModel : ObservableObject
    {
        private readonly IDataService? _dataService;

        // ה-Constructor הריק שמונע שגיאות ב-XAML
        public FamilyDashboardViewModel()
        {
        }

        // ה-Constructor שמקבל את השירות (הוסיפי פרמטרים אם את מעבירה יותר מזה)
        public FamilyDashboardViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }
    }
}