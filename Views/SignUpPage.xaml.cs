using Microsoft.Maui.Controls;
using CareReminderApp.ViewModels;
using CareReminderApp.Services;



namespace CareReminderApp.Views
{
    public partial class SignUpPage : ContentPage
    {
        // הדרך הנכונה: מקבלים את ה-ViewModel מוכן מהמערכת
        public SignUpPage(SignUpPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        // אין צורך בבנאי ריק אם רשמת את הדף ב-MauiProgram.cs
    }
}