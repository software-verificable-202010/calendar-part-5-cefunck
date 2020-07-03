using NUnit.Framework;
using Calendar.Controllers;
using System;
using Moq;
using System.Collections.Generic;
using Calendar.Interfaces;

namespace Calendar.Test.UnitTests
{
    public class AppointmentControllerTests
    {
        #region Constants
        private const int validStartHour = 12;
        private const int validEndHour = 13;
        private const int invalidEndHour = 11;
        private const int minutes = 00;
        private const int seconds = 00;
        private const string nullValue = null;
        private const string empty = "";
        private const string aSpace = " ";
        private const string moreThanOneSpace = "   ";
        private const string aTitle = "a title";
        private const string aDescription = "a description";
        private const string validUserName = "usuario1";
        private const string userNameThatIsNotOwner = "a username that is not owner";
        private const string ownerUserName = "owner username";
        private const string guestUserName = "guest username";
        private const string emptyTitleMessage = "Debe ingresar un título";
        private const string invalidEndTimeMessage = "Debe ingresar hora de fin válida";
        #endregion


        #region Fields
        private TimeSpan aStart = new TimeSpan(validStartHour, minutes, seconds);
        private TimeSpan aValidEnd = new TimeSpan(validEndHour, minutes, seconds);
        private TimeSpan aInvalidEnd = new TimeSpan(invalidEndHour, minutes, seconds);
        private Mock<IAppointment> mockAppointment;
        private Mock<UserController> mockUserController;
        private AppointmentController appointmentController;
        #endregion


        #region Properties
        #endregion


        #region Methods
        [SetUp]
        public void Setup()
        {
            mockUserController = new Mock<UserController>();
            mockAppointment = new Mock<IAppointment>();
            appointmentController = new AppointmentController(mockAppointment.Object, mockUserController.Object);
        }

        [TearDown]
        public void TearDown()
        {
            mockUserController = null;
            mockAppointment = null;
            appointmentController = null;
        }

        [Test]
        public void HasOwnerPermissions_ownerUserName_ReturnsTrue()
        {
            mockAppointment
                .Setup(appointment => appointment.IsOwner(ownerUserName))
                .Returns(true);

            appointmentController.RefreshPermissions(ownerUserName);

            Assert.IsTrue(appointmentController.HasOwnerPermissions);
        }

        [Test]
        public void HasOwnerPermissions_userNameThatIsNotOwner_ReturnsFalse()
        {
            appointmentController.RefreshPermissions(userNameThatIsNotOwner);
            Assert.IsFalse(appointmentController.HasOwnerPermissions);
        }

