using EdenProject.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EdenProject.ViewModels
{
    [QueryProperty(nameof(AnalysisText), "analysisText")]
    public class AnalysisResultsViewModel : ViewModelBase
    {
        public ObservableCollection<ChatMessage> ChatHistory { get; set; } = new ObservableCollection<ChatMessage>();

        private string? _analysisText;
        public string? AnalysisText
        {
            get => _analysisText;
            set
            {
                // פענוח הטקסט מה-URL והוספתו לצ'אט
                _analysisText = Uri.UnescapeDataString(value ?? "");
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(_analysisText))
                {
                    ChatHistory.Clear();
                    ChatHistory.Add(new ChatMessage { Text = _analysisText, IsAi = true, Timestamp = DateTime.Now });
                }
            }
        }

        private string? _userQuestion;
        public string? UserQuestion { get => _userQuestion; set { _userQuestion = value; OnPropertyChanged(); } }

        public ICommand SendQuestionCommand { get; }

        public AnalysisResultsViewModel()
        {
            SendQuestionCommand = new Command(async () => await OnSendQuestion());
        }

        private async Task OnSendQuestion()
        {
            if (string.IsNullOrWhiteSpace(UserQuestion)) return;

            ChatHistory.Add(new ChatMessage { Text = UserQuestion, IsAi = false, Timestamp = DateTime.Now });
            string currentQ = UserQuestion;
            UserQuestion = string.Empty;

            await Task.Delay(1500);
            ChatHistory.Add(new ChatMessage
            {
                Text = $"זו שאלה חשובה לגבי {currentQ}. מנקודת המבט של קליין, מומלץ להמשיך לעקוב אחרי המשחק בבית הבובות.",
                IsAi = true,
                Timestamp = DateTime.Now
            });
        }
    }
}