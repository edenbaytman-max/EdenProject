using EdenProject.ViewModels;

namespace EdenProject.Views
{
    public partial class AccountPage : ContentPage
    {
        public AccountPage(AccountPageViewModel viewModel)
        {
            InitializeComponent();

            // הגדרת ה-BindingContext כדי שה-XAML יזהה את הפקודות והשדות
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // בדיקה שה-BindingContext הוא אכן ה-ViewModel הנכון
            if (BindingContext is AccountPageViewModel vm)
            {
                vm.ReloadUser();
            }
        }
    }
}