        [Test]
        public void SourceAppointment_SomeSourceAppointment_ReturnsSameSourceAppointment()
        {
            Assert.AreEqual(mockAppointment.Object, appointmentController.SourceAppointment);
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        public void ValidationMessages_InvalidTitle_ContainsInvalidTitleMessage(string invalidTitle)
        {
            List<string> aGuestsNamesList = new List<string>();

            appointmentController.RefreshCandidateData(
                invalidTitle, 
                aDescription, 
                aGuestsNamesList, 
                aStart, 
                aValidEnd);

            appointmentController.RefreshValidationMessages();

            Assert.Contains(emptyTitleMessage, appointmentController.ValidationMessages);
        }

        [Test]
        public void ValidationMessages_InvalidAppointmentEnd_ReturnsInvalidTitleMessage()
        {            
            List<string> aGuestsNamesList = new List<string>();

            appointmentController.RefreshCandidateData(
                aTitle, 
                aDescription, 
                aGuestsNamesList, 
                aStart, 
                aInvalidEnd);

            appointmentController.RefreshValidationMessages();

            Assert.Contains(invalidEndTimeMessage, appointmentController.ValidationMessages);
        }

        [Test]
        public void CalendarAppointments_SomeApponintmentsList_ReturnsSameAppointmensList()
        {
            List<IAppointment> mockAppointmentsList = new List<IAppointment>()
            {
                mockAppointment.Object
            };
            AppointmentController.AssignCalendarAppointments(mockAppointmentsList);
            Assert.AreEqual(mockAppointmentsList, AppointmentController.CalendarAppointments);
        }

        [Test]
        public void RefreshPermissions_NullCurrentUser_HasOwnerPermissionsPropertyIsFalse()
        {
            string nullCurrentUserName = null;
            appointmentController.RefreshPermissions(nullCurrentUserName);
            Assert.IsFalse(appointmentController.HasOwnerPermissions);
        }

        [Test]
        public void RefreshPermissions_OwnerOfSelectedAppointment_HasOwnerPermissionsPropertyIsTrue()
        {
            mockAppointment.Setup(appointment => appointment.IsOwner(ownerUserName)).Returns(true);
            appointmentController.RefreshPermissions(ownerUserName);
            Assert.IsTrue(appointmentController.HasOwnerPermissions);
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        public void IsEditingExistingAppointment_NewSourceAppointment_ReturnsFalse(string blankTitle)
        {
            mockAppointment
                .SetupGet(appointment => appointment.Title)
                .Returns(blankTitle);

            Assert.IsFalse(appointmentController.IsEditingExistingAppointment());
        }

        [Test]
        public void IsEditingExistingAppointment_AlreadyExistingSourceAppointment_ReturnsTrue()
        {
            mockAppointment.SetupProperty(appointment => appointment.Title, aTitle);
            Assert.IsTrue(appointmentController.IsEditingExistingAppointment());
        }

        [Test]
        public void SaveAppointmentData_InvocationSourceAppointmentSetters_TrueInAllOfThem()
        {
            List<string> aGuestsNamesList = new List<string>()
            {
                guestUserName
            };

            mockAppointment.SetupSet(appointment => appointment.Title = It.IsAny<string>());
            mockAppointment.SetupSet(appointment => appointment.Description = It.IsAny<string>());
            mockAppointment.SetupSet(appointment => appointment.StartTime = It.IsAny<DateTime>());
            mockAppointment.SetupSet(appointment => appointment.EndTime = It.IsAny<DateTime>());
            mockAppointment.Setup(appointment => appointment.AssignGuests(It.IsAny<List<string>>()));

            appointmentController.RefreshCandidateData(
                aTitle, 
                aDescription, 
                aGuestsNamesList, 
                aStart, 
                aValidEnd);

            appointmentController.SaveAppointmentData();

            mockAppointment.VerifyAll();
        }

        [Test]
        public void RefreshCandidateData_ValidData_CanSaveAfterRefresh()
        {
            List<string> aGuestsNamesList = new List<string>()
            {
                validUserName
            };

            mockAppointment.Setup(appointment => appointment.IsOwner(ownerUserName)).Returns(true);
            appointmentController.RefreshPermissions(ownerUserName);

            bool canSaveBeforeRefresh = appointmentController.CanSaveSourceAppointment();

            appointmentController.RefreshCandidateData(
                aTitle, 
                aDescription, 
                aGuestsNamesList, 
                aStart, 
                aValidEnd);

            bool canSaveAfterRefresh = appointmentController.CanSaveSourceAppointment();

            Assert.Multiple(() =>
            {
                Assert.IsFalse(canSaveBeforeRefresh);
                Assert.IsTrue(canSaveAfterRefresh);
            });
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        [TestCase(userNameThatIsNotOwner)]
        public void IsOwnerInvited_UserNameThatIsNotOwner_ReturnsFalse(string guestUserName)
        {
            List<string> guestsList = new List<string>()
            {
                guestUserName
            };

            Assert.IsFalse(appointmentController.IsOwnerInvited());
        }

        [Test]
        public void IsOwnerInvited_OwnerUserName_ReturnsTrue()
        {

            List<string> guestsList = new List<string>()
            {
                ownerUserName
            };

            appointmentController.RefreshCandidateData(
                aTitle, 
                aDescription, 
                guestsList, 
                aStart, 
                aValidEnd);

            mockAppointment
                .SetupGet(appointment => appointment.OwnerUserName)
                .Returns(ownerUserName);

            Assert.IsTrue(appointmentController.IsOwnerInvited());
        }

        [Test]
        public void CanSaveSourceAppointemt_HasNotOwnerPermissions_ReturnsFalse()
        {
            List<string> aGuestsNamesList = new List<string>()
            {
                guestUserName
            };

            appointmentController.RefreshCandidateData(
                aTitle, 
                aDescription, 
                aGuestsNamesList, 
                aStart, 
                aValidEnd);

            Assert.IsFalse(appointmentController.CanSaveSourceAppointment());
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        public void CanSaveSourceAppointemt_HasBlankTitle_ReturnsFalse(string blankTitle)
        {
            const string aValidGuestName = "usuario1";

            List<string> aGuestsNamesList = new List<string>()

            {
                aValidGuestName
            };

            mockAppointment
                .SetupGet(appointment => appointment.OwnerUserName)
                .Returns(ownerUserName);

            appointmentController.RefreshPermissions(ownerUserName);

            appointmentController.RefreshCandidateData(
                blankTitle, 
                aDescription, 
                aGuestsNamesList, 
                aStart, 
                aValidEnd);

            Assert.IsFalse(appointmentController.CanSaveSourceAppointment());
        }

        [Test]
        public void CanSaveSourceAppointemt_HasInvalidEnd_ReturnsFalse()
        {            
            const string aValidGuestName = "usuario1";
            List<string> aGuestsNamesList = new List<string>()

            {
                aValidGuestName
            };

            mockAppointment
                .SetupGet(appointment => appointment.OwnerUserName)
                .Returns(ownerUserName);

            appointmentController.RefreshPermissions(ownerUserName);

            appointmentController.RefreshCandidateData(
                aTitle, 
                aDescription, 
                aGuestsNamesList, 
                aStart, 
                aInvalidEnd);

            Assert.IsFalse(appointmentController.CanSaveSourceAppointment());
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        public void CanSaveSourceAppointemt_HasInvalidGuestUserName_ReturnsFalse(string invalidGuestUserName)
        {

            List<string> aGuestsNamesList = new List<string>()

            {
                invalidGuestUserName
            };

            mockAppointment
                .SetupGet(appointment => appointment.OwnerUserName)
                .Returns(ownerUserName);

            appointmentController.RefreshPermissions(ownerUserName);

            appointmentController.RefreshCandidateData(
                aTitle, 
                aDescription, 
                aGuestsNamesList, 
                aStart, 
                aValidEnd);

            Assert.IsFalse(appointmentController.CanSaveSourceAppointment());
        }

        [Test]
        public void CanSaveSourceAppointemt_HasValidData_ReturnsTrue()
        {
            const string validGuestUserName = "usuario1";

            List<string> aGuestsNamesList = new List<string>()

            {
                validGuestUserName
            };

            mockAppointment
                .Setup(appointment => appointment.IsOwner(ownerUserName))
                .Returns(true);

            appointmentController.RefreshPermissions(ownerUserName);

            appointmentController.RefreshCandidateData(
                aTitle, 
                aDescription, 
                aGuestsNamesList, 
                aStart, 
                aValidEnd);

            Assert.IsTrue(appointmentController.CanSaveSourceAppointment());
        }

        [Test]
        public void DeleteSourceAppointment_NoInput_IsInGarbagePropertySettedToTrue()
        {
            appointmentController.DeleteSourceAppointment();
            mockAppointment.VerifySet(appointment => appointment.IsInGarbage = true);
        }
        #endregion
    }
}