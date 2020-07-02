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
        private string sourceUserName;
        private static readonly List<string> calendarUserNames = new List<string>()
        {
            defaultUsername1,
            defaultUsername2
        };
        #endregion


        #region Properties
        public virtual string SourceUserName
        {
            get
            {
                return sourceUserName;
            }
            set
            {
                sourceUserName = value;
            }
        }

        public virtual bool IsValid
        {
            get
            {
                return calendarUserNames.Contains(sourceUserName);
            }
        }
        #endregion


        #region Methods
        public UserController()
        {
        }

        public UserController(string sourceUserName)
        {
            this.sourceUserName = sourceUserName;
        }

        public List<string> GetValidUserNamesOf(List<string> UserNames)
        {
            List<string> validUserNames = calendarUserNames
                .Where(username => UserNames.Contains(username))
                .ToList();

            return validUserNames;
        }


        public bool ExistsInvalidUserName(List<string> UserNames)
        {
            List<UserController> userControllers = UserNames
                .Select(userName => new UserController(userName))
                .ToList();

            bool existsInvalidUserName = userControllers
                .Any(userController => !userController.IsValid);

            return existsInvalidUserName;
        }
        #endregion
    }
}
