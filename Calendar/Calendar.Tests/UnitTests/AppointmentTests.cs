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
        private const string nullUserName = null;
        private const string empty = "";
        private const string aSpace = " ";
        private const string moreThanOneSpace = "    ";
        private const string validUserNameThatHasNotPermissions = "a user name that has not permissions";
        private const string validUserNameThatIsNotOwner = "a user name that is not owner";
        private const string validUserNameThatIsNotGuest = "a user name that is not guest";
        #endregion


        #region Fields
        private Appointment appointment;
        private readonly string aOwnerName = It.IsAny<string>();
        private readonly DateTime aDate = It.IsAny<DateTime>();
        #endregion


        #region Properties
        #endregion


        #region Methods
        [SetUp]
        public void Setup()
        {
            appointment = new Appointment(aDate, aOwnerName);
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
        public void StartTime_SomeStart_ReturnsSameStart()
        {
            appointment.StartTime = aDate;
            Assert.AreEqual(aDate, appointment.StartTime);
        }

        [Test]
        public void EndTime_SomeEnd_ReturnsSameEnd()
        {
            appointment.EndTime = aDate;
            Assert.AreEqual(aDate, appointment.EndTime);
        }

        [Test]
        public void IsInGarbage_DefaultValue_ReturnsFalseAsDefaultValue()
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
        public void OwnerUserName_SomeAppointmentOwner_ReturnsSameAppointmentOwner()
        {
            const string aNewOwnerUserName = "a new owner username";
            appointment.OwnerUserName = aNewOwnerUserName;
            Assert.AreEqual(aNewOwnerUserName, appointment.OwnerUserName);
        }

        [Test]
        public void GuestsUserNames_SomeAppointmentGuests_ReturnsSameAppointmentGuests()
        {
            const string aGuestUserName = "a guest name";
            const string otherGuestUserName = "other guest username";

            List<string> aGuestList = new List<string>()
            {
                aGuestUserName,
                otherGuestUserName
            };

            appointment.AssignGuests(aGuestList);

            Assert.AreEqual(aGuestList, appointment.GuestsUserNames);
        }

        [Test]
        public void IsCollidingWith_NullAppointment_ThrowsArgumentNullException()
        {
            IAppointment nullAppointment = null;

            Assert.That(
                () => appointment.IsCollidingWith(nullAppointment),
                Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void IsCollidingWith_AppointmentThatColliding_ReturnsTrue()
        {
            const int startHour = 12;
            const int startMinutes = 00;
            const int startSeconds = 00;
            const int durationInHours = 1;

            DateTime startTime = aDate.Date + new TimeSpan(startHour, startMinutes, startSeconds);
            DateTime endTime = startTime.AddHours(durationInHours);

            appointment.StartTime = startTime;
            appointment.EndTime = endTime;

            Mock<IAppointment> mockAppointmentThatColliding = new Mock<IAppointment>();

            mockAppointmentThatColliding
                .SetupGet(appointment => appointment.StartTime)
                .Returns(startTime);

            mockAppointmentThatColliding
                .SetupGet(appointment => appointment.EndTime)
                .Returns(endTime);

            Assert.IsTrue(appointment.IsCollidingWith(mockAppointmentThatColliding.Object));
        }

        [Test]
        public void IsCollidingWith_AppointmentThatNotColliding_ReturnsFalse()
        {
            
            const int startHour = 15;
            const int startMinutes = 00;
            const int startSeconds = 00;
            const int durationInHours = 1;

            const int startHourThatNotColliding = 8;
            const int startMinutesThatNotColliding = 00;
            const int startSecondsThatNotColliding = 00;
            const int durationThatNotColliding = 1;

            DateTime startTime = aDate.Date + new TimeSpan(startHour, startMinutes, startSeconds);
            DateTime endTime = startTime.AddHours(durationInHours);

            DateTime startTimeThatNotColliding = aDate.Date + new TimeSpan(startHourThatNotColliding, startMinutesThatNotColliding, startSecondsThatNotColliding);
            DateTime endTimeThatNotColliding = startTimeThatNotColliding.AddHours(durationThatNotColliding);

            appointment.StartTime = startTime;
            appointment.EndTime = endTime;

            Mock<IAppointment> mockAppointmentThatNotColliding = new Mock<IAppointment>();

            mockAppointmentThatNotColliding
                .SetupGet(appointment => appointment.StartTime)
                .Returns(startTimeThatNotColliding);

            mockAppointmentThatNotColliding
                .SetupGet(appointment => appointment.EndTime)
                .Returns(endTimeThatNotColliding);

            Assert.IsFalse(appointment.IsCollidingWith(mockAppointmentThatNotColliding.Object));
        }

        [TestCase(nullUserName)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        [TestCase(validUserNameThatHasNotPermissions)]
        public void HasReadPermissions_UserNameThatHasNotPermissions_ReturnsFalse(string UserNameThatHasNotPermissions)
        {
            Assert.IsFalse(appointment.HasReadPermissions(UserNameThatHasNotPermissions));
        }

        [Test]
        public void HasReadPermissions_OwnerUserName_ReturnsTrue()
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
        public void IsOwner_OwnerUserName_ReturnsTrue()
        {
            const string ownerUserName = "appointment owner name";
            appointment.OwnerUserName = ownerUserName;
            Assert.IsTrue(appointment.IsOwner(ownerUserName));
        }

        [TestCase(nullUserName)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        [TestCase(validUserNameThatIsNotOwner)]
        public void IsOwner_UserNameThatIsNotOwner_ReturnsFalse(string userNameThatIsNotOwner)
        {
            const string ownerUserName = "owner name";
            appointment.OwnerUserName = ownerUserName;
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

        [TestCase(nullUserName)]
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