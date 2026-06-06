using CareReminderApp.ViewModels;

namespace CareReminderApp.Views;

// אחראי על חיבור בין ממשק המשתמש לבין מודל התצוגה
public partial class AddReminderPage : ContentPage
{
    public AddReminderPage(AddReminderViewModel vm)
    {
        InitializeComponent();

        // קביעת מודל התצוגה כנתון הקישור של הדף
        BindingContext = vm;
    }
}