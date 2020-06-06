using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            List<Appointment> selfAppointments = GetSelfAppointments();
            bool existsCollisionWithSelfAppointments = selfAppointments.Any(i => i.IsCollidingWith(appointment));

            List<Appointment> appointmentsWichThisUserIsInvited = GetAppointmentsWhichIAmInvited();
            bool existsCollisionWithAppointmentsWichThisUserIsIvited = appointmentsWichThisUserIsInvited.Any(i => i.IsCollidingWith(appointment));

            return existsCollisionWithSelfAppointments | existsCollisionWithAppointmentsWichThisUserIsIvited;
        }

        private List<Appointment> GetSelfAppointments()
        {
            List<Appointment> selfAppointments = Utilities.CalendarAppointments.Where(appointment => appointment.IsOwner(this)).ToList();
            return selfAppointments;
        }

        private List<Appointment> GetAppointmentsWhichIAmInvited()
        {
            List<Appointment> appointmentsWichThisUserIsInvited = Utilities.CalendarAppointments.Where(appointment => appointment.IsGuest(this)).ToList();
            return appointmentsWichThisUserIsInvited;
        }

        #endregion
    }
}
