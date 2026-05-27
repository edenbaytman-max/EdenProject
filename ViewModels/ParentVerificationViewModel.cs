using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using EdenProject.Models;
using EdenProject.Services;

namespace EdenProject.ViewModels
{
    public class ParentVerificationViewModel : ViewModelBase, IQueryAttributable
    {
        // נתונים שמתקבלים מעמוד המשחק
        private List<GameAction> _gameActions;
        private Child _childDetails;

        // פרופרטיז לתצוגה
        private string _passwordInput;
        public string PasswordInput
        {
            get => _passwordInput;
            set { _passwordInput = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); IsErrorVisible = !string.IsNullOrEmpty(value); }
        }

        private bool _isErrorVisible;
        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set { _isErrorVisible = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand VerifyPasswordCommand { get; }
        public ICommand CancelCommand { get; }

        public ParentVerificationViewModel()
        {
            VerifyPasswordCommand = new Command(async () => await OnVerifyPassword());
            CancelCommand = new Command(async () => await OnCancel());
        }

        // פונקציית קבלת הפרמטרים מהניווט
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("GameActions"))
            {
                _gameActions = query["GameActions"] as List<GameAction>;
            }
            if (query.ContainsKey("ChildDetails"))
            {
                _childDetails = query["ChildDetails"] as Child;
            }
        }

        private async Task OnVerifyPassword()
        {
            ErrorMessage = "";

            if (string.IsNullOrWhiteSpace(PasswordInput))
            {
                ErrorMessage = "אנא הזן סיסמה.";
                return;
            }

            // שליפת הסיסמה האמיתית של המשתמש המחובר כעת באפליקציה
            string correctPassword = App.CurrentUser?.UserPassword;

            // בדיקת גיבוי למקרה שהאובייקט זמנית ריק בזמן הבדיקות שלך
            if (string.IsNullOrEmpty(correctPassword))
            {
                correctPassword = "admin"; // סיסמת ברירת מחדל לבדיקות במעבדה
            }

            if (PasswordInput != correctPassword)
            {
                ErrorMessage = "סיסמה שגויה! גישת הורים נדחתה.";
                return;
            }

            // אם הסיסמה נכונה - נפעיל את ה-AI תחת מסך טעינה
            IsBusy = true;
            try
            {
                var aiService = new AIService();
                string analysisResult = await aiService.GetAnalysisAsync(_gameActions, _childDetails);

                // ניווט לעמוד התוצאות הסופי עם הניתוח המלא שהתקבל
                await Shell.Current.GoToAsync($"AnalysisResultsPage?analysisText={Uri.EscapeDataString(analysisResult)}");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("שגיאה", "חלה שגיאה בעיבוד הנתונים מול השרת: " + ex.Message, "סגור");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnCancel()
        {
            // חזרה צעד אחד אחורה לבית הבובות
            await Shell.Current.GoToAsync("..");
        }
    }
}