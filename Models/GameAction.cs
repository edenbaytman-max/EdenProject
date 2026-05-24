using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdenProject.Models
{
    public class GameAction
    {
        public string CharacterName { get; set; } // למשל: אמא, אריה, הילד עצמו
        public string RoomName { get; set; }      // למשל: חדר הורים, סלון, גינה
        public string Emotion { get; set; }       // למשל: כעס, שמחה, עצב
        public DateTime Timestamp { get; set; }
    }
}
