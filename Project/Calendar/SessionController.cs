using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    public static class SessionController
    {
        #region Constants
        private const string calendarUsersResourceName = "calendarUsers";
        private const string currentUserResourceName = "currentUser";
        private const string userDataFilePath = "applicationUsersData";
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static void SetCurrenUser(User currentUser)
        {
            App.Current.Resources[currentUserResourceName] = currentUser;
        }
        public static User GetUserByName(string name)
        {
            List<User> calendarUser = GetCalendarUsers();
            foreach (User user in calendarUser)
            {
                if (user.Name == name)
                {
                    return user;
                }
            }
            return null;
        }
        public static User GetCurrenUser()
        {
            return App.Current.Resources[currentUserResourceName] as User;
        }
        public static List<User> GetCalendarUsers()
        {
            List<User> calendarUsers = App.Current.Resources[calendarUsersResourceName] as List<User>;
            if (calendarUsers == null)
            {
                LoadDefaultUsers();
            }
            return calendarUsers;
        }
        public static void SetCalendarUsers(List<User> calendarUsers)
        {
            App.Current.Resources[calendarUsersResourceName] = calendarUsers;
        }
        private static void LoadPersistentUsers()
        {
            LoadDefaultUsers();
        }
        private static void LoadDefaultUsers()
        {
            const string defaultUserName1 = "usuario1";
            const string defaultUserName2 = "usuario2";
            List<User> defaultUsers = new List<User>()
                {
                    new User(defaultUserName1),
                    new User(defaultUserName2)
                };
            SetCalendarUsers(defaultUsers);
        }
        #endregion
    }
}
