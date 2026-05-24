using EdenProject.Views; // וודאי שה-Namespace נכון

namespace EdenProject;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // רישום כל דפי המשנה של האפליקציה
        Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
        Routing.RegisterRoute(nameof(AccountPage), typeof(AccountPage));
        Routing.RegisterRoute(nameof(AdminPage), typeof(AdminPage));
        Routing.RegisterRoute(nameof(UsersListPage), typeof(UsersListPage));
        Routing.RegisterRoute(nameof(AnalysisResultsPage), typeof(AnalysisResultsPage));
        Routing.RegisterRoute(nameof(AddChildPage), typeof(AddChildPage));
        Routing.RegisterRoute(nameof(DollHousePage), typeof(DollHousePage));

    }
}