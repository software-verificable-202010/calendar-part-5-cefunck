using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    [Serializable]
    public class User
    {
        #region Constants
        #endregion

        #region Fields
        private string userName;
        #endregion

        #region Properties
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
            }
        }
        #endregion

        #region Methods
        public User(string userName) 
        {
            this.userName = userName;
        }
        #endregion
    }
}
