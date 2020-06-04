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
        #endregion

        #region Fields
        private string title;
        private string description;
        private DateTime start;
        private DateTime end;
        private bool isInGarbage;
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
            if (appointment == null)
            {
                throw new ArgumentNullException("appointment");
            }
            bool isColliding = this.start < appointment.end & appointment.start < this.end;
            return isColliding;
        }
        public void ClearGuests() 
        {
            guests.Clear();
        }
        public void AddGuests(List<User> users) 
        {
            guests.AddRange(users);
        }
        #endregion
    }
}
