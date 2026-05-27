using EdenProject.Models;
using EdenProject.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace EdenProject.ViewModels
{
    [QueryProperty(nameof(AnalysisText), "analysisText")]
    public class AnalysisResultsViewModel : ViewModelBase
    {
        private readonly AIService _aiService;

        public ObservableCollection<ChatMessage> ChatHistory { get; set; } = new ObservableCollection<ChatMessage>();

        private string _userQuestion = string.Empty;
        public string UserQuestion
        {
            get => _userQuestion;
            set { _userQuestion = value; OnPropertyChanged(); }
        }

        private string _analysisText = string.Empty;
        public string AnalysisText
        {
            get => _analysisText;
            set
            {
                // ברגע שהטקסט מגיע מהמסך הקודם, אנחנו מפענחים אותו ומכניסים כהודעה הראשונה של ה-AI
                _analysisText = Uri.UnescapeDataString(value ?? "");
                OnPropertyChanged();

                if (!string.IsNullOrEmpty(_analysisText))
                {
                    ChatHistory.Clear();
                    ChatHistory.Add(new ChatMessage { Text = _analysisText, IsAi = true });
                }
            }
        }

        public ICommand SendQuestionCommand { get; }

        public AnalysisResultsViewModel()
        {
            _aiService = new AIService();
            SendQuestionCommand = new Command(async () => await OnSendQuestion());
        }

        private async Task OnSendQuestion()
        {
            if (string.IsNullOrWhiteSpace(UserQuestion)) return;

            string currentQuestion = UserQuestion;
            UserQuestion = string.Empty; // איפוס תיבת הטקסט מיד

            // 1. הוספת שאלת ההורה לצ'אט
            ChatHistory.Add(new ChatMessage { Text = currentQuestion, IsAi = false });

            try
            {
                // 2. הוספת הודעת טעינה זמנית
                var loadingMessage = new ChatMessage { Text = "ה-AI חושב על תשובה... ⏳", IsAi = true };
                ChatHistory.Add(loadingMessage);

                // 3. שליחה ל-Gemini (העברת כל ההיסטוריה או רק השאלה הנוכחית עם קונטקסט)
                // יצרנו פונקציה ייעודית ב-AIService להמשך הצ'אט עם ההורה
                string aiResponse = await _aiService.GetFollowUpAnswerAsync(AnalysisText, currentQuestion);

                // 4. הסרת הודעת הטעינה והוספת התשובה האמיתית
                ChatHistory.Remove(loadingMessage);
                ChatHistory.Add(new ChatMessage { Text = aiResponse, IsAi = true });
            }
            catch (Exception ex)
            {
                ChatHistory.Add(new ChatMessage { Text = $"שגיאה בקבלת תשובה: {ex.Message}", IsAi = true });
            }
        }
    }
}