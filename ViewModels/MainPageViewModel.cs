#nullable disable
using EdenProject.Models;
using System.Windows.Input;

namespace EdenProject.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        // Property להודעת ברוכים הבאים - מושך נתונים מהמשתמש הסטטי ב-App
        public string HelloMessage
        {
            get
            {
                if (App.CurrentUser != null && !string.IsNullOrEmpty(App.CurrentUser.FirstName))
                {
                    return $"Hello {App.CurrentUser.FirstName}";
                }
                return "Hello Guest";
            }
        }

        // בדיקה האם המשתמש הוא אדמין (לצורך הצגת/הסתרת כפתור הניהול)
        public bool IsAdmin => App.CurrentUser?.IsAdmin ?? false;

        #region Commands
        public ICommand NavigateCommand { get; }
        public ICommand SignOutCommand { get; }
        #endregion

        public MainPageViewModel()
        {
            // פקודת ניווט כללית לפי שם ה-Route שנשלח מה-XAML
            NavigateCommand = new Command<string>(async (route) =>
            {
                if (!string.IsNullOrEmpty(route))
                {
                    // אם מנסים לנווט ל-MainPage, נשתמש בנתיב מוחלט כדי למנוע את השגיאה
                    if (route == "MainPage")
                    {
                        await Shell.Current.GoToAsync("///MainPage");
                        return;
                    }

                    // בדיקת אדמין (כפי שהיה לך)
                    if (route == "AdminPage" && !IsAdmin)
                    {
                        await Shell.Current.DisplayAlert("שגיאה", "אין לך הרשאות ניהול", "סגור");
                        return;
                    }

                    // ניווט רגיל לשאר הדפים
                    await Shell.Current.GoToAsync(route);
                }
            });

            // פקודת התנתקות
            SignOutCommand = new Command(async () =>
            {
                bool confirm = await Shell.Current.DisplayAlert("התנתקות", "האם אתה בטוח שברצונך לצאת?", "כן", "לא");
                if (confirm)
                {
                    App.CurrentUser = null; // מנקה את פרטי המשתמש מהזיכרון
                    await Shell.Current.GoToAsync("///SignInPage"); // חוזר לדף ההתחברות ומאפס את המחסנית
                }
            });
        }

        // מתודה שניתן לקרוא لها מה-Code Behind (OnAppearing) כדי לרענן את השם
        public void RefreshUser()
        {
            OnPropertyChanged(nameof(HelloMessage));
            OnPropertyChanged(nameof(IsAdmin));
        }
    }
}