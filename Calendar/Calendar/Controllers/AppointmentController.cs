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
            string sourceAppointmentTitle = sourceAppointment.Title.Trim();
            bool isNotBlankTitle = sourceAppointmentTitle.Length != 0;
            return isNotBlankTitle;
        }

        public void SaveAppointmentData()
        {
            sourceAppointment.Title = candidateTitle;
            sourceAppointment.Description = candidateDescription;
            sourceAppointment.Start = candidateStart;
            sourceAppointment.End = candidateEnd;
            sourceAppointment.AssignGuests(candidateGuestsUserNames);
        }

        public void RefreshCandidateData(string titleFieldText, string descriptionFieldText, List<string> guestsNamesInField, TimeSpan startTimeField, TimeSpan endTimeField)
        {
            candidateTitle = titleFieldText;
            candidateDescription = descriptionFieldText;
            candidateStart = sourceAppointment.Start.Date + startTimeField;
            candidateEnd = sourceAppointment.End.Date + endTimeField;
            candidateGuestsUserNames = guestsNamesInField;
        }

        public bool IsOwnerInvited(List<string> guestsNamesInField)
        {
            bool isNotNullGuestsNamesList = guestsNamesInField != null;
            bool isOwnerInvited = false;

            if (isNotNullGuestsNamesList)
            {
                string ownerName = sourceAppointment.OwnerUserName;
                isOwnerInvited = guestsNamesInField.Contains(ownerName);
            }

            return isNotNullGuestsNamesList & isOwnerInvited;
        }

        public bool CanSaveSourceAppointment()
        {
            return hasOwnerPermissions & 
                IsNotBlankTitle(candidateTitle) & 
                IsValidEndTime() & 
                AreValidGuests(candidateGuestsUserNames) & 
                !ExistingAppointmentCollision();
        }

        public void DeleteSourceAppointment()
        {
            sourceAppointment.IsInGarbage = true;
        }

        public bool AreValidGuests(List<string> guestsNamesInField)
        {
            bool isNotNullGuestNamesList = guestsNamesInField != null;
            bool isGuestFieldEmpty = true;
            bool existsInvalidUsername = false;

            if (isNotNullGuestNamesList)
            {
                isGuestFieldEmpty = guestsNamesInField.Count == 0;
                existsInvalidUsername = userController.ExistsInvalidUserName(guestsNamesInField);
            }

            
            return (isGuestFieldEmpty | (!existsInvalidUsername & !IsOwnerInvited(guestsNamesInField)));
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

            if (!IsNotBlankTitle(candidateTitle))
            {
                validationMessages.Add(invalidTitleMessage);
            }
            if (!IsValidEndTime())
            {
                validationMessages.Add(invalidEndTimeMessage);
            }
            if (!AreValidGuests(candidateGuestsUserNames))
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
                bool isInvalidUsername = !userController.IsValid;
                bool isInvalidGuest = isOwnerUser | isInvalidUsername;
                if (isInvalidGuest)
                {
                    validationMessages.Add(string.Format(CultureInfo.CurrentCulture, nameFormat, name));
                }
            }
        }

        public bool IsNotBlankTitle(string titleFieldText)
        {
            bool isNotNullTitle = titleFieldText != null;
            bool isNotBlankTitle = false;

            if (isNotNullTitle)
            {
                string titleCandidate = titleFieldText.Trim();
                isNotBlankTitle = titleCandidate.Length != 0;
            }
            
            return isNotNullTitle & isNotBlankTitle;
        }

        public bool IsValidEndTime()
        {
            bool isEndAfterStart = candidateEnd > candidateStart;
            return isEndAfterStart;
        }

        public string GetSourceAppointmentTitle() 
        {
            return sourceAppointment.Title;
        }

        public string GetSourceAppointmentDescription() 
        {
            return sourceAppointment.Description;
        }

        public DateTime GetSourceAppointmentStart() 
        {
            return sourceAppointment.Start;
        }

        public DateTime GetSourceAppointmentEnd() 
        {
            return sourceAppointment.End;
        }

        public List<string> GetSourceAppointmentGuestsUserNames()
        {
            return sourceAppointment.GuestsUserNames;
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

        public bool HasAppointmentCollision(string guestUserName, IAppointment appointmentThatCouldCollide)
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

        private bool IsValidGuest(string candidateGuestUsername)
        {
            UserController candidateGuestUserController = new UserController(candidateGuestUsername);
            bool isNotNullUser = candidateGuestUserController != null;
            bool isNotOwner = false;
            bool hasNotAppointmentCollision = false;

            if (isNotNullUser)
            {
                isNotOwner = !sourceAppointment.IsOwner(candidateGuestUsername);
                hasNotAppointmentCollision = !HasAppointmentCollision(candidateGuestUsername, sourceAppointment);
            }

            return isNotNullUser & isNotOwner & hasNotAppointmentCollision;

        }

        private void ClearOldValidationMessages()
        {
            validationMessages.Clear();
        }

        #endregion
    }
}
