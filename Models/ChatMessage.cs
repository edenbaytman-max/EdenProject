using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdenProject.Models
{
    public class ChatMessage
    {
        public string? Text { get; set; }
        public bool IsAi { get; set; } // true ל-AI, false להורה
        public DateTime Timestamp { get; set; }
    }
}
