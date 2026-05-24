using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EdenProject.Models;

namespace EdenProject.Services
{
    public class AIService
    {
        // מפתח ה-API החינמי שלך מ-Google AI Studio
        private const string ApiKey = "AIzaSyAYZY8Pp4WtVsMDRQa8iPSjm8Jcn5PcYic";
        // שימוש במודל העדכני והמהיר ביותר לשכבה החינמית
        private const string ApiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={ApiKey}";

        public async Task<string> GetAnalysisAsync(List<GameAction> actions, Child childInfo)
        {
            try
            {
                // 1. עיבוד רשימת הפעולות של הגרירה והאימוג'ים לטקסט מסודר עבור ה-AI
                var actionsSummary = new StringBuilder();
                foreach (var action in actions)
                {
                    actionsSummary.AppendLine($"- בשעה {action.Timestamp:HH:mm:ss}: הילד הניח את הדמות '{action.CharacterName}' בתוך החדר '{action.RoomName}'. האווירה/מצב הרוח שנקבע לחדר: {action.Emotion}.");
                }

                using (var client = new HttpClient())
                {
                    // 2. הנחיית מערכת (System Instruction) - חוקי הברזל הפסיכולוגיים של מלאני קליין
                    string systemInstruction = "אתה פסיכולוג קליני מומחה לילדים, המתמחה בתיאוריית יחסי האובייקט של מלאני קליין ובתרפיה במשחק (Play Therapy). " +
                                               "התפקיד שלך הוא לנתח את דפוסי המשחק של הילד על סמך רשימת הפעולות שהוא ביצע בבית בובות וירטואלי, ולספק תובנות עבור ההורה. " +
                                               "חוקים חשובים:\n" +
                                               "1. ענה תמיד בעברית רהוטה, חמה, מכילה ומעצימה.\n" +
                                               "2. אל תפחיד את ההורה במונחים קליניים קשים, אלא הנגש את הרעיונות של קליין (כמו אובייקט טוב/רע, השלכת רגשות, תוקפנות, או תהליכי 'תיקון' - Reparation) בצורה עדינה.\n" +
                                               "3. מבנה התשובה חייב להכיל תמיד כותרות ברורות עבור: סיכום המשחק, תובנות רגשיות לפי מלאני קליין, והמלצה יישומית ותומכת להורה.";

                    // 3. בניית ה-Prompt האישי עם פרטי הילד והיסטוריית המשחק האמיתית
                    string prompt = $"הילד/ה {childInfo.Name}, בן/בת {childInfo.Age}, שיחק/ה בבית בובות אינטראקטיבי.\n" +
                                    $"להלן היסטוריית הפעולות המדויקת מהמשחק:\n{actionsSummary}\n\n" +
                                    $"אנא ספק את הניתוח הקלייניאני המקצועי והתומך בהתאם להנחיות המערכת.";

                    // 4. בניית האובייקט המובנה שה-API של גוגל מצפה לקבל
                    var requestBody = new
                    {
                        contents = new[]
                        {
                            new { parts = new[] { new { text = $"{systemInstruction}\n\n{prompt}" } } }
                        }
                    };

                    string jsonContext = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(jsonContext, Encoding.UTF8, "application/json");

                    // 5. שליחת הבקשה הרשמית לגוגל
                    var response = await client.PostAsync(ApiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();

                        // 6. פירוק ה-JSON שחזר ושליפת הטקסט של הניתוח
                        dynamic json = JsonConvert.DeserializeObject(responseString);
                        string aiAnalysis = json.candidates[0].content.parts[0].text;

                        return aiAnalysis;
                    }

                    return "שגיאה: השרת של ה-AI לא החזיר תשובה תקינה. אנא נסו שוב מאוחר יותר.";
                }
            }
            catch (Exception ex)
            {
                return $"שגיאה בחיבור ל-AI: {ex.Message}";
            }
        }
    }
}