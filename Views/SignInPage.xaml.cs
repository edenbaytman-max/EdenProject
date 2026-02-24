using EdenProject.ViewModels;

namespace EdenProject.Views;

public partial class SignInPage : ContentPage
{
    // 1. The Backup Constructor (Shell uses this)
    public SignInPage() : this(IPlatformApplication.Current.Services.GetService<SignInPageViewModel>())
    {
    }

    // 2. The Real Constructor (DI uses this)
    public SignInPage(SignInPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}