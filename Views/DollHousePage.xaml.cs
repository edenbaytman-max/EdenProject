using EdenProject.ViewModels;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

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
            var recognizer = (DragGestureRecognizer)sender;

            // שליפת מחרוזת הנתונים שהוגדרה ב-XAML
            if (recognizer.DragStartingCommandParameter is string fullData)
            {
                e.Data.Properties.Add("FullData", fullData);
            }
        }

        private void DropGestureRecognizer_Drop(object sender, DropEventArgs e)
        {
            if (!e.Data.Properties.ContainsKey("FullData")) return;

            string fullData = (string)e.Data.Properties["FullData"];

            var recognizer = (DropGestureRecognizer)sender;
            string room = recognizer.AutomationId;

            string[] parts = fullData.Split(':');
            if (parts.Length < 2) return;

            string type = parts[0];
            string item = parts[1];

            var data = new Dictionary<string, string>
            {
                { "Item", item },
                { "Room", room },
                { "Type", type }
            };

            if (BindingContext is DollHouseViewModel viewModel)
            {
                viewModel.DropItemCommand.Execute(data);
            }
        }
    }
}