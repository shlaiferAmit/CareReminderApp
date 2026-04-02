using Microsoft.Maui.Controls;
using CareReminderApp.ViewModels;
using CareReminderApp.Services;


namespace CareReminderApp.Views
{
    public partial class ElderRemindersPage : ContentPage
    {
        public ElderRemindersPage(ElderRemindersViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // בכל פעם שהדף מופיע (גם בחזרה מניווט), אנחנו מעדכנים את הספירה
            if (BindingContext is ElderRemindersViewModel vm)
            {
                await vm.LoadRemindersAsync();
            }
        }
    }
}