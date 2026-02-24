#nullable disable
using EdenProject.Models;
using EdenProject.Service;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EdenProject.ViewModels
{
    public class UsersListPageViewModel : ViewModelBase
    {
        private readonly DBMokup _db;
        private List<User> _allUsers; // רשימה פנימית לסינון (חיפוש)

        public ObservableCollection<User> Users { get; set; }
        public ICommand UserSelectedCommand { get; }

        public UsersListPageViewModel()
        {
            _db = new DBMokup();

            // טעינת הנתונים
            _allUsers = _db.GetUsers();
            Users = new ObservableCollection<User>(_allUsers);

            // פקודת הניווט לדף AccountPage [מקור: סעיף 2 במשימה]
            UserSelectedCommand = new Command<User>(async (user) =>
            {
                if (user != null)
                {
                    await Shell.Current.GoToAsync("AccountPage", new Dictionary<string, object>
                    {
                        { "SelectedUser", user }
                    });
                }
            });
        }
    }
}