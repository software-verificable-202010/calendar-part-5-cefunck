using System;
using System.Collections.Generic;
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
        #endregion

        #region Fields
        private string title;
        private string description;
        private DateTime start;
        private DateTime end;
        private bool isInGarbage;
        private User owner;
        private List<User> guests;
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
            set 
            { 
                guests = value;
            }
        }
        #endregion

        #region Methods
        public Appointment(string title, string description, DateTime start, DateTime end, User owner)
        {
            Title = title;
            Description = description;
            Start = start;
            End = end;
            this.owner = owner;
            isInGarbage = false;
            this.guests = new List<User>();
        }

        public bool IsOwnerOrGuest(User user)
        {
            bool isOwner = owner.Name == user.Name;
            bool isGuest = guests.Any(i => i.Name == user.Name);
            return isOwner | isGuest;
        }
        public bool IsCollidingWith(Appointment appointment)
        {
            DateTime otherAppointmentStart = appointment.Start;
            DateTime otherAppointmentEnd = appointment.End;
            bool isCollidingWithOtherAppointmentStart = otherAppointmentStart >= this.start & otherAppointmentStart <= this.end;
            bool isCollidingWithOtherAppointmentEnd = otherAppointmentEnd >= this.start & otherAppointmentEnd <= this.end;
            bool isColliding = isCollidingWithOtherAppointmentStart | isCollidingWithOtherAppointmentEnd;
            return isColliding;
        }
        #endregion
    }
}
