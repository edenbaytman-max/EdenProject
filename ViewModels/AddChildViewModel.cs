using EdenProject.Models;
using EdenProject.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace EdenProject.ViewModels
{
    public class AddChildViewModel : ViewModelBase
    {
        private readonly FirebaseService _firebaseService;

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private int _age;
        public int Age
        {
            get => _age;
            set { _age = value; OnPropertyChanged(); }
        }

        public ICommand SaveChildCommand { get; }

        public AddChildViewModel()
        {
            _firebaseService = new FirebaseService();

            // הגדרת הפקודה לשמירה
            SaveChildCommand = new Command(async () => await OnSaveChild());
        }

        private async Task OnSaveChild()
        {
            // 1. בדיקת תקינות בסיסית (ולידציה)
            if (string.IsNullOrWhiteSpace(Name) || Age <= 0)
            {
                await App.Current.MainPage.DisplayAlert("שגיאה", "אנא הזינו שם וגיל תקינים", "אוקיי");
                return;
            }

            // 2. בדיקה שהמשתמש מחובר (למניעת קריסה בשליפת ה-ID)
            if (App.CurrentUser == null)
            {
                await App.Current.MainPage.DisplayAlert("שגיאה", "משתמש לא מחובר. אנא התחבר מחדש.", "אוקיי");
                await Shell.Current.GoToAsync("//SignInPage");
                return;
            }

            // 3. יצירת אובייקט הילד החדש
            var newChild = new Child
            {
                Name = this.Name,
                Age = this.Age,
                ParentId = App.CurrentUser.UserId // מקשר את הילד להורה הנוכחי
            };

            // 4. שמירה ב-Firebase
            bool success = await _firebaseService.AddChildAsync(newChild);

            if (success)
            {
                await App.Current.MainPage.DisplayAlert("הצלחה", $"{Name} נוסף/ה בהצלחה!", "מעולה");

                // 5. חזרה למסך הקודם (ה-MainPage יתעדכן בזכות ה-OnAppearing)
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("שגיאה", "לא הצלחנו לשמור את הפרטים במסד הנתונים", "נסה שוב");
            }
        }
    }
}