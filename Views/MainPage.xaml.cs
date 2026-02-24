using EdenProject.ViewModels;

namespace EdenProject.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // הוספת פונקציה שקורית בכל פעם שהדף עולה למסך
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // מרענן את הנתונים ב-ViewModel כדי שהשם והרשאות האדמין יופיעו מיד
        if (BindingContext is MainPageViewModel vm)
        {
            vm.RefreshUser();
        }
    }
}