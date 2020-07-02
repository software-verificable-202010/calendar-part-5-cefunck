using NUnit.Framework;
using System;
using Moq;
using System.Collections.Generic;
using Calendar.Models;
using Calendar.Interfaces;

namespace Calendar.Test
{
    public class AppointmentTests
    {
        #region Constants
        const string nullValue = null;
        const string empty = "";
        const string aSpace = " ";
        const string moreThanOneSpace = "    ";
        const string validUserNameThatHasNotPermissions = "a user name that has not permissions";
        const string validUserNameThatIsNotOwner = "a user name that is not owner";
        const string validUserNameThatIsNotGuest = "a user name that is not guest";
        #endregion


        #region Fields
        private Appointment appointment;
        private string aOwnerName;
        private DateTime aStart;
        #endregion


        #region Properties
        #endregion


        #region Methods
        [SetUp]
        public void Setup()
        {
            aStart = It.IsAny<DateTime>();
            aOwnerName = It.IsAny<string>();
            appointment = new Appointment(aStart, aOwnerName);
        }

        [TearDown]
        public void TearDown()
        {
            appointment = null;
        }

        [Test]
        public void Title_SomeTitle_ReturnsSameTitle()
        {
            // Arrange
            const string aTitle = "a title";
            appointment.Title = aTitle;

            // Act & Assert
            Assert.AreEqual(aTitle, appointment.Title);
        }

        [Test]
        public void Description_SomeDescription_ReturnsSameDescription()
        {
            // Arrange
            const string aDescription = "a descrption";
            appointment.Description = aDescription;

            // Act & Assert
            Assert.AreEqual(aDescription, appointment.Description);
        }

        [Test]
        public void Start_SomeStart_ReturnsSameStart()
        {
            // Arrange
            DateTime newStart = DateTime.Now;
            appointment.StartTime = newStart;

            // Act & Assert
            Assert.AreEqual(newStart, appointment.StartTime);
        }

        [Test]
        public void End_SomeEnd_ReturnsSameEnd()
        {
            // Arrange
            DateTime end = DateTime.Now;
            appointment.EndTime = end;

            // Act & Assert
            Assert.AreEqual(end, appointment.EndTime);
        }

        [Test]
        public void IsInGarbage_DefaultValue_ReturnsDefaultValue()
        {
            // Act & Assert
            Assert.IsFalse(appointment.IsInGarbage);
        }

        [Test]
        public void IsInGarbage_SomeValue_ReturnsSameValue()
        {
            // Arrange
            appointment.IsInGarbage = true;

            // Act & Assert
            Assert.IsTrue(appointment.IsInGarbage);
        }

        [Test]
        public void Owner_SomeAppointmentOwner_ReturnsSameAppointmentOwner()
        {
            // Arrange
            const string aNewOwnerUsername = "a new owner username";
            appointment.OwnerUserName = aNewOwnerUsername;

            // Act & Assert
            Assert.AreEqual(aNewOwnerUsername, appointment.OwnerUserName);
        }

        [Test]
        public void Guests_SomeAppointmentGuests_ReturnsSameAppointmentGuests()
        {
            // Arrange
            const string aGuestUsername = "a guest name";
            const string otherGuestUsername = "other guest username";

            List<string> aGuestList = new List<string>()
            {
                aGuestUsername,
                otherGuestUsername
            };
            appointment.AssignGuests(aGuestList);

            // Act & Assert
            Assert.AreEqual(aGuestList, appointment.GuestsUserNames);
        }

