#nullable disable
using EdenProject.Models;
using EdenProject.Service;
using System.Windows.Input;

namespace EdenProject.ViewModels
{
    public class SignUpViewModel : ViewModelBase
    {
        private readonly DBMokup _db;

        #region Private Fields
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _password;
        private string _mobile;
        private bool _entryAsPassword;
        private string _passwordIconCode;
        #endregion

        #region Public Properties
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); ((Command)SignUpCommand).ChangeCanExecute(); }
        }
        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); ((Command)SignUpCommand).ChangeCanExecute(); }
        }
        public string UserEmail
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); ((Command)SignUpCommand).ChangeCanExecute(); }
        }
        public string UserPassword
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); ((Command)SignUpCommand).ChangeCanExecute(); }
        }
        public string Mobile
        {
            get => _mobile;
            set { _mobile = value; OnPropertyChanged(); ((Command)SignUpCommand).ChangeCanExecute(); }
        }

        public bool EntryAsPassword
        {
            get => _entryAsPassword;
            set { _entryAsPassword = value; OnPropertyChanged(); }
        }

        public string PasswordIconCode
        {
            get => _passwordIconCode;
            set { _passwordIconCode = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public ICommand ShowPasswordCommand { get; }
        public ICommand SignUpCommand { get; }
        public ICommand SignInCommand { get; }
        #endregion

        public SignUpViewModel()
        {
            _db = new DBMokup();

            // מצב ראשוני
            EntryAsPassword = true;
            PasswordIconCode = "\ue8f5"; // אייקון עין סגורה

            // אתחול פקודות
            ShowPasswordCommand = new Command(TogglePasswordButton);
            SignUpCommand = new Command(SignUp, Validate);
            SignInCommand = new Command(NavigateToSignIn);
        }

        private void TogglePasswordButton()
        {
            EntryAsPassword = !EntryAsPassword;
            PasswordIconCode = EntryAsPassword ? "\ue8f5" : "\ue8f4";
        }

        private bool Validate()
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(UserEmail) &&
                   !string.IsNullOrWhiteSpace(UserPassword) &&
                   !string.IsNullOrWhiteSpace(Mobile);
        }

        private async void SignUp()
        {
            // 1. יצירת אובייקט המשתמש מהשדות בטופס
            var newUser = new User
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                UserEmail = this.UserEmail,
                UserPassword = this.UserPassword,
                // וודאי שהשם במודל תואם (UserMobile או Mobile)
                UserMobile = this.Mobile
            };

            // 2. שמירה ב-Mockup
            _db.AddUser(newUser);

            // --- התיקון הקריטי כאן ---
            // 3. הגדרת המשתמש שנרשם כמשתמש המחובר של האפליקציה
            App.CurrentUser = newUser;
            // -------------------------

            // 4. הצגת הודעת הצלחה
            await Shell.Current.DisplayAlert("הצלחה", $"ברוכים הבאים {FirstName}!", "אישור");

            // 5. ניווט ישיר ל-MainPage
            await Shell.Current.GoToAsync("///MainPage");
        }

        private async void NavigateToSignIn()
        {
            // חזרה לדף ה-Login (אם המשתמש התחרט ורוצה להתחבר)
            await Shell.Current.GoToAsync("//SignInPage");
        }
    }
}