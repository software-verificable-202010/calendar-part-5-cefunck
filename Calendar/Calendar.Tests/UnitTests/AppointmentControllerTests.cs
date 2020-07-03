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
        private const string nullValue = null;
        private const string empty = "";
        private const string aSpace = " ";
        private const string moreThanOneSpace = "   ";
        private const string aTitle = "a title";
        private const string aDescription = "a description";
        private const string usernameThatIsNotOwner = "a username that is not owner";
        private const string ownerUsername = "owner username";
        private const string guestUserName = "guest username";
        private const string emptyTitleMessage = "Debe ingresar un título";
        private const string invalidEndTimeMessage = "Debe ingresar hora de fin válida";
        #endregion


        #region Fields
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
        public void HasOwnerPermissions_ownerUsername_ReturnsTrue()
        {
            mockAppointment
                .Setup(appointment => appointment.IsOwner(ownerUsername))
                .Returns(true);

            appointmentController.RefreshPermissions(ownerUsername);

            Assert.IsTrue(appointmentController.HasOwnerPermissions);
        }

        [Test]
        public void HasOwnerPermissions_usernameThatIsNotOwner_ReturnsFalse()
        {
            appointmentController.RefreshPermissions(usernameThatIsNotOwner);

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
        public void ValidationMessages_InvalidTitle_ReturnsInvalidTitleMessage(string invalidTitle)
        {
            TimeSpan aStart = new TimeSpan(12, 00, 00);
            TimeSpan aEnd = new TimeSpan(12, 01, 00);
            List<string> aGuestsNamesList = new List<string>();

            appointmentController.RefreshCandidateData(invalidTitle, aDescription, aGuestsNamesList, aStart, aEnd);
            appointmentController.RefreshValidationMessages();

            Assert.Contains(emptyTitleMessage, appointmentController.ValidationMessages);
        }

        public void ValidationMessages_InvalidAppointmentEnd_ReturnsInvalidTitleMessage(string invalidTitle)
        {
            TimeSpan aStart = new TimeSpan(12, 30, 00);
            TimeSpan aInvalidEnd = new TimeSpan(12, 00, 00);
            List<string> aGuestsNamesList = new List<string>();

            appointmentController.RefreshCandidateData(aTitle, aDescription, aGuestsNamesList, aStart, aInvalidEnd);
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
            mockAppointment.Setup(appointment => appointment.IsOwner(ownerUsername)).Returns(true);
            appointmentController.RefreshPermissions(ownerUsername);
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
            TimeSpan aStart = new TimeSpan(12, 00, 00);
            TimeSpan aEnd = new TimeSpan(12, 01, 00);
            List<string> aGuestsNamesList = new List<string>()
            {
                guestUserName
            };
            mockAppointment.SetupSet(appointment => appointment.Title = It.IsAny<string>());
            mockAppointment.SetupSet(appointment => appointment.Description = It.IsAny<string>());
            mockAppointment.SetupSet(appointment => appointment.StartTime = It.IsAny<DateTime>());
            mockAppointment.SetupSet(appointment => appointment.EndTime = It.IsAny<DateTime>());
            mockAppointment.Setup(appointment => appointment.AssignGuests(It.IsAny<List<string>>()));
            appointmentController.RefreshCandidateData(aTitle, aDescription, aGuestsNamesList, aStart, aEnd);

            appointmentController.SaveAppointmentData();

            mockAppointment.VerifyAll();
        }

        [Test]
        public void RefreshCandidateData_ValidData_CanSaveAfterRefresh()
        {
            TimeSpan aStart = new TimeSpan(12, 00, 00);
            TimeSpan aEnd = new TimeSpan(12, 01, 00);
            List<string> aGuestsNamesList = new List<string>()
            {
                "usuario1"
            };
            mockAppointment.Setup(appointment => appointment.IsOwner(ownerUsername)).Returns(true);
            appointmentController.RefreshPermissions(ownerUsername);
            bool canSaveBeforeRefresh = appointmentController.CanSaveSourceAppointment();

            appointmentController.RefreshCandidateData(aTitle, aDescription, aGuestsNamesList, aStart, aEnd);

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
        [TestCase(usernameThatIsNotOwner)]
        public void IsOwnerInvited_UsernameThatIsNotOwner_ReturnsFalse(string guestUserName)
        {
            List<string> guestsList = new List<string>()
            {
                guestUserName
            };
            Assert.IsFalse(appointmentController.IsOwnerInvited());
        }

        [Test]
        public void IsOwnerInvited_OwnerUsername_ReturnsTrue()
        {
            TimeSpan aStart = new TimeSpan(12, 00, 00);
            TimeSpan aEnd = new TimeSpan(12, 01, 00);
            List<string> guestsList = new List<string>()
            {
                ownerUsername
            };
            appointmentController.RefreshCandidateData(aTitle, aDescription, guestsList, aStart, aEnd);

            mockAppointment
                .SetupGet(appointment => appointment.OwnerUserName)
                .Returns(ownerUsername);

            Assert.IsTrue(appointmentController.IsOwnerInvited());
        }

        [Test]
        public void CanSaveSourceAppointemt_HasNotOwnerPermissions_ReturnsFalse()
        {
            TimeSpan aStart = new TimeSpan(12, 00, 00);
            TimeSpan aEnd = new TimeSpan(12, 01, 00);
            List<string> aGuestsNamesList = new List<string>()
            {
                guestUserName
            };
            appointmentController.RefreshCandidateData(aTitle, aDescription, aGuestsNamesList, aStart, aEnd);

            Assert.IsFalse(appointmentController.CanSaveSourceAppointment());
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        public void CanSaveSourceAppointemt_HasBlankTitle_ReturnsFalse(string blankTitle)
        {
            const string aValidGuestName = "usuario1";
            TimeSpan aStart = new TimeSpan(12, 00, 00);
            TimeSpan aEnd = new TimeSpan(12, 01, 00);
            List<string> aGuestsNamesList = new List<string>()

            {
                aValidGuestName
            };

            mockAppointment
                .SetupGet(appointment => appointment.OwnerUserName)
                .Returns(ownerUsername);
            appointmentController.RefreshPermissions(ownerUsername);
            appointmentController.RefreshCandidateData(blankTitle, aDescription, aGuestsNamesList, aStart, aEnd);

            Assert.IsFalse(appointmentController.CanSaveSourceAppointment());
        }

        [Test]
        public void CanSaveSourceAppointemt_HasInvalidEnd_ReturnsFalse()
        {            
            const string aValidGuestName = "usuario1";
            TimeSpan aStart = new TimeSpan(12, 01, 00);
            TimeSpan aInvalidEnd = new TimeSpan(12, 00, 00);
            List<string> aGuestsNamesList = new List<string>()

            {
                aValidGuestName
            };

            mockAppointment
                .SetupGet(appointment => appointment.OwnerUserName)
                .Returns(ownerUsername);
            appointmentController.RefreshPermissions(ownerUsername);
            appointmentController.RefreshCandidateData(aTitle, aDescription, aGuestsNamesList, aStart, aInvalidEnd);

            Assert.IsFalse(appointmentController.CanSaveSourceAppointment());
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        public void CanSaveSourceAppointemt_HasInvalidGuestUsername_ReturnsFalse(string invalidGuestUsername)
        {
            TimeSpan aStart = new TimeSpan(12, 00, 00);
            TimeSpan aEnd = new TimeSpan(12, 01, 00);
            List<string> aGuestsNamesList = new List<string>()

            {
                invalidGuestUsername
            };

            mockAppointment
                .SetupGet(appointment => appointment.OwnerUserName)
                .Returns(ownerUsername);
            appointmentController.RefreshPermissions(ownerUsername);
            appointmentController.RefreshCandidateData(aTitle, aDescription, aGuestsNamesList, aStart, aEnd);

            Assert.IsFalse(appointmentController.CanSaveSourceAppointment());
        }

        [Test]
        public void CanSaveSourceAppointemt_HasValidData_ReturnsTrue()
        {
            const string validGuestUsername = "usuario1";
            TimeSpan aStart = new TimeSpan(12, 00, 00);
            TimeSpan aEnd = new TimeSpan(12, 01, 00);
            List<string> aGuestsNamesList = new List<string>()

            {
                validGuestUsername
            };

            mockAppointment
                .Setup(appointment => appointment.IsOwner(ownerUsername))
                .Returns(true);
            appointmentController.RefreshPermissions(ownerUsername);
            appointmentController.RefreshCandidateData(aTitle, aDescription, aGuestsNamesList, aStart, aEnd);

            Assert.IsTrue(appointmentController.CanSaveSourceAppointment());
        }

        [Test]
        public void DeleteSourceAppointment__SetIsInGarbagePropertyToTrue()
        {
            appointmentController.DeleteSourceAppointment();
            mockAppointment.VerifySet(appointment => appointment.IsInGarbage = true);
        }
        #endregion
    }
}