using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    public partial class EldersListPage : ContentPage
    {
        public EldersListPage(EldersListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // הפעלת פקודת הטעינה בכל פעם שהעמוד מופיע
            if (BindingContext is EldersListViewModel viewModel)
            {
                await viewModel.LoadEldersCommand.ExecuteAsync(null);
            }
        }
    }
}