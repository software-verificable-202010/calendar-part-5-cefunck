using System.Collections.Generic;
using System.Linq;

namespace Calendar.Controllers
{
    public class UserController
    {
        #region Constants
        private const string firstDefaultUserName = "usuario1";
        private const string secondDefaultUserName = "usuario2";
        #endregion


        #region Fields
        private string sourceUserName;
        private readonly List<string> calendarUserNames = new List<string>()
        {
            firstDefaultUserName,
            secondDefaultUserName
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

        public virtual bool IsValidUserName
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

        public bool ExistsInvalidUserName(List<string> userNames)
        {
            if (userNames == null)
            {
                throw new System.ArgumentNullException(nameof(userNames));
            }

            List<string> validUserNames = this.GetValidUserNamesOf(userNames);
            bool existsInvalidUserName = validUserNames.Count < userNames.Count;
            
            return existsInvalidUserName;
        }

        public List<string> GetValidUserNamesOf(List<string> userNames)
        {
            List<string> validUserNames = calendarUserNames
                .Where(userName => userNames.Contains(userName))
                .ToList();

            return validUserNames;
        }
        #endregion
    }
}
