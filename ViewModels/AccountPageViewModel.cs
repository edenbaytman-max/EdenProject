#nullable disable
using EdenProject.Models;
using EdenProject.Service;
using System.Windows.Input;

namespace EdenProject.ViewModels
{
    // השתמשי ב-QueryProperty כדי לקבל את המשתמש שנבחר מה-AdminPage (אם רלוונטי)
    [QueryProperty(nameof(SelectedUser), "SelectedUser")]
    public class AccountPageViewModel : ViewModelBase
    {
        private readonly DBMokup _db;
        private User _selectedUser;

        // המשתמש שמוצג ועובר עריכה
        public User SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        public ICommand SaveChangesCommand { get; }

        public AccountPageViewModel()
        {
            _db = new DBMokup();

            // אם נכנסנו לדף החשבון שלי (לא דרך אדמין), נטען את המשתמש המחובר
            if (App.CurrentUser != null)
            {
                SelectedUser = App.CurrentUser;
            }

            SaveChangesCommand = new Command(async () => await SaveChanges());
        }

        private async Task SaveChanges()
        {
            if (SelectedUser == null) return;

            // בגלל שהרשימה ב-DBMokup היא סטטית והאובייקט הוא Reference, 
            // השינויים כבר קרו באובייקט. אנחנו רק מציגים הודעה וחוזרים.

            await Shell.Current.DisplayAlert("הצלחה", "הפרטים עודכנו בהצלחה", "אישור");
            await Shell.Current.GoToAsync(".."); // חזרה לדף הקודם
        }

        public ICommand NavigateBackCommand => new Command(async () =>
    await Shell.Current.GoToAsync(".."));

        public void ReloadUser()
        {
            // אם SelectedUser כבר קיבל ערך מהאדמין (דרך ה-QueryProperty)
            // אנחנו לא רוצים לדרוס אותו עם המשתמש המחובר!
            if (SelectedUser == null)
            {
                if (App.CurrentUser != null)
                {
                    SelectedUser = App.CurrentUser;
                }
            }

            // בכל מקרה, נודיע ל-XAML להתרענן
            OnPropertyChanged(nameof(SelectedUser));
        }
    }
}