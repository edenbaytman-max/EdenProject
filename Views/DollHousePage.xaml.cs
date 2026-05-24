using EdenProject.ViewModels;

namespace EdenProject.Views
{
    public partial class DollHousePage : ContentPage
    {
        public DollHousePage()
        {
            InitializeComponent();
            BindingContext = new DollHouseViewModel();
        }

        private void DragGestureRecognizer_DragStarting(object sender, DragStartingEventArgs e)
        {
            // מקבל את הפרמטר שכתבנו ב-XAML (למשל "Character:אמא" או "Emotion:כעס")
            string fullData = (string)((DragGestureRecognizer)sender).CommandParameter;
            e.Data.Package.Properties.Add("FullData", fullData);
        }

        private void DropGestureRecognizer_Drop(object sender, DropEventArgs e)
        {
            if (!e.Data.Package.Properties.ContainsKey("FullData")) return;

            string fullData = (string)e.Data.Package.Properties["FullData"];
            string room = (string)((DropGestureRecognizer)sender).CommandParameter;

            // פיצול הנתונים לפי הנקודתיים
            string[] parts = fullData.Split(':');
            string type = parts[0];  // "Character" או "Emotion"
            string item = parts[1];  // השם של הדמות או האימוג'י עצמו

            var data = new Dictionary<string, string>
            {
                { "Item", item },
                { "Room", room },
                { "Type", type }
            };

            // הפעלת הפקודה המעודכנת ב-ViewModel
            ((DollHouseViewModel)BindingContext).DropItemCommand.Execute(data);
        }
    }
}