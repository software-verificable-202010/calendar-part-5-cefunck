using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Tests.UnitTests
{
    class DateUtilitiesTests
    {
        #region Constants
        #endregion


        #region Fields
        #endregion


        #region Properties
        #endregion


        #region Methods
        [SetUp]
        public void SetUp()
        {

        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void DisplayedDate_SomeDateToDisplay_ReturnsSameDate()
        {
            // Arrange
            DateTime aDateToDisplay = It.IsAny<DateTime>();
            DateUtilities.DisplayedDate = aDateToDisplay;

            // Act & Assert
            Assert.AreEqual(aDateToDisplay, DateUtilities.DisplayedDate);
        }

        [Test]
        public void DayNames_NoInput_ReturnsDayNames()
        {
            // Arrange
            const string mondayName = "Lunes";
            const string tuesdayName = "Martes";
            const string wednesdayName = "Miércoles";
            const string thursdayName = "Jueves";
            const string fridayName = "Viernes";
            const string saturdayName = "Sábado";
            const string sundayName = "Domingo";

            List<string> dayNames = new List<string>()
            {
                mondayName,
                tuesdayName,
                wednesdayName,
                thursdayName,
                fridayName,
                saturdayName,
                sundayName
            };

            // Assert
            Assert.AreEqual(dayNames,DateUtilities.DayNames);
        }

        [Test]
        public void SetDisplayedDateTo_SomeDateTime_ReturnsSameDateTime()
        {
            // Arrange
            DateTime aDateTime = It.IsAny<DateTime>();

            // Act
            DateUtilities.SetDisplayedDateTo(aDateTime);

            // Assert
            Assert.AreEqual(aDateTime, DateUtilities.DisplayedDate);
        }

        [Test]
        public void GetDayNumberInWeek_MondayDateTime_ReturnsOne()
        {
            // Arrange
            int expectedDayNumber = 1;
            DateTime aMonday = new DateTime(2020, 6, 29);


            // Act & Assert
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aMonday));
        }

        [Test]
        public void GetDayNumberInWeek_TuesdayDateTime_ReturnsTwoo()
        {
            // Arrange
            int expectedDayNumber = 2;
            DateTime aTuesday = new DateTime(2020, 6, 30);

            // Act & Assert
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aTuesday));
        }

        [Test]
        public void GetDayNumberInWeek_WednesdayDateTime_ReturnsThree()
        {
            // Arrange
            int expectedDayNumber = 3;
            DateTime aWednesday = new DateTime(2020, 7, 1);

            // Act & Assert
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aWednesday));
        }

        [Test]
        public void GetDayNumberInWeek_ThursdayDateTime_ReturnsFour()
        {
            // Arrange
            int expectedDayNumber = 4;
            DateTime aThursday = new DateTime(2020, 7, 2);

            // Act & Assert
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aThursday));
        }

        [Test]
        public void GetDayNumberInWeek_FridayDateTime_ReturnsFive()
        {
            // Arrange
            int expectedDayNumber = 5;
            DateTime aFriday = new DateTime(2020, 7, 3);

            // Act & Assert
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aFriday));
        }

        [Test]
        public void GetDayNumberInWeek_SaturdayDateTime_ReturnsSix()
        {
            // Arrange
            int expectedDayNumber = 6;
            DateTime aSaturday = new DateTime(2020, 7, 4);

            // Act & Assert
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aSaturday));
        }

        [Test]
        public void GetDayNumberInWeek_SundayDateTime_ReturnsSeven()
        {
            // Arrange
            int expectedDayNumber = 7;
            DateTime aSunday = new DateTime(2020, 7, 5);

            // Act & Assert
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aSunday));
        }

        [Test]
        public void GetNameOfDayInSpanish_MondayDateTime_ReturnsMondayInSpansh()
        {
            // Arrange
            const string expectedDayName = "Lunes";
            DateTime aMonday = new DateTime(2020, 6, 29);

            // Act & Assert
            Assert.AreEqual(expectedDayName, DateUtilities.GetNameOfDayInSpanish(aMonday));
        }

        [Test]
        public void GetNameOfDayInSpanish_TuesdayDateTime_ReturnsTuesdayInSpansh()
        {
            // Arrange
            const string expectedDayName = "Martes";
            DateTime aTuesday = new DateTime(2020, 6, 30);

            // Act & Assert
            Assert.AreEqual(expectedDayName, DateUtilities.GetNameOfDayInSpanish(aTuesday));
        }

        [Test]
        public void GetNameOfDayInSpanish_WednesdayDateTime_ReturnsWednesdayInSpanish()
        {
            // Arrange
            const string expectedDayName = "Miércoles";
            DateTime aWednesday = new DateTime(2020, 7, 1);

            // Act & Assert
            Assert.AreEqual(expectedDayName, DateUtilities.GetNameOfDayInSpanish(aWednesday));
        }

        [Test]
        public void GetNameOfDayInSpanish_ThursdayDateTime_ReturnsThursdayInSpansh()
        {
            // Arrange
            const string expectedDayName = "Jueves";
            DateTime aThursday = new DateTime(2020, 7, 2);

            // Act & Assert
            Assert.AreEqual(expectedDayName, DateUtilities.GetNameOfDayInSpanish(aThursday));
        }

        [Test]
        public void GetNameOfDayInSpanish_FridayDateTime_ReturnsFridayInSpansh()
        {
            // Arrange
            const string expectedDayName = "Viernes";
            DateTime aFriday = new DateTime(2020, 7, 3);

            // Act & Assert
            Assert.AreEqual(expectedDayName, DateUtilities.GetNameOfDayInSpanish(aFriday));
        }

        [Test]
        public void GetNameOfDayInSpanish_SaturdayDateTime_ReturnsSaturdayInSpanish()
        {
            // Arrange
            const string expectedDayName = "Sábado";
            DateTime aSaturday = new DateTime(2020, 7, 4);

            // Act & Assert
            Assert.AreEqual(expectedDayName, DateUtilities.GetNameOfDayInSpanish(aSaturday));
        }

        [Test]
        public void GetNameOfDayInSpanish_SundayDateTime_ReturnsSundayInSpanish()
        {
            // Arrange
            const string expectedDayName = "Domingo";
            DateTime aSunday = new DateTime(2020, 7, 5);

            // Act & Assert
            Assert.AreEqual(expectedDayName, DateUtilities.GetNameOfDayInSpanish(aSunday));
        }


        #endregion
    }
}
