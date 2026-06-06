// אחראי על הצגת ממשק העריכה וחיבורו למודל התצוגה

using CareReminderApp.ViewModels;

namespace CareReminderApp.Views
{
    public partial class ChangeProfilePage : ContentPage
    {
        public ChangeProfilePage(ChangeProfileViewModel viewModel)
        {
            InitializeComponent();

            // קביעת מודל התצוגה כנתון הקישור של הדף
            BindingContext = viewModel;
        }
    }
}