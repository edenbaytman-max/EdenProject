using EdenProject.ViewModels;

namespace EdenProject.Views;

public partial class UsersListPage : ContentPage
{
    public UsersListPage() : this(IPlatformApplication.Current.Services.GetService<UsersListPageViewModel>())
    {
    }

    public UsersListPage(UsersListPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}