using System.Collections.Generic;
using System.Linq;
using Calendar.Interfaces;
using Calendar.Models;

namespace Calendar.Controllers
{
    public class UserController
    {
        #region Constants
        private const string defaultUsername1 = "usuario1";
        private const string defaultUsername2 = "usuario2";
        #endregion


        #region Fields
        private string sourceUsername;
        private static readonly List<string> calendarUsernames = new List<string>()
        {
            defaultUsername1,
            defaultUsername2
        };
        #endregion


        #region Properties
        public virtual string SourceUsername
        {
            get
            {
                return sourceUsername;
            }
            set
            {
                sourceUsername = value;
            }
        }

        public virtual bool IsValid
        {
            get
            {
                return calendarUsernames.Contains(sourceUsername);
            }
        }
        #endregion


        #region Methods
        public UserController()
        {
        }

        public UserController(string sourceUsername)
        {
            this.sourceUsername = sourceUsername;
        }

        public List<string> GetValidUsernamesOf(List<string> usernames)
        {
            List<string> validUsernames = calendarUsernames
                .Where(username => usernames.Contains(username))
                .ToList();

            return validUsernames;
        }


        public bool ExistsInvalidUsername(List<string> usernames)
        {
            List<UserController> userControllers = usernames
                .Select(username => new UserController(username))
                .ToList();

            bool existsInvalidUsername = userControllers
                .Any(userController => !userController.IsValid);

            return existsInvalidUsername;
        }
        #endregion
    }
}
