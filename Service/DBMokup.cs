using EdenProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EdenProject.Service
{
    // שנה ל-public כדי שה-ViewModels יוכלו להשתמש במחלקה ללא שגיאות נגישות
    public class DBMokup
    {
        // רשימה סטטית כדי שכל המופעים של ה-Service יראו את אותם הנתונים
        private static List<User> _users = new List<User>();

        public DBMokup()
        {
            // הוספת משתמשים ראשוניים רק אם הרשימה ריקה
            if (_users.Count == 0)
            {
                _users.Add(new User
                {
                    UserEmail = "a",
                    UserPassword = "a",
                    FirstName = "Admin", // הוספתי שם כדי שלא יהיה Hello ריק
                    LastName = "System",
                    IsAdmin = true
                });

                _users.Add(new User
                {
                    UserEmail = "user@test.com",
                    UserPassword = "123",
                    FirstName = "Eden",
                    LastName = "Project",
                    IsAdmin = false
                });
            }
        }

        public bool isExist(string uEmail, string uPass)
        {
            return _users.Any(u => u.UserEmail == uEmail && u.UserPassword == uPass);
        }

        public void AddUser(User newUser)
        {
            if (newUser != null)
            {
                _users.Add(newUser);
            }
        }

        public User GetUser(string email, string pass)
        {
            // מחזיר את המשתמש המלא כולל ה-FirstName וה-IsAdmin
            return _users.FirstOrDefault(u => u.UserEmail == email && u.UserPassword == pass);
        }

        public List<User> GetUsers()
        {
            return _users;
        }

        public void DeleteUser(User user)
        {
            // חיפוש המשתמש לפי אימייל (או מזהה ייחודי אחר) ומחיקתו מהרשימה הסטטית
            var userToRemove = _users.FirstOrDefault(u => u.UserEmail == user.UserEmail);
            if (userToRemove != null)
            {
                _users.Remove(userToRemove);
            }
        }
    }
}