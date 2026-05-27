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

        // רשימות התצוגה עבור כל חדר בנפרד
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

            DropItemCommand = new Command<Dictionary<string, string>>((data) =>
            {
                if (data == null) return;

                string item = data["Item"];
                string room = data["Room"];
                string type = data["Type"];

                // 1. הוספה ויזואלית לרשימה של החדר
                AddItemToRoomVisual(room, item);

                // 2. לוגיקת הצמדה חכמה עבור ה-AI
                if (type == "Character")
                {
                    var newAction = new GameAction
                    {
                        Timestamp = DateTime.Now,
                        RoomName = room,
                        CharacterName = item,
                        Emotion = "לא נקבע" // יתעדכן אם יגררו רגש על החדר
                    };
                    ActionsTracked.Add(newAction);
                    GameStatusMessage = $"הדמות '{item}' הונחה ב{room}";
                }
                else // מדובר בגרור של Emotion
                {
                    // מחפשים אם יש כבר דמות בחדר הזה שגררו אותה קודם לכן (האחרונה שנכנסה לחדר)
                    var lastActionInRoom = ActionsTracked
                        .Where(a => a.RoomName == room && a.CharacterName != "כללי/אווירה")
                        .LastOrDefault();

                    if (lastActionInRoom != null && lastActionInRoom.Emotion == "לא נקבע")
                    {
                        // מצמידים את הרגש ישירות לדמות הקיימת בחדר!
                        lastActionInRoom.Emotion = item;
                        GameStatusMessage = $"הדמות '{lastActionInRoom.CharacterName}' מרגישה {item} ב{room}";
                    }
                    else
                    {
                        // אם אין דמות בחדר או שהדמויות כבר קיבלו רגש, מייצרים פעולת אווירה כללית לחדר
                        var ambientAction = new GameAction
                        {
                            Timestamp = DateTime.Now,
                            RoomName = room,
                            CharacterName = "כללי/אווירה",
                            Emotion = item
                        };
                        ActionsTracked.Add(ambientAction);
                        GameStatusMessage = $"נקבעה אווירת {item} ב{room}";
                    }
                }
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

            bool confirm = await App.Current.MainPage.DisplayAlert("סיום משחק", "האם לסיים את המשחק ולעבור לאזור ההורים?", "כן", "עוד לא");
            if (!confirm) return;

            try
            {
                var childToSend = CurrentChild ?? new Child { Name = this.ChildName, Age = 5 };

                var navigationParameter = new ShellNavigationQueryParameters
                {
                    { "GameActions", ActionsTracked.ToList() },
                    { "ChildDetails", childToSend }
                };

                await Shell.Current.GoToAsync("ParentVerificationPage", navigationParameter);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("שגיאה", "חלה בעיה במעבר לעמוד האימות: " + ex.Message, "סגור");
            }
        }
    }
}