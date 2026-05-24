using EdenProject.ViewModels;

namespace EdenProject.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as MainPageViewModel;
        if (vm != null)
        {
            await vm.LoadChildren(); // הוספת await כדי שיחכה לנתונים
        }
    }
}