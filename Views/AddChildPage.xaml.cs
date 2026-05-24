using EdenProject.ViewModels;

namespace EdenProject.Views
{
    public partial class AddChildPage : ContentPage
    {
        public AddChildPage()
        {
            InitializeComponent();
            BindingContext = new AddChildViewModel();
        }
    }
}