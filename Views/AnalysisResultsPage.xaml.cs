using EdenProject.ViewModels;
using Microsoft.Maui.Controls;

namespace EdenProject.Views;

public partial class AnalysisResultsPage : ContentPage
{
    public AnalysisResultsPage()
    {
        InitializeComponent();
        // קישור חיוני ללוגיקת ה-ViewModel החדשה של עמוד זה!
        BindingContext = new AnalysisResultsViewModel();
    }
}