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
    public class Appointment : INotifyPropertyChanged
    {
        #region Constants
        #endregion

        #region Fields
        public event PropertyChangedEventHandler PropertyChanged;
        private string title;
        private string description;
        private DateTime start;
        private DateTime end;
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
