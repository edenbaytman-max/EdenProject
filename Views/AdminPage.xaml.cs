using EdenProject.ViewModels;

namespace EdenProject.Views;

public partial class AdminPage : ContentPage
{
    // בנאי עבור ה-Shell
    public AdminPage() : this(IPlatformApplication.Current.Services.GetService<AdminPageViewModel>())
    {
    }

    // בנאי עבור ה-Dependency Injection
    public AdminPage(AdminPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // קריאה לפונקציית הריענון ב-ViewModel בכל פעם שהדף נפתח
        if (BindingContext is AdminPageViewModel vm)
        {
            vm.RefreshUsersList();
        }
    }
}