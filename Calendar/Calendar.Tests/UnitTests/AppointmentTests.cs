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
            
            const string aTitle = "a title";
            appointment.Title = aTitle;

            
            Assert.AreEqual(aTitle, appointment.Title);
        }

        [Test]
        public void Description_SomeDescription_ReturnsSameDescription()
        {
            
            const string aDescription = "a descrption";
            appointment.Description = aDescription;

            
            Assert.AreEqual(aDescription, appointment.Description);
        }

        [Test]
        public void Start_SomeStart_ReturnsSameStart()
        {
            
            DateTime newStart = DateTime.Now;
            appointment.StartTime = newStart;

            
            Assert.AreEqual(newStart, appointment.StartTime);
        }

        [Test]
        public void End_SomeEnd_ReturnsSameEnd()
        {
            
            DateTime end = DateTime.Now;
            appointment.EndTime = end;

            
            Assert.AreEqual(end, appointment.EndTime);
        }

        [Test]
        public void IsInGarbage_DefaultValue_ReturnsDefaultValue()
        {
            
            Assert.IsFalse(appointment.IsInGarbage);
        }

        [Test]
        public void IsInGarbage_SomeValue_ReturnsSameValue()
        {
            
            appointment.IsInGarbage = true;

            
            Assert.IsTrue(appointment.IsInGarbage);
        }

        [Test]
        public void Owner_SomeAppointmentOwner_ReturnsSameAppointmentOwner()
        {
            
            const string aNewOwnerUsername = "a new owner username";
            appointment.OwnerUserName = aNewOwnerUsername;

            
            Assert.AreEqual(aNewOwnerUsername, appointment.OwnerUserName);
        }

        [Test]
        public void Guests_SomeAppointmentGuests_ReturnsSameAppointmentGuests()
        {
            
            const string aGuestUsername = "a guest name";
            const string otherGuestUsername = "other guest username";

            List<string> aGuestList = new List<string>()
            {
                aGuestUsername,
                otherGuestUsername
            };
            appointment.AssignGuests(aGuestList);

            
            Assert.AreEqual(aGuestList, appointment.GuestsUserNames);
        }

        [Test]
        public void IsCollidingWith_NullAppointment_ReturnsArgumentNullException()
        {
            
            IAppointment nullAppointment = null;

            
            Assert.That(() => appointment.IsCollidingWith(nullAppointment), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void IsCollidingWith_AppointmentThatColliding_ReturnsTrue()
        {
            
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

            
            Assert.IsTrue(appointment.IsCollidingWith(mockAppointmentThatColliding.Object));
        }

        [Test]
        public void IsCollidingWith_AppointmentThatNotColliding_ReturnsFalse()
        {
            
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

            
            Assert.IsFalse(appointment.IsCollidingWith(mockAppointmentThatNotColliding.Object));
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        [TestCase(validUserNameThatHasNotPermissions)]
        public void HasReadPermissions_UserNameThatHasNotPermissions_ReturnsFalse(string UserNameThatHasNotPermissions)
        {
            
            Assert.IsFalse(appointment.HasReadPermissions(UserNameThatHasNotPermissions));
        }

        [Test]
        public void HasReadPermissions_OwnerUsername_ReturnsTrue()
        {
            
            const string ownerName = "appointment owner name";
            appointment.OwnerUserName = ownerName;

            
            Assert.IsTrue(appointment.HasReadPermissions(ownerName));
        }

        [Test]
        public void HasReadPermissions_GuestUserame_ReturnsTrue()
        {
                   
            const string guestName = "appointment guest name";

            List<string> guestsUserNamesList = new List<string>()
            {
                guestName
            };

            appointment.AssignGuests(guestsUserNamesList);

            
            Assert.IsTrue(appointment.HasReadPermissions(guestName));
        }

        [Test]
        public void IsOwner_OwnerUsername_ReturnsTrue()
        {
            
            const string ownerUsername = "appointment owner name";
            appointment.OwnerUserName = ownerUsername;

            
            Assert.IsTrue(appointment.IsOwner(ownerUsername));
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        [TestCase(validUserNameThatIsNotOwner)]
        public void IsOwner_UserNameThatIsNotOwner_ReturnsFalse(string userNameThatIsNotOwner)
        {
            
            const string ownerUsername = "owner name";
            appointment.OwnerUserName = ownerUsername;

            
            Assert.IsFalse(appointment.IsOwner(userNameThatIsNotOwner));
        }

        [Test]
        public void IsGuest_GuestName_ReturnsTrue()
        {
            
            const string guestUserName = "appointment guest username";

            List<string> guestList = new List<string>()
            {
                guestUserName
            };
            appointment.AssignGuests(guestList);


            
            Assert.IsTrue(appointment.IsGuest(guestUserName));
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        [TestCase(validUserNameThatIsNotGuest)]
        public void IsGuest_UserNameThatIsNotGuest_ReturnsFalse(string userNameThatIsNotGuest)
        {
            
            Assert.IsFalse(appointment.IsGuest(userNameThatIsNotGuest));
        }
        #endregion
    }
}