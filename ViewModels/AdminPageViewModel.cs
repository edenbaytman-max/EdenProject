#nullable disable
using EdenProject.Models;
using EdenProject.Service;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EdenProject.ViewModels
{
    public class AdminPageViewModel : ViewModelBase
    {
        private readonly DBMokup _db;

        #region Properties
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set { _users = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public ICommand UserSelectedCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand ViewUsersCommand => new Command(() => RefreshUsersList());
        #endregion

        public AdminPageViewModel()
        {
            _db = new DBMokup();
            RefreshUsersList();

            // פקודה לעריכת משתמש
            UserSelectedCommand = new Command<User>(async (user) =>
            {
                if (user != null)
                {
                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "SelectedUser", user }
                    };
                    await Shell.Current.GoToAsync("AccountPage", navigationParameter);
                }
            });

            // פקודה למחיקת משתמש - הועברה אל תוך הבנאי
            DeleteUserCommand = new Command<User>(async (user) =>
            {
                if (user == null) return;

                bool confirm = await Shell.Current.DisplayAlert("מחיקת משתמש",
                    $"האם אתה בטוח שברצונך למחוק את {user.FirstName} {user.LastName}?", "כן", "לא");

                if (confirm)
                {
                    _db.DeleteUser(user);
                    RefreshUsersList();
                    await Shell.Current.DisplayAlert("הצלחה", "המשתמש נמחק בהצלחה", "אישור");
                }
            });
        }

        // פונקציה שמושכת את כל המשתמשים מה-DB המשותף
        public void RefreshUsersList()
        {
            var allUsers = _db.GetUsers();
            Users = new ObservableCollection<User>(allUsers);
        }
    }
}