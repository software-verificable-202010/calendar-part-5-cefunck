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
        private readonly List<string> calendarUserNames = new List<string>()
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
                return this.calendarUserNames.Contains(sourceUserName);
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


        public bool ExistsInvalidUserName(List<string> userNames)
        {
            bool isNotNullUserNamesList = userNames != null;
            bool existsInvalidUserName = false;

            if (isNotNullUserNamesList)
            {
                List<string> validUserNames = this.GetValidUserNamesOf(userNames);
                existsInvalidUserName = validUserNames.Count < userNames.Count;
            }
            
            return isNotNullUserNamesList && existsInvalidUserName;
        }
        #endregion
    }
}
