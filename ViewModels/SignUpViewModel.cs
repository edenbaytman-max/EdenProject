using EdenProject.Models;
using EdenProject.Services;
using System.Windows.Input;

namespace EdenProject.ViewModels
{
    public class SignUpViewModel : ViewModelBase
    {
        private readonly FirebaseService _firebaseService;

        // שדות Binding
        private string _firstName;
        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(); } }

        private string _lastName;
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(); } }

        private string _userEmail;
        public string UserEmail { get => _userEmail; set { _userEmail = value; OnPropertyChanged(); } }

        private string _userMobile;
        public string UserMobile { get => _userMobile; set { _userMobile = value; OnPropertyChanged(); } }

        private string _userPassword;
        public string UserPassword { get => _userPassword; set { _userPassword = value; OnPropertyChanged(); } }

        private bool _entryAsPassword = true;
        public bool EntryAsPassword { get => _entryAsPassword; set { _entryAsPassword = value; OnPropertyChanged(); } }

        private string _passwordIconCode = "👁"; // או קוד ה-Material שלך
        public string PasswordIconCode { get => _passwordIconCode; set { _passwordIconCode = value; OnPropertyChanged(); } }

        public ICommand SignUpCommand { get; }
        public ICommand ShowPasswordCommand { get; }
        public ICommand SignInCommand { get; }

        public SignUpViewModel()
        {
            _firebaseService = new FirebaseService();

            SignUpCommand = new Command(async () => await OnSignUp());

            ShowPasswordCommand = new Command(() =>
            {
                EntryAsPassword = !EntryAsPassword;
                PasswordIconCode = EntryAsPassword ? "👁" : "🔒";
            });

            SignInCommand = new Command(async () => await Shell.Current.GoToAsync("//SignInPage"));
        }

        private async Task OnSignUp()
        {
            // בדיקה שכל השדות מלאים
            if (string.IsNullOrEmpty(UserEmail) || string.IsNullOrEmpty(UserPassword) || string.IsNullOrEmpty(FirstName))
            {
                await App.Current.MainPage.DisplayAlert("שגיאה", "אנא מלאי את כל השדות החשובים", "אוקיי");
                return;
            }

            var newUser = new User
            {
                UserEmail = this.UserEmail,
                UserPassword = this.UserPassword,
                FirstName = this.FirstName,
                LastName = this.LastName,
                UserMobile = this.UserMobile,
                IsAdmin = false
            };

            var success = await _firebaseService.RegisterUser(newUser);

            if (success)
            {
                await App.Current.MainPage.DisplayAlert("הצלחה", "נרשמת בהצלחה למערכת KleinPlay AI", "מעולה");

                // כאן הניווט לדף הראשי (MainPage) תוך איפוס המחסנית (Stack)
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("שגיאה", "לא הצלחנו לשמור את הנתונים ב-Firebase", "נסה שוב");
            }

        }
    }
}