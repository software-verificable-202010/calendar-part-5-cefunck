namespace Calendar.Controllers
{
    public class SessionController
    {
        #region Constants
        #endregion


        #region Fields
        private string currentUsername;
        #endregion


        #region Properties
        public string CurrentUserName 
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
            if (userController == null)
            {
                throw new System.ArgumentNullException(nameof(userController));
            }

            if (userController.IsValidUserName)
            {
                currentUsername = userController.SourceUserName;
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
