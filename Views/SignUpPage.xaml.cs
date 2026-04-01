using Microsoft.Maui.Controls;
using CareReminderApp.ViewModels;
using CareReminderApp.Services;



namespace CareReminderApp.Views
{
    public partial class SignUpPage : ContentPage
    {
        // זה מה שיפתור את השגיאה בתמונה 9
        public SignUpPage(IDataService dataService)
        {
            InitializeComponent();
            BindingContext = new SignUpPageViewModel(dataService);
        }

        // בנאי ריק כגיבוי ל-Shell
        public SignUpPage()
        {
            InitializeComponent();
        }
    }
}