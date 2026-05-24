using EdenProject.Models;
using EdenProject.Services;
using System.Windows.Input;

namespace EdenProject.ViewModels
{
    public class SignInPageViewModel : ViewModelBase
    {
        private readonly FirebaseService _firebaseService;

        // שדות Binding - שמות תואמים בדיוק ל-XAML שלך
        private string _userName;
        public string UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsSignInButtonEnabled)); }
        }

        private string _userPassword;
        public string UserPassword
        {
            get => _userPassword;
            set { _userPassword = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsSignInButtonEnabled)); }
        }

        private bool _entryAsPassword = true;
        public bool EntryAsPassword { get => _entryAsPassword; set { _entryAsPassword = value; OnPropertyChanged(); } }

        private string _togglePaswwordButtonText = "visibility";
        public string TogglePaswwordButtonText { get => _togglePaswwordButtonText; set { _togglePaswwordButtonText = value; OnPropertyChanged(); } }

        private string _loginMessage;
        public string LoginMessage { get => _loginMessage; set { _loginMessage = value; OnPropertyChanged(); } }

        private bool _signInMessageVisible;
        public bool SignInMessageVisible { get => _signInMessageVisible; set { _signInMessageVisible = value; OnPropertyChanged(); } }

        private Color _signInColor = Colors.White;
        public Color SignInColor { get => _signInColor; set { _signInColor = value; OnPropertyChanged(); } }

        // בדיקה האם הכפתור פעיל
        public bool IsSignInButtonEnabled => !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(UserPassword);

        public ICommand SignInCommand { get; }
        public ICommand ShowPasswordCommand { get; }
        public ICommand GoToSignUpCommand { get; }

        public SignInPageViewModel()
        {
            _firebaseService = new FirebaseService();

            SignInCommand = new Command(async () => await OnSignIn());

            ShowPasswordCommand = new Command(() =>
            {
                EntryAsPassword = !EntryAsPassword;
                TogglePaswwordButtonText = EntryAsPassword ? "visibility" : "visibility_off";
            });

            GoToSignUpCommand = new Command(async () => await Shell.Current.GoToAsync("SignUpPage"));
        }

        private async Task OnSignIn()
        {
            SignInMessageVisible = true;
            LoginMessage = "מתחבר...";
            SignInColor = Colors.White;

            // שימוש ב-UserName (שב-XAML הוא האימייל)
            var user = await _firebaseService.LoginUser(UserName, UserPassword);

            if (user != null)
            {
                if (user.IsAdmin)
                    await Shell.Current.GoToAsync("//ManagerPage");
                else
                    App.CurrentUser = user;
                    await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                SignInColor = Colors.Red;
                LoginMessage = "אימייל או סיסמה שגויים";
            }
        }
    }
}