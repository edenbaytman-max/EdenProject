using EdenProject.ViewModels;

namespace EdenProject.Views
{
    public partial class ParentVerificationPage : ContentPage
    {
        public ParentVerificationPage()
        {
            InitializeComponent();
            BindingContext = new ParentVerificationViewModel();
        }
    }
}