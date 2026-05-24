using Firebase.Database;
using Firebase.Database.Query;
using EdenProject.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace EdenProject.Services
{
    public class FirebaseService
    { 
        // הכתובת ששלחת לי
        private const string FirebaseUrl = "https://kleinplayai-default-rtdb.firebaseio.com/";
        private readonly FirebaseClient client;

        public FirebaseService()
        {
            client = new FirebaseClient(FirebaseUrl);
        }



        public async Task<bool> AddChildAsync(Child child)
        {
            try
            {
                // ודאי שהאובייקט לא ריק
                if (child == null) return false;

                var result = await client
                    .Child("Children")
                    .PostAsync(child);

                child.Id = result.Key; // שמירת המפתח שנוצר
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Critical Error: {ex.Message}");
                return false;
            }
        }



        // 1. הרשמה - שמירת משתמש חדש
        public async Task<bool> RegisterUser(User newUser)
        {
            try
            {
                var result = await client
                    .Child("Users")
                    .PostAsync(newUser);

                // עדכון ה-Id שנוצר אוטומטית בתוך האובייקט
                newUser.UserId = result.Key;
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 2. התחברות - בדיקה האם משתמש קיים לפי אימייל וסיסמה
        public async Task<User> LoginUser(string email, string password)
        {
            var users = await client
                .Child("Users")
                .OnceAsync<User>();

            var user = users.FirstOrDefault(u => u.Object.UserEmail == email && u.Object.UserPassword == password);

            if (user != null)
            {
                user.Object.UserId = user.Key; // שומרים את ה-ID לשימוש עתידי
                return user.Object;
            }
            return null;
        }

        // 3. משיכת כל המשתמשים - עבור דף המנהל (Manager Page)
        public async Task<List<User>> GetAllUsers()
        {
            var users = await client
                .Child("Users")
                .OnceAsync<User>();

            return users.Select(u => new User
            {
                UserId = u.Key,
                UserEmail = u.Object.UserEmail,
                FirstName = u.Object.FirstName,
                LastName = u.Object.LastName,
                UserMobile = u.Object.UserMobile,
                IsAdmin = u.Object.IsAdmin,
                CreatedAt = u.Object.CreatedAt
            }).ToList();
        }

        // 4. מחיקת משתמש - עבור דף המנהל
        public async Task<bool> DeleteUser(string userId)
        {
            try
            {
                await client.Child("Users").Child(userId).DeleteAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }



        public async Task<List<Child>> GetChildrenByParent(string parentId)
        {
            try
            {
                var allChildren = await client.Child("Children").OnceAsync<Child>();

                // כאן אנחנו מסננים לפי ה-ID של ההורה
                return allChildren
                    .Where(c => c.Object.ParentId == parentId)
                    .Select(c => {
                        var child = c.Object;
                        child.Id = c.Key; // שומרים את המפתח של הילד
                        return child;
                    }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching children: {ex.Message}");
                return new List<Child>();
            }
        }




    }
}