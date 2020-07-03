using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Calendar.Tests.UnitTests
{
    class DateUtilitiesTests
    {
        #region Constants
        private const int year2020 = 2020;
        private const int july = 7;
        private const int mondayInJuly2020 = 6;
        private const int tuesdayInJuly2020 = 7;
        private const int wednesdayInJuly2020 = 8;
        private const int thursdayInJuly2020 = 9;
        private const int fridayInJuly2020 = 10;
        private const int saturdayInJuly2020 = 11;
        private const int sundayInJuly2020 = 12;
        private const string mondayNameInSpanish = "Lunes";
        private const string tuesdayNameInSpanish = "Martes";
        private const string wednesdayNameInSpanish = "Miércoles";
        private const string thursdayNameInSpanish = "Jueves";
        private const string fridayNameInSpanish = "Viernes";
        private const string saturdayNameInSpanish = "Sábado";
        private const string sundayNameInSpanish = "Domingo";
        #endregion


        #region Fields
        private readonly DateTime aMonday = new DateTime(year2020, july, mondayInJuly2020);
        private readonly DateTime aTuesday = new DateTime(year2020, july, tuesdayInJuly2020);
        private readonly DateTime aWednesday = new DateTime(year2020, july, wednesdayInJuly2020);
        private readonly DateTime aThursday = new DateTime(year2020, july, thursdayInJuly2020);
        private readonly DateTime aFriday = new DateTime(year2020, july, fridayInJuly2020);
        private readonly DateTime aSaturday = new DateTime(year2020, july, saturdayInJuly2020);
        private readonly DateTime aSunday = new DateTime(year2020, july, sundayInJuly2020);
        #endregion


        #region Properties
        #endregion


        #region Methods
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
            List<string> dayNames = new List<string>()
            {
                mondayNameInSpanish,
                tuesdayNameInSpanish,
                wednesdayNameInSpanish,
                thursdayNameInSpanish,
                fridayNameInSpanish,
                saturdayNameInSpanish,
                sundayNameInSpanish
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
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aMonday));
        }

        [Test]
        public void GetDayNumberInWeek_TuesdayDateTime_ReturnsTwoo()
        {
            int expectedDayNumber = 2;   
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aTuesday));
        }

        [Test]
        public void GetDayNumberInWeek_WednesdayDateTime_ReturnsThree()
        {
            int expectedDayNumber = 3;
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aWednesday));
        }

        [Test]
        public void GetDayNumberInWeek_ThursdayDateTime_ReturnsFour()
        {
            int expectedDayNumber = 4;
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aThursday));
        }

        [Test]
        public void GetDayNumberInWeek_FridayDateTime_ReturnsFive()
        {
            int expectedDayNumber = 5;
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aFriday));
        }

        [Test]
        public void GetDayNumberInWeek_SaturdayDateTime_ReturnsSix()
        {
            int expectedDayNumber = 6;
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aSaturday));
        }

        [Test]
        public void GetDayNumberInWeek_SundayDateTime_ReturnsSeven()
        {
            int expectedDayNumber = 7;
            Assert.AreEqual(expectedDayNumber, DateUtilities.GetDayNumberInWeek(aSunday));
        }

        [Test]
        public void GetNameOfDayInSpanish_MondayDateTime_ReturnsMondayInSpansh()
        {
            Assert.AreEqual(mondayNameInSpanish, DateUtilities.GetNameOfDayInSpanish(aMonday));
        }

        [Test]
        public void GetNameOfDayInSpanish_TuesdayDateTime_ReturnsTuesdayInSpansh()
        {
            Assert.AreEqual(tuesdayNameInSpanish, DateUtilities.GetNameOfDayInSpanish(aTuesday));
        }

        [Test]
        public void GetNameOfDayInSpanish_WednesdayDateTime_ReturnsWednesdayInSpanish()
        {
            Assert.AreEqual(wednesdayNameInSpanish, DateUtilities.GetNameOfDayInSpanish(aWednesday));
        }

        [Test]
        public void GetNameOfDayInSpanish_ThursdayDateTime_ReturnsThursdayInSpansh()
        {
            Assert.AreEqual(thursdayNameInSpanish, DateUtilities.GetNameOfDayInSpanish(aThursday));
        }

        [Test]
        public void GetNameOfDayInSpanish_FridayDateTime_ReturnsFridayInSpansh()
        {
            Assert.AreEqual(fridayNameInSpanish, DateUtilities.GetNameOfDayInSpanish(aFriday));
        }

        [Test]
        public void GetNameOfDayInSpanish_SaturdayDateTime_ReturnsSaturdayInSpanish()
        {
            Assert.AreEqual(saturdayNameInSpanish, DateUtilities.GetNameOfDayInSpanish(aSaturday));
        }

        [Test]
        public void GetNameOfDayInSpanish_SundayDateTime_ReturnsSundayInSpanish()
        {
            Assert.AreEqual(sundayNameInSpanish, DateUtilities.GetNameOfDayInSpanish(aSunday));
        }

        #endregion
    }
}
