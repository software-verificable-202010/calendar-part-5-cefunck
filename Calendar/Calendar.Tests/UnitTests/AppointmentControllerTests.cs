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
            // Arrange
            mockAppointment
                .Setup(appointment => appointment.IsOwner(ownerUsername))
                .Returns(true);

            appointmentController.RefreshPermissions(ownerUsername);

            // Act & Assert
            Assert.IsTrue(appointmentController.HasOwnerPermissions);
        }

        [Test]
        public void HasOwnerPermissions_usernameThatIsNotOwner_ReturnsFalse()
        {
            // Arrange
            appointmentController.RefreshPermissions(usernameThatIsNotOwner);

            // Act & Assert
            Assert.IsFalse(appointmentController.HasOwnerPermissions);
        }

        [Test]
        public void SourceAppointment_SomeSourceAppointment_ReturnsSameSourceAppointment()
        {
            // Act & Assert
            Assert.AreEqual(mockAppointment.Object, appointmentController.SourceAppointment);
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        public void ValidationMessages_InvalidTitle_ReturnsInvalidTitleMessage(string invalidTitle)
        {
            // Arrange
            TimeSpan aStart = new TimeSpan(12, 00, 00);
            TimeSpan aEnd = new TimeSpan(12, 01, 00);
            List<string> aGuestsNamesList = new List<string>();

            appointmentController.RefreshCandidateData(invalidTitle, aDescription, aGuestsNamesList, aStart, aEnd);
            appointmentController.RefreshValidationMessages();

            // Act & Assert
            Assert.Contains(emptyTitleMessage, appointmentController.ValidationMessages);
        }

        public void ValidationMessages_InvalidAppointmentEnd_ReturnsInvalidTitleMessage(string invalidTitle)
        {
            // Arrange
            TimeSpan aStart = new TimeSpan(12, 30, 00);
            TimeSpan aInvalidEnd = new TimeSpan(12, 00, 00);
            List<string> aGuestsNamesList = new List<string>();

            appointmentController.RefreshCandidateData(aTitle, aDescription, aGuestsNamesList, aStart, aInvalidEnd);
            appointmentController.RefreshValidationMessages();

            // Act & Assert
            Assert.Contains(invalidEndTimeMessage, appointmentController.ValidationMessages);
        }

        [Test]
        public void CalendarAppointments_SomeApponintmentsList_ReturnsSameAppointmensList()
        {
            // Arrange
            List<IAppointment> mockAppointmentsList = new List<IAppointment>()
            {
                mockAppointment.Object
            };
            AppointmentController.AssignCalendarAppointments(mockAppointmentsList);

            // Act & Assert
            Assert.AreEqual(mockAppointmentsList, AppointmentController.CalendarAppointments);
        }

        [Test]
        public void RefreshPermissions_NullCurrentUser_HasOwnerPermissionsPropertyIsFalse()
        {
            // Arrange
            string nullCurrentUserName = null;

            // Act
            appointmentController.RefreshPermissions(nullCurrentUserName);

            //Assert
            Assert.IsFalse(appointmentController.HasOwnerPermissions);
        }

        [Test]
        public void RefreshPermissions_OwnerOfSelectedAppointment_HasOwnerPermissionsPropertyIsTrue()
        {
            // Arrange
            mockAppointment.Setup(appointment => appointment.IsOwner(ownerUsername)).Returns(true);

            // Act
            appointmentController.RefreshPermissions(ownerUsername);

            //Assert
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

            // Act & Assert
            Assert.IsFalse(appointmentController.IsEditingExistingAppointment());
        }

        [Test]
        public void IsEditingExistingAppointment_AlreadyExistingSourceAppointment_ReturnsTrue()
        {
            // Arrange
            mockAppointment.SetupProperty(appointment => appointment.Title, aTitle);

            // Act & Assert
            Assert.IsTrue(appointmentController.IsEditingExistingAppointment());
        }

        [Test]
        public void SaveAppointmentData_InvocationSourceAppointmentSetters_TrueInAllOfThem()
        {
            // Arrange
            TimeSpan aStart = new TimeSpan(12, 00, 00);
            TimeSpan aEnd = new TimeSpan(12, 01, 00);
            List<string> aGuestsNamesList = new List<string>()
            {
                guestUserName
            };

            mockAppointment.SetupSet(appointment => appointment.Title = It.IsAny<string>());
            mockAppointment.SetupSet(appointment => appointment.Description = It.IsAny<string>());
            mockAppointment.SetupSet(appointment => appointment.Start = It.IsAny<DateTime>());
            mockAppointment.SetupSet(appointment => appointment.End = It.IsAny<DateTime>());
            mockAppointment.Setup(appointment => appointment.AssignGuests(It.IsAny<List<string>>()));
            appointmentController.RefreshCandidateData(aTitle, aDescription, aGuestsNamesList, aStart, aEnd);

            // Act
            appointmentController.SaveAppointmentData();

            // Assert
            mockAppointment.VerifyAll();
        }

        [Test]
        public void RefreshCandidateData_ValidData_CanSaveAfterRefresh()
        {
            // Arrange
            TimeSpan aStart = new TimeSpan(12, 00, 00);
            TimeSpan aEnd = new TimeSpan(12, 01, 00);
            List<string> aGuestsNamesList = new List<string>()
            {
                "usuario1"
            };
            mockAppointment.Setup(appointment => appointment.IsOwner(ownerUsername)).Returns(true);
            appointmentController.RefreshPermissions(ownerUsername);
            bool canSaveBeforeRefresh = appointmentController.CanSaveSourceAppointment();

            // Act
            appointmentController.RefreshCandidateData(aTitle, aDescription, aGuestsNamesList, aStart, aEnd);

            // Assert
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
            // Arrange
            List<string> guestsList = new List<string>()
            {
                guestUserName
            };

            // Act & Assert
            Assert.IsFalse(appointmentController.IsOwnerInvited());
        }

        [Test]
        public void IsOwnerInvited_OwnerUsername_ReturnsTrue()
        {
            // Arrange
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

            // Act & Assert
            Assert.IsTrue(appointmentController.IsOwnerInvited());
        }

        [Test]
        public void CanSaveSourceAppointemt_HasNotOwnerPermissions_ReturnsFalse()
        {
            // Arrange
            TimeSpan aStart = new TimeSpan(12, 00, 00);
            TimeSpan aEnd = new TimeSpan(12, 01, 00);
            List<string> aGuestsNamesList = new List<string>()
            {
                guestUserName
            };
            appointmentController.RefreshCandidateData(aTitle, aDescription, aGuestsNamesList, aStart, aEnd);

            // Act & Assert
            Assert.IsFalse(appointmentController.CanSaveSourceAppointment());
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        public void CanSaveSourceAppointemt_HasBlankTitle_ReturnsFalse(string blankTitle)
        {
            // Arrange
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

            // Act & Assert
            Assert.IsFalse(appointmentController.CanSaveSourceAppointment());
        }

        [Test]
        public void CanSaveSourceAppointemt_HasInvalidEnd_ReturnsFalse()
        {
            // Arrange
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

            // Act & Assert
            Assert.IsFalse(appointmentController.CanSaveSourceAppointment());
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        public void CanSaveSourceAppointemt_HasInvalidGuestUsername_ReturnsFalse(string invalidGuestUsername)
        {
            // Arrange
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

            // Act & Assert
            Assert.IsFalse(appointmentController.CanSaveSourceAppointment());
        }

        [Test]
        public void CanSaveSourceAppointemt_HasValidData_ReturnsTrue()
        {
            // Arrange
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

            // Act & Assert
            Assert.IsTrue(appointmentController.CanSaveSourceAppointment());
        }

        [Test]
        public void DeleteSourceAppointment__SetIsInGarbagePropertyToTrue()
        {
            // Act
            appointmentController.DeleteSourceAppointment();

            // Assert
            mockAppointment.VerifySet(appointment => appointment.IsInGarbage = true);
        }
        #endregion
    }
}