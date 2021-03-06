﻿using System;
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

        DateTime StartTime
        {
            get;
            set;
        }

        DateTime EndTime
        {
            get;
            set;
        }

        bool IsInGarbage
        {
            get;
            set;
        }

        string OwnerUserName
        {
            get;
            set;
        }

        List<string> GuestsUserNames
        {
            get;
        }
        #endregion


        #region Methods

        void AssignGuests(List<string> userNames);

        bool IsCollidingWith(IAppointment otherAppointment);

        bool HasReadPermissions(string userName);

        bool IsOwner(string userName);

        bool IsGuest(string userName);

        #endregion
    }
}
