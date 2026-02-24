#nullable disable 
using EdenProject.Service;
using EdenProject.Views;
using System.Windows.Input;
using Microsoft.Maui.Graphics;

namespace EdenProject.ViewModels
{
    public class SignInPageViewModel : ViewModelBase
    {
        private readonly DBMokup _db;
        private bool _entryAsPassword;
        private string _userName;
        private string _password;
        private string _togglePasswordButtonText;
        private string _loginMessage;
        private bool _signInMessageVisible;
        private Color _signInColor;
        private bool _isSignInButtonEnabled;

        public ICommand ShowPasswordCommand { get; }
        public ICommand SignInCommand { get; }
        public ICommand GoToSignUpCommand { get; }

        public SignInPageViewModel()
        {
            _db = new DBMokup();
            EntryAsPassword = true;
            TogglePaswwordButtonText = "\ue8f5";
            SignInMessageVisible = false;
            LoginMessage = string.Empty;
            SignInColor = Colors.Transparent;

            ShowPasswordCommand = new Command(TogglePasswordVisibility);
            SignInCommand = new Command(Button_Clicked, CanSignIn);

            GoToSignUpCommand = new Command(async () =>
                await Shell.Current.GoToAsync(nameof(SignUpPage)));
        }

        #region Properties
        public bool IsSignInButtonEnabled
        {
            get => _isSignInButtonEnabled;
            set { _isSignInButtonEnabled = value; OnPropertyChanged(); }
        }

        public Color SignInColor
        {
            get => _signInColor;
            set { _signInColor = value; OnPropertyChanged(); }
        }

        public bool SignInMessageVisible
        {
            get => _signInMessageVisible;
            set { _signInMessageVisible = value; OnPropertyChanged(); }
        }

        public string LoginMessage
        {
            get => _loginMessage;
            set { _loginMessage = value; OnPropertyChanged(); }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
                UpdateCommandStates();
            }
        }

        public string UserPassword
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                UpdateCommandStates();
            }
        }

        public bool EntryAsPassword
        {
            get => _entryAsPassword;
            set { _entryAsPassword = value; OnPropertyChanged(); }
        }

        public string TogglePaswwordButtonText
        {
            get => _togglePasswordButtonText;
            set { _togglePasswordButtonText = value; OnPropertyChanged(); }
        }
        #endregion

        private void UpdateCommandStates()
        {
            ((Command)SignInCommand).ChangeCanExecute();
            IsSignInButtonEnabled = CanSignIn();
        }

        private void TogglePasswordVisibility()
        {
            EntryAsPassword = !EntryAsPassword;
            TogglePaswwordButtonText = EntryAsPassword ? "\ue8f5" : "\ue8f4";
        }

        private async void Button_Clicked()
        {
            SignInMessageVisible = true;

            // בדיקה האם המשתמש קיים
            if (_db.isExist(UserName, UserPassword))
            {
                // --- תוספת לניהול אדמין ---
                // שולפים את אובייקט המשתמש המלא מה-DB (וודאי שיש לך מתודת GetUser ב-DBMokup)
                var loggedInUser = _db.GetUser(UserName, UserPassword);

                // שומרים את המשתמש במיקום סטטי ב-App כדי שיהיה נגיש לכל האפליקציה
                App.CurrentUser = loggedInUser;
                // -------------------------

                LoginMessage = "התחברת בהצלחה!";
                SignInColor = Colors.Green;

                await Task.Delay(1000);
                await Shell.Current.GoToAsync("///MainPage");
            }
            else
            {
                LoginMessage = "שגיאת התחברות - נתונים שגויים";
                SignInColor = Colors.Red;
            }
        }

        private bool CanSignIn()
        {
            return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(UserPassword);
        }
    }
}