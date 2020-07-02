using System.Collections.Generic;
using Calendar.Controllers;
using Calendar.Models;

namespace Calendar.Controllers
{
    public class SessionController
    {
        #region Constants
        #endregion


        #region Fields
        private static string currentUsername;
        #endregion


        #region Properties
        public string CurrentUsername 
        {
            get 
            {
                return currentUsername;
            }
            set
            {
                currentUsername = value;
            }
        }
        #endregion


        #region Methods

        public SessionController()
        {
            currentUsername = null;
        }

        public void LogOn(UserController userController)
        {
            if (userController.IsValid)
            {
                currentUsername = userController.SourceUsername;
            }
        }

        public bool IsSessionLogoned()
        {
            bool isLogoned = currentUsername != null;
            return isLogoned;
        }
        #endregion
    }
}
