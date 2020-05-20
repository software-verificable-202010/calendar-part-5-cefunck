using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    [Serializable]
    class Appointment
    {
        #region Constants
        #endregion

        #region Fields
        private string title;
        private string description;
        private DateTime start;
        private DateTime end;
        #endregion

        #region Properties
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public DateTime Start
        {
            get { return start; }
            set { start = value; }
        }
        public DateTime End
        {
            get { return end; }
            set { end = value; }
        }
        #endregion

        #region Methods
        public Appointment(string title, string description, DateTime start, DateTime end)
        {
            Title = title;
            Description = description;
            Start = start;
            End = end;
        }
        #endregion
    }
}
