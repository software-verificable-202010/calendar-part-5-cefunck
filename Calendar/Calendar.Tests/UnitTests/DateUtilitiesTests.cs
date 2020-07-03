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
            
            DateTime aDateToDisplay = It.IsAny<DateTime>();
            DateUtilities.DisplayedDate = aDateToDisplay;

            
            Assert.AreEqual(aDateToDisplay, DateUtilities.DisplayedDate);
        }

        [Test]
        public void DayNames_NoInput_ReturnsDayNames()
        {
            
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

            
            Assert.AreEqual(dayNames,DateUtilities.DayNames);
        }

        [Test]
        public void SetDisplayedDateTo_SomeDateTime_ReturnsSameDateTime()
        {
            
            DateTime aDateTime = It.IsAny<DateTime>();

            
            DateUtilities.SetDisplayedDateTo(aDateTime);

            
            Assert.AreEqual(aDateTime, DateUtilities.DisplayedDate);
        }

        [Test]
        public void GetDayNumberInWeek_MondayDateTime_ReturnsOne()
        {
            
            int expectedDayNumber = 1;
            DateTime aMonday = new DateTime(2020, 6, 29);


            
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aMonday));
        }

        [Test]
        public void GetDayNumberInWeek_TuesdayDateTime_ReturnsTwoo()
        {
            
            int expectedDayNumber = 2;
            DateTime aTuesday = new DateTime(2020, 6, 30);

            
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aTuesday));
        }

        [Test]
        public void GetDayNumberInWeek_WednesdayDateTime_ReturnsThree()
        {
            
            int expectedDayNumber = 3;
            DateTime aWednesday = new DateTime(2020, 7, 1);

            
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aWednesday));
        }

        [Test]
        public void GetDayNumberInWeek_ThursdayDateTime_ReturnsFour()
        {
            
            int expectedDayNumber = 4;
            DateTime aThursday = new DateTime(2020, 7, 2);

            
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aThursday));
        }

        [Test]
        public void GetDayNumberInWeek_FridayDateTime_ReturnsFive()
        {
            
            int expectedDayNumber = 5;
            DateTime aFriday = new DateTime(2020, 7, 3);

            
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aFriday));
        }

        [Test]
        public void GetDayNumberInWeek_SaturdayDateTime_ReturnsSix()
        {
            
            int expectedDayNumber = 6;
            DateTime aSaturday = new DateTime(2020, 7, 4);

            
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aSaturday));
        }

        [Test]
        public void GetDayNumberInWeek_SundayDateTime_ReturnsSeven()
        {
            
            int expectedDayNumber = 7;
            DateTime aSunday = new DateTime(2020, 7, 5);

            
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aSunday));
        }

        [Test]
        public void GetNameOfDayInSpanish_MondayDateTime_ReturnsMondayInSpansh()
        {
            
            const string expectedDayName = "Lunes";
            DateTime aMonday = new DateTime(2020, 6, 29);

            
            Assert.AreEqual(expectedDayName, DateUtilities.GetNameOfDayInSpanish(aMonday));
        }

        [Test]
        public void GetNameOfDayInSpanish_TuesdayDateTime_ReturnsTuesdayInSpansh()
        {
            
            const string expectedDayName = "Martes";
            DateTime aTuesday = new DateTime(2020, 6, 30);

            
            Assert.AreEqual(expectedDayName, DateUtilities.GetNameOfDayInSpanish(aTuesday));
        }

        [Test]
        public void GetNameOfDayInSpanish_WednesdayDateTime_ReturnsWednesdayInSpanish()
        {
            
            const string expectedDayName = "Miércoles";
            DateTime aWednesday = new DateTime(2020, 7, 1);

            
            Assert.AreEqual(expectedDayName, DateUtilities.GetNameOfDayInSpanish(aWednesday));
        }

        [Test]
        public void GetNameOfDayInSpanish_ThursdayDateTime_ReturnsThursdayInSpansh()
        {
            
            const string expectedDayName = "Jueves";
            DateTime aThursday = new DateTime(2020, 7, 2);

            
            Assert.AreEqual(expectedDayName, DateUtilities.GetNameOfDayInSpanish(aThursday));
        }

        [Test]
        public void GetNameOfDayInSpanish_FridayDateTime_ReturnsFridayInSpansh()
        {
            
            const string expectedDayName = "Viernes";
            DateTime aFriday = new DateTime(2020, 7, 3);

            
            Assert.AreEqual(expectedDayName, DateUtilities.GetNameOfDayInSpanish(aFriday));
        }

        [Test]
        public void GetNameOfDayInSpanish_SaturdayDateTime_ReturnsSaturdayInSpanish()
        {
            
            const string expectedDayName = "Sábado";
            DateTime aSaturday = new DateTime(2020, 7, 4);

            
            Assert.AreEqual(expectedDayName, DateUtilities.GetNameOfDayInSpanish(aSaturday));
        }

        [Test]
        public void GetNameOfDayInSpanish_SundayDateTime_ReturnsSundayInSpanish()
        {
            
            const string expectedDayName = "Domingo";
            DateTime aSunday = new DateTime(2020, 7, 5);

            
            Assert.AreEqual(expectedDayName, DateUtilities.GetNameOfDayInSpanish(aSunday));
        }


        #endregion
    }
}
