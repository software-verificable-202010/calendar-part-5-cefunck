using Calendar.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calendar.Models
{
    [Serializable]
    public class Appointment : IAppointment
    {
        #region Constants
        private const string empty = "";
        private const int defaultDurationInMinutes = 30;
        #endregion


        #region Fields
        private string title;
        private string description;
        private bool isInGarbage;
        private DateTime start;
        private DateTime end;
        private string ownerUsername;
        private readonly List<string> guestsUsernames;
        #endregion


        #region Properties
        public string Title
        {
            get 
            { 
                return title; 
            }
            set 
            { 
                title = value;
            }
        }

        public string Description
        {
            get 
            { 
                return description; 
            }
            set
            {
                description = value;
            }
        }

        public DateTime Start
        {
            get 
            {
                return start; 
            }
            set 
            { 
                start = value;
            }
        }

        public DateTime End
        {
            get 
            { 
                return end;
            }
            set 
            { 
                end = value;
            }
        }

        public bool IsInGarbage
        {
            get 
            {
                return isInGarbage;
            }
            set 
            {
                isInGarbage = value;
            }
        }

        public string OwnerUsername 
        {
            get 
            {
                return ownerUsername;
            }
            set 
            {
                ownerUsername = value;
            }
        }

        public List<string> GuestsUsernames
        {
            get
            {
                return guestsUsernames;
            }
        }
        #endregion


        #region Methods
        public Appointment(DateTime startTime, string ownerUsername)
        {
            title = empty;
            description = empty;
            start = startTime;
            end = start.AddMinutes(defaultDurationInMinutes);
            this.ownerUsername = ownerUsername;
            guestsUsernames = new List<string>();
            isInGarbage = false;
        }

        public void AssignGuests(List<string> usernames)
        {
            List<string> distinctUsers = usernames.Distinct().ToList();
            guestsUsernames.Clear();
            guestsUsernames.AddRange(distinctUsers);
        }

        public bool IsCollidingWith(IAppointment otherAppointment)
        {
            if (otherAppointment is null)
            {
                throw new ArgumentNullException();
            }

            bool isThisStartingBeforeOtherEnds = this.start < otherAppointment.End;
            bool isThisEndingAfterOtherStarts = otherAppointment.Start < this.end;
            bool isColliding = isThisStartingBeforeOtherEnds & isThisEndingAfterOtherStarts;
            return isColliding;
        }

        public bool HasReadPermissions(string username)
        {
            return IsOwner(username) | IsGuest(username);
        }

        public bool IsOwner(string username) 
        {
            bool isNotNullUsername = username != null;
            bool isOwner = false;

            if (isNotNullUsername)
            {
                isOwner = OwnerUsername == username;
            }

            return isNotNullUsername & isOwner;
        }

        public bool IsGuest(string username)
        {
            bool isNotNullUsername = username != null;
            bool isGuest = false;

            if (isNotNullUsername)
            {
                isGuest = guestsUsernames.Any(guestUsername => guestUsername == username);
            }

            return isNotNullUsername & isGuest;
        }

        #endregion
    }
}
