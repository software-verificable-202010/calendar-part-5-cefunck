using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
        #endregion

        #region Methods
        public Appointment(string title, string description, DateTime start, DateTime end)
        {
            Title = title;
            Description = description;
            Start = start;
            End = end;
            isInGarbage = false;
        }
        #endregion
    }
}
