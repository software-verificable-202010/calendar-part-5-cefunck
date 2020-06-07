using System;
using System.Collections.Generic;
using System.Linq;

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

        public bool HasAppointmentCollision(Appointment appointmentThatCouldCollide)
        {
            List<Appointment> selfAppointments = GetSelfAppointments();
            bool existsCollisionWithSelfAppointments = selfAppointments
                .Any(appointment => appointment.IsCollidingWith(appointmentThatCouldCollide));

            List<Appointment> appointmentsWichThisUserIsInvited = GetAppointmentsWhichThisUserIsInvited();
            bool existsCollisionWithAppointmentsWichThisUserIsIvited = appointmentsWichThisUserIsInvited
                .Any(appointment => appointment.IsCollidingWith(appointmentThatCouldCollide));

            return existsCollisionWithSelfAppointments | existsCollisionWithAppointmentsWichThisUserIsIvited;
        }

        private List<Appointment> GetSelfAppointments()
        {
            List<Appointment> selfAppointments = Utilities.CalendarAppointments
                .Where(appointment => appointment.IsOwner(this)).ToList();
            return selfAppointments;
        }

        private List<Appointment> GetAppointmentsWhichThisUserIsInvited()
        {
            List<Appointment> appointmentsWichThisUserIsInvited = Utilities.CalendarAppointments
                .Where(appointment => appointment.IsGuest(this)).ToList();
            return appointmentsWichThisUserIsInvited;
        }

        #endregion
    }
}
