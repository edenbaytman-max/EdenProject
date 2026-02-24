using EdenProject.Models;

namespace EdenProject;

public partial class App : Application
{
    // We inject the 'shell' which was already built by MauiProgram

    public static User CurrentUser { get; set; }
    public App(AppShell shell)
    {
        InitializeComponent();

        MainPage = shell;
    }

   
}