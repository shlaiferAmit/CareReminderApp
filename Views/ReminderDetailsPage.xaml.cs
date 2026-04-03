using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    // השם כאן חייב להיות זהה ל-x:Class ב-XAML
    public partial class ReminderDetailsPage : ContentPage
    {
        public ReminderDetailsPage(ReminderDetailsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}