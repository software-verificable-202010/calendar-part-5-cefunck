using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar.Models;

namespace Calendar.Interfaces
{
    public interface IAppointment
    {
        #region Properties
        string Title
        {
            get;
            set;
        }

        string Description
        {
            get;
            set;
        }

        DateTime Start
        {
            get;
            set;
        }

        DateTime End
        {
            get;
            set;
        }

        bool IsInGarbage
        {
            get;
            set;
        }

        string OwnerUsername
        {
            get;
            set;
        }

        List<string> GuestsUsernames
        {
            get;
        }
        #endregion


        #region Methods

        void AssignGuests(List<string> guestsUsernames);

        bool IsCollidingWith(IAppointment otherAppointment);

        bool HasReadPermissions(string username);

        bool IsOwner(string username);

        bool IsGuest(string username);

        #endregion
    }
}
