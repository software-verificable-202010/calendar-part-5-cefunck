using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calendar
{
    [Serializable]
    public class Appointment
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
        private User owner;
        private readonly List<User> guests;
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

        public User Owner 
        {
            get 
            {
                return owner;
            }
            set 
            {
                owner = value;
            }
        }

        public List<User> Guests
        {
            get
            {
                return guests;
            }
        }
        #endregion


        #region Methods
        public void AssignGuests(List<User> users)
        {
            guests.Clear();
            guests.AddRange(users);
        }

        public Appointment(DateTime start, User owner)
        {
            Title = empty;
            Description = empty;
            Start = start;
            End = start.AddMinutes(defaultDurationInMinutes);
            this.owner = owner;
            isInGarbage = false;
            this.guests = new List<User>();
        }

        public bool IsCollidingWith(Appointment otherAppointment)
        {
            const string argumentNameOfNullException = "appointment";
            if (otherAppointment == null)
            {
                throw new ArgumentNullException(argumentNameOfNullException);
            }

            bool isThisStartingBeforeOtherEnds = this.start < otherAppointment.end;
            bool isThisEndingAfterOtherStarts = otherAppointment.start < this.end;
            bool isColliding = isThisStartingBeforeOtherEnds & isThisEndingAfterOtherStarts;
            return isColliding;
        }

        public bool HasReadPermissions(User user)
        {
            return HasOwnerPermissions(user) | IsGuest(user);
        }

        public bool HasOwnerPermissions(User user) 
        {
            return owner.Name == user.Name;
        }

        private bool IsGuest(User user) 
        {
            return guests.Any(guest => guest.Name == user.Name);
        }

        #endregion
    }
}