        [Test]
        public void IsCollidingWith_NullAppointment_ReturnsArgumentNullException()
        {
            // Arrange
            IAppointment nullAppointment = null;

            // Act & Assert
            Assert.That(() => appointment.IsCollidingWith(nullAppointment), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void IsCollidingWith_AppointmentThatColliding_ReturnsTrue()
        {
            // Arrange
            const int appointmentStartHour = 12;
            const int appointmentStartMinutes = 00;
            const int appointmentStartSeconds = 00;
            const int appointmentHoursDuration = 2;

            const int mockAppointmentStartHour = 12;
            const int mockAppointmentStartMinutes = 00;
            const int mockAppointmentStartSeconds = 00;
            const int mockAppointmentHoursDuration = 1;

            DateTime appointmentStart = DateTime.Now.Date + new TimeSpan(appointmentStartHour, appointmentStartMinutes, appointmentStartSeconds);
            DateTime appointmentEnd = appointmentStart.AddHours(appointmentHoursDuration);

            DateTime mockAppointmentStart = DateTime.Now.Date + new TimeSpan(mockAppointmentStartHour, mockAppointmentStartMinutes, mockAppointmentStartSeconds);
            DateTime mockAppointmentEnd = mockAppointmentStart.AddHours(mockAppointmentHoursDuration);

            appointment.StartTime = appointmentStart;
            appointment.EndTime = appointmentEnd;

            Mock<IAppointment> mockAppointmentThatColliding = new Mock<IAppointment>();
            mockAppointmentThatColliding.SetupGet(appointment => appointment.StartTime).Returns(mockAppointmentStart);
            mockAppointmentThatColliding.SetupGet(appointment => appointment.EndTime).Returns(mockAppointmentEnd);

            // Act & Assert
            Assert.IsTrue(appointment.IsCollidingWith(mockAppointmentThatColliding.Object));
        }

        [Test]
        public void IsCollidingWith_AppointmentThatNotColliding_ReturnsFalse()
        {
            // Arrange
            const int appointmentStartHour = 15;
            const int appointmentStartMinutes = 00;
            const int appointmentStartSeconds = 00;
            const int appointmentHoursDuration = 1;

            const int mockAppointmentStartHour = 8;
            const int mockAppointmentStartMinutes = 00;
            const int mockAppointmentStartSeconds = 00;
            const int mockAppointmentHoursDuration = 1;

            DateTime appointmentStart = DateTime.Now.Date + new TimeSpan(appointmentStartHour, appointmentStartMinutes, appointmentStartSeconds);
            DateTime appointmentEnd = appointmentStart.AddHours(appointmentHoursDuration);

            DateTime mockAppointmentStart = DateTime.Now.Date + new TimeSpan(mockAppointmentStartHour, mockAppointmentStartMinutes, mockAppointmentStartSeconds);
            DateTime mockAppointmentEnd = mockAppointmentStart.AddHours(mockAppointmentHoursDuration);

            appointment.StartTime = appointmentStart;
            appointment.EndTime = appointmentEnd;

            Mock<IAppointment> mockAppointmentThatNotColliding = new Mock<IAppointment>();
            mockAppointmentThatNotColliding.SetupGet(appointment => appointment.StartTime).Returns(mockAppointmentStart);
            mockAppointmentThatNotColliding.SetupGet(appointment => appointment.EndTime).Returns(mockAppointmentEnd);

            // Act & Assert
            Assert.IsFalse(appointment.IsCollidingWith(mockAppointmentThatNotColliding.Object));
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        [TestCase(validUserNameThatHasNotPermissions)]
        public void HasReadPermissions_UserNameThatHasNotPermissions_ReturnsFalse(string UserNameThatHasNotPermissions)
        {
            // Act & Assert
            Assert.IsFalse(appointment.HasReadPermissions(UserNameThatHasNotPermissions));
        }

        [Test]
        public void HasReadPermissions_OwnerUsername_ReturnsTrue()
        {
            // Arrange
            const string ownerName = "appointment owner name";
            appointment.OwnerUserName = ownerName;

            // Act & Assert
            Assert.IsTrue(appointment.HasReadPermissions(ownerName));
        }

        [Test]
        public void HasReadPermissions_GuestUserame_ReturnsTrue()
        {
            // Arrange       
            const string guestName = "appointment guest name";

            List<string> guestsUserNamesList = new List<string>()
            {
                guestName
            };

            appointment.AssignGuests(guestsUserNamesList);

            // Act & Assert
            Assert.IsTrue(appointment.HasReadPermissions(guestName));
        }

        [Test]
        public void IsOwner_OwnerUsername_ReturnsTrue()
        {
            // Arrange
            const string ownerUsername = "appointment owner name";
            appointment.OwnerUserName = ownerUsername;

            // Act & Assert
            Assert.IsTrue(appointment.IsOwner(ownerUsername));
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        [TestCase(validUserNameThatIsNotOwner)]
        public void IsOwner_UserNameThatIsNotOwner_ReturnsFalse(string userNameThatIsNotOwner)
        {
            // Arrange
            const string ownerUsername = "owner name";
            appointment.OwnerUserName = ownerUsername;

            // Act & Assert
            Assert.IsFalse(appointment.IsOwner(userNameThatIsNotOwner));
        }

        [Test]
        public void IsGuest_GuestName_ReturnsTrue()
        {
            // Arrange
            const string guestUserName = "appointment guest username";

            List<string> guestList = new List<string>()
            {
                guestUserName
            };
            appointment.AssignGuests(guestList);


            // Act & Assert
            Assert.IsTrue(appointment.IsGuest(guestUserName));
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        [TestCase(validUserNameThatIsNotGuest)]
        public void IsGuest_UserNameThatIsNotGuest_ReturnsFalse(string userNameThatIsNotGuest)
        {
            // Act & Assert
            Assert.IsFalse(appointment.IsGuest(userNameThatIsNotGuest));
        }
        #endregion
    }
}