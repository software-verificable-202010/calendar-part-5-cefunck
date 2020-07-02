using Calendar.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Calendar.Controllers
{
    public class AppointmentController
    {
        #region Constants
        private const string invalidTitleMessage = "Debe ingresar un título";
        private const string invalidEndTimeMessage = "Debe ingresar hora de fin válida";
        private const string invalidGuestsMessage = "Los siguientes invitados no son válidos:";
        private const string appointmentCollisionMessage = "Los siguientes invitados tienen un evento que colisiona:";
        private const string appointmentsDataFilePath = "applicationAppointmentsData";
        #endregion


        #region Fields
        private bool hasOwnerPermissions = false;
        private string candidateTitle;
        private string candidateDescription;
        private DateTime candidateStart;
        private DateTime candidateEnd;
        private UserController userController;
        private List<string> candidateGuestsUserNames = new List<string>();
        private readonly List<string> validationMessages = new List<string>();
        private readonly IAppointment sourceAppointment;
        private static List<IAppointment> calendarAppointments = new List<IAppointment>();
        #endregion


        #region Properties
        public bool HasOwnerPermissions 
        {
            get 
            {
                return hasOwnerPermissions;
            }
        }

        public string SourceAppointmentTitle
        {
            get
            {
                return sourceAppointment.Title;
            }
        }

        public string SourceAppointmentDescription
        {
            get
            {
                return sourceAppointment.Description;
            }
        }

        public DateTime SourceAppointmentStart
        {
            get
            {
                return sourceAppointment.StartTime;
            }
        }

        public DateTime SourceAppointmentEnd
        {
            get
            {
                return sourceAppointment.EndTime;
            }
        }

        public List<string> SourceAppointmentGuestsUserNames
        {
            get
            {
                return sourceAppointment.GuestsUserNames;
            }
        }

        public IAppointment SourceAppointment 
        { 
            get 
            { 
                return sourceAppointment; 
            } 
        }

        public List<string> ValidationMessages
        {
            get 
            {
                return validationMessages;
            }
        }

        public static List<IAppointment> CalendarAppointments
        {
            get
            {
                return calendarAppointments;
            }
        }

        #endregion


        #region Methods
        public AppointmentController(IAppointment sourceAppointment, UserController userController) 
        {
            this.sourceAppointment = sourceAppointment;
            this.userController = userController;
        }

        public void RefreshPermissions(string userName)
        {
            hasOwnerPermissions = sourceAppointment.IsOwner(userName);
        }

        public bool IsEditingExistingAppointment()
        {
            bool isNotNullSourceAppointment = sourceAppointment != null;
            bool isNotNullSourceAppointmentTitle = false;
            bool isNotBlankTitle = false;

            if (isNotNullSourceAppointment)
            {
                isNotNullSourceAppointmentTitle = sourceAppointment.Title != null;
            }

            if (isNotNullSourceAppointmentTitle)
            {
                string sourceAppointmentTitle = sourceAppointment.Title.Trim();
                isNotBlankTitle = sourceAppointmentTitle.Length != 0;
            }
            
            return isNotNullSourceAppointmentTitle && isNotBlankTitle;
        }

        public void SaveAppointmentData()
        {
            sourceAppointment.Title = candidateTitle;
            sourceAppointment.Description = candidateDescription;
            sourceAppointment.StartTime = candidateStart;
            sourceAppointment.EndTime = candidateEnd;
            sourceAppointment.AssignGuests(candidateGuestsUserNames);
        }

        public void RefreshCandidateData(string titleFieldText, string descriptionFieldText, List<string> guestsNamesInField, TimeSpan startTimeField, TimeSpan endTimeField)
        {
            candidateTitle = titleFieldText;
            candidateDescription = descriptionFieldText;
            candidateStart = sourceAppointment.StartTime.Date + startTimeField;
            candidateEnd = sourceAppointment.EndTime.Date + endTimeField;
            candidateGuestsUserNames = guestsNamesInField;
        }

        public bool IsOwnerInvited()
        {
            bool isNotNullGuestUserNames = candidateGuestsUserNames != null;
            string ownerName = sourceAppointment.OwnerUserName;
            bool isOwnerInvited = false;

            if (isNotNullGuestUserNames)
            {
                isOwnerInvited = candidateGuestsUserNames.Contains(ownerName);
            }

            return isNotNullGuestUserNames && isOwnerInvited;
        }

        public bool CanSaveSourceAppointment()
        {
            return hasOwnerPermissions & 
                IsNotBlankTitle() & 
                IsValidEndTime() & 
                AreValidGuests() & 
                !ExistingAppointmentCollision();
        }

        public void DeleteSourceAppointment()
        {
            sourceAppointment.IsInGarbage = true;
        }

        public bool AreValidGuests()
        {
            bool isNotNullCandidateGuestsUserNames = candidateGuestsUserNames != null;
            bool isGuestFieldEmpty = true;
            bool existsInvalidUsername = false;
            bool areValidGuests = false;

            if (isNotNullCandidateGuestsUserNames)
            {
                isGuestFieldEmpty = candidateGuestsUserNames.Count == 0;
                existsInvalidUsername = userController.ExistsInvalidUserName(candidateGuestsUserNames);
                areValidGuests = (isGuestFieldEmpty | (!existsInvalidUsername & !IsOwnerInvited()));
            }
            

            return isNotNullCandidateGuestsUserNames & areValidGuests;
        }

        public bool ExistingAppointmentCollision()
        {
            bool hasAppointmentCollision = candidateGuestsUserNames
                .Any(guestUserName => HasAppointmentCollision(guestUserName, sourceAppointment));

            return hasAppointmentCollision;
        }

        public void RefreshValidationMessages()
        {
            ClearOldValidationMessages();

            if (!IsNotBlankTitle())
            {
                validationMessages.Add(invalidTitleMessage);
            }
            if (!IsValidEndTime())
            {
                validationMessages.Add(invalidEndTimeMessage);
            }
            if (!AreValidGuests())
            {
                validationMessages.Add(invalidGuestsMessage);
                AddInvalidGuestNamesToValidationMessages();
            }
            if (ExistingAppointmentCollision())
            {
                validationMessages.Add(appointmentCollisionMessage);
                AddCollisionedGuestNamesToValidationMessages();
            }
        }

        public void AddCollisionedGuestNamesToValidationMessages()
        {
            const string nameFormat = "- {0}";
            foreach (string candidateGuestUsername in candidateGuestsUserNames)
            {
                bool isNotOwner = !sourceAppointment.IsOwner(candidateGuestUsername);

                bool isColliding = HasAppointmentCollision(candidateGuestUsername, sourceAppointment);
                if (isNotOwner & isColliding)
                {
                    validationMessages.Add(string.Format(CultureInfo.CurrentCulture, nameFormat, candidateGuestUsername));
                }
            }
        }

        public void AddInvalidGuestNamesToValidationMessages()
        {
            const string nameFormat = "- {0}";
            foreach (string name in candidateGuestsUserNames)
            {
                userController.SourceUserName = name;
                bool isOwnerUser = name == sourceAppointment.OwnerUserName;
                bool isInvalidUsername = !userController.IsValidUserName;
                bool isInvalidGuest = isOwnerUser | isInvalidUsername;
                if (isInvalidGuest)
                {
                    validationMessages.Add(string.Format(CultureInfo.CurrentCulture, nameFormat, name));
                }
            }
        }

        public bool IsNotBlankTitle()
        {
            bool isNotNullCandidateTitle = candidateTitle != null;
            bool isNotBlankTitle = false;

            if (isNotNullCandidateTitle)
            {
                isNotBlankTitle = candidateTitle.Trim().Length != 0;
            }
            
            return isNotNullCandidateTitle && isNotBlankTitle;
        }

        public bool IsValidEndTime()
        {
            bool isEndAfterStart = candidateEnd > candidateStart;
            return isEndAfterStart;
        }

        public static List<IAppointment> GetAppointmentsWhereIsOwner(string userName)
        {
            List<IAppointment> userAppointments = CalendarAppointments.Where(appointment => appointment.IsOwner(userName)).ToList();
            return userAppointments;
        }

        public static List<IAppointment> GetAppointmentsWhereIsGuest(string userName)
        {
            List<IAppointment> appointmentsWichThisUserIsInvited = CalendarAppointments.Where(appointment => appointment.IsGuest(userName)).ToList();
            return appointmentsWichThisUserIsInvited;
        }

        public static void AssignCalendarAppointments(List<IAppointment> appointments)
        {
            calendarAppointments = appointments;
        }

        public static void LoadPersistentAppointments()
        {
            bool isAppointmentsDataFileFound = File.Exists(appointmentsDataFilePath);

            if (isAppointmentsDataFileFound)
            {
                using (FileStream file = new FileStream(appointmentsDataFilePath, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    object deserealizedAppointments = bf.Deserialize(file);
                    calendarAppointments = deserealizedAppointments as List<IAppointment>;
                }
            }
            else
            {
                calendarAppointments = new List<IAppointment>();
            }
        }

        public static void SavePersistentAppointments()
        {
            List<IAppointment> calendarAppointments = CalendarAppointments;
            using (FileStream file = new FileStream(appointmentsDataFilePath, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, calendarAppointments);
                file.Flush();
            }
        }

        public static bool HasAppointmentCollision(string guestUserName, IAppointment appointmentThatCouldCollide)
        {
            bool isNotNullGuestUsername = guestUserName != null;
            bool hasCollisionWithHisOwnAppointments = false;
            bool hasCollisionWithAppointmentsWichThisUserIsIvited = false;

            if (isNotNullGuestUsername)
            {

                hasCollisionWithHisOwnAppointments = AppointmentController.GetAppointmentsWhereIsOwner(guestUserName)
                    .Any(appointment => appointment
                    .IsCollidingWith(appointmentThatCouldCollide));

                hasCollisionWithAppointmentsWichThisUserIsIvited = AppointmentController.GetAppointmentsWhereIsGuest(guestUserName)
                    .Any(appointment => appointment
                    .IsCollidingWith(appointmentThatCouldCollide) & appointment != appointmentThatCouldCollide);
            }

            return isNotNullGuestUsername && (hasCollisionWithHisOwnAppointments || hasCollisionWithAppointmentsWichThisUserIsIvited);
        }

        private void ClearOldValidationMessages()
        {
            validationMessages.Clear();
        }

        #endregion
    }
}
