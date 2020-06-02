using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Calendar
{
    [Serializable]
    public class User
    {
        #region Constants
        #endregion

        #region Fields
        private string name;
        #endregion

        #region Properties
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        #endregion

        #region Methods
        public User(string userName) 
        {
            this.name = userName;
        }
        public bool HasAppointmentCollision(Appointment appointment)
        {
            List<Appointment> selfAppointments = GetAppointments();
            return selfAppointments.Any(i => i.IsCollidingWith(appointment));
        }

        private List<Appointment> GetAppointments()
        {
            return Utilities.GetCalendarAppointments().Where(i => i.Owner.Name == this.name).ToList();
        }


        #endregion
    }
}
