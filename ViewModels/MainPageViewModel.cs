using EdenProject.Models;
using EdenProject.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EdenProject.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly FirebaseService _firebaseService;

        // רשימת הילדים שתוצג ב-CollectionView
        public ObservableCollection<Child> ChildrenList { get; set; } = new ObservableCollection<Child>();

        private string _helloMessage;
        public string HelloMessage { get => _helloMessage; set { _helloMessage = value; OnPropertyChanged(); } }

        // הילד שנבחר מהרשימה
        private Child _selectedChild;
        public Child SelectedChild
        {
            get => _selectedChild;
            set
            {
                _selectedChild = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsChildSelected));
            }
        }

        // האם כפתור "התחל משחק" פעיל
        public bool IsChildSelected => SelectedChild != null;

        public bool IsAdmin { get; set; } // יתעדכן לפי המשתמש המחובר

        public ICommand AddChildCommand { get; }
        public ICommand StartGameCommand { get; }
        public ICommand SignOutCommand { get; }
        public ICommand NavigateCommand { get; }

        public MainPageViewModel()
        {
            _firebaseService = new FirebaseService();

            // הגדרת פקודות
            AddChildCommand = new Command(async () => await Shell.Current.GoToAsync("AddChildPage"));

            StartGameCommand = new Command(async () =>
            {
                // מעבר למסך המשחק עם הילד שנבחר
                await Shell.Current.GoToAsync($"DollHousePage?childId={SelectedChild.Id}");
            });

            SignOutCommand = new Command(async () => await Shell.Current.GoToAsync("//SignInPage"));

            NavigateCommand = new Command<string>(async (page) => await Shell.Current.GoToAsync($"//{page}"));

            // טעינה ראשונית של הנתונים
            LoadChildren();
        }

        public async Task LoadChildren() // שיניתי ל-Task במקום void כדי שנוכל לחכות לזה
        {
            // בדיקה מי המשתמש המחובר באמת
            if (App.CurrentUser == null) return;

            string currentParentId = App.CurrentUser.UserId;

            var children = await _firebaseService.GetChildrenByParent(currentParentId);

            ChildrenList.Clear();
            foreach (var child in children)
            {
                ChildrenList.Add(child);
            }

            HelloMessage = $"שלום, {App.CurrentUser.FirstName}";
        }
    }
}