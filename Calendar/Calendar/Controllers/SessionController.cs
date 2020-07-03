namespace Calendar.Controllers
{
    public class SessionController
    {
        #region Constants
        #endregion


        #region Fields
        private string currentUserName;
        #endregion


        #region Properties
        public string CurrentUserName 
        {
            get 
            {
                return currentUserName;
            }
            set
            {
                currentUserName = value;
            }
        }
        #endregion


        #region Methods

        public SessionController()
        {
            currentUserName = null;
        }

        public void LogOn(UserController userController)
        {
            if (userController == null)
            {
                throw new System.ArgumentNullException(nameof(userController));
            }

            if (userController.IsValidUserName)
            {
                currentUserName = userController.SourceUserName;
            }
        }

        public bool IsSessionLogoned()
        {
            bool isLogoned = currentUserName != null;
            return isLogoned;
        }
        #endregion
    }
}
