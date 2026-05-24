using EdenProject.Models;
using EdenProject.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace EdenProject.ViewModels
{
    [QueryProperty(nameof(ChildId), "childId")]
    public class DollHouseViewModel : ViewModelBase
    {
        private readonly FirebaseService _firebaseService;

        public ObservableCollection<GameAction> ActionsTracked { get; set; } = new ObservableCollection<GameAction>();

        // רשימות התצוגה עבור כל חדר בנפרד (כדי שהפריטים יישארו בחדר ויזואלית)
        public ObservableCollection<string> ParentsRoomItems { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> ChildsRoomItems { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> KitchenItems { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> LivingRoomItems { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> GardenItems { get; set; } = new ObservableCollection<string>();

        // רשימות עבור הסרגלים
        public List<string> Characters { get; } = new List<string> { "הילד", "אמא", "אבא", "אח", "אחות", "סבא", "סבתא", "כלב", "חתול", "אריה", "כריש" };
        public List<string> Emotions { get; } = new List<string> { "שמחה 😃", "עצב 😢", "כעס 😡", "פחד 😨", "אהבה 🥰", "קנאה 💚", "רוגע 😌", "סערה ⛈️" };

        private string? _childId;
        public string? ChildId
        {
            get => _childId;
            set
            {
                _childId = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(_childId))
                {
                    Task.Run(async () => await LoadChildDetails());
                }
            }
        }

        private Child? _currentChild;
        public Child? CurrentChild { get => _currentChild; set { _currentChild = value; OnPropertyChanged(); } }

        private string _childName = "הילד";
        public string ChildName { get => _childName; set { _childName = value; OnPropertyChanged(); } }

        private string _gameStatusMessage = "גררו דמויות ומצבי רוח אל חדרי הבית!";
        public string GameStatusMessage { get => _gameStatusMessage; set { _gameStatusMessage = value; OnPropertyChanged(); } }

        public ICommand DropItemCommand { get; }
        public ICommand EndGameCommand { get; }

        public DollHouseViewModel()
        {
            _firebaseService = new FirebaseService();

            // פקודה אחת חכמה שמטפלת גם בגרירת דמות וגם בגרירת רגש
            DropItemCommand = new Command<Dictionary<string, string>>((data) =>
            {
                if (data == null) return;

                string item = data["Item"]; // יכול להיות דמות או אימוג'י
                string room = data["Room"];
                string type = data["Type"]; // "Character" או "Emotion"

                // 1. הוספה ויזואלית לרשימה של החדר המתאים
                AddItemToRoomVisual(room, item);

                // 2. תיעוד הפעולה עבור ה-AI של גוגל
                var newAction = new GameAction
                {
                    Timestamp = DateTime.Now,
                    RoomName = room
                };

                if (type == "Character")
                {
                    newAction.CharacterName = item;
                    newAction.Emotion = "לא נקבע";
                    GameStatusMessage = $"הדמות '{item}' הונחה ב{room}";
                }
                else // Emotion
                {
                    newAction.CharacterName = "כללי/אווירה";
                    newAction.Emotion = item;
                    GameStatusMessage = $"נקבעה אווירת {item} ב{room}";
                }

                ActionsTracked.Add(newAction);
            });

            EndGameCommand = new Command(async () => await OnEndGame());
        }

        private void AddItemToRoomVisual(string room, string item)
        {
            switch (room)
            {
                case "חדר הורים": ParentsRoomItems.Add(item); break;
                case "חדר ילד": ChildsRoomItems.Add(item); break;
                case "מטבח": KitchenItems.Add(item); break;
                case "סלון": LivingRoomItems.Add(item); break;
                case "גינה": GardenItems.Add(item); break;
            }
        }

        private async Task LoadChildDetails()
        {
            if (string.IsNullOrEmpty(ChildId)) return;
            try
            {
                var children = await _firebaseService.GetChildrenByParent(App.CurrentUser?.UserId ?? "");
                CurrentChild = children.FirstOrDefault(c => c.Id == ChildId);
                if (CurrentChild != null)
                {
                    ChildName = CurrentChild.Name;
                    GameStatusMessage = $"המשחק של {ChildName} התחיל! גררו פריטים לבית.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading child details: {ex.Message}");
            }
        }

        private async Task OnEndGame()
        {
            if (ActionsTracked.Count == 0)
            {
                await App.Current.MainPage.DisplayAlert("KleinPlay AI", "הילד עדיין לא שיחק בבית.", "הבנתי");
                return;
            }

            bool confirm = await App.Current.MainPage.DisplayAlert("סיום משחק", "האם לסיים ולשלוח את הניתוח ל-AI?", "כן, נתח", "עוד לא");
            if (!confirm) return;

            try
            {
                var aiService = new AIService();
                var childToSend = CurrentChild ?? new Child { Name = this.ChildName, Age = 5 };
                string analysisResult = await aiService.GetAnalysisAsync(ActionsTracked.ToList(), childToSend);

                await Shell.Current.GoToAsync($"AnalysisResultsPage?analysisText={Uri.EscapeDataString(analysisResult)}");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("שגיאה", "חלה בעיה בייצור הניתוח: " + ex.Message, "סגור");
            }
        }
    }
}