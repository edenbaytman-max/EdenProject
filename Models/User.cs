using System;

namespace EdenProject.Models
{
    public class User
    {
        // מזהה ייחודי שנוצר על ידי Firebase - קריטי למחיקה ועדכון נתונים
        public string? UserId { get; set; }

        public string? UserEmail { get; set; }

        public string? UserPassword { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? UserMobile { get; set; }

        // מגדיר האם המשתמש הוא מנהל (עבור ה-Manager Page)
        public bool IsAdmin { get; set; }

        // תאריך יצירת המשתמש (עוזר לסדר את הרשימה בניהול)
        public DateTime CreatedAt { get; set; }

        // בנאי ברירת מחדל - חובה עבור העבודה עם FirebaseDatabase.net
        public User()
        {
            CreatedAt = DateTime.Now;
            IsAdmin = false; // כברירת מחדל משתמש חדש הוא לא מנהל
        }

        // מאפיין לקבלת השם המלא בקלות (נוח להצגה ב-UI)
        public string FullName => $"{FirstName} {LastName}";
    }
}