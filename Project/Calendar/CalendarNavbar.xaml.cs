using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calendar
{
    /// <summary>
    /// Lógica de interacción para CalendarNavbar.xaml
    /// </summary>
    public partial class CalendarNavbar : UserControl
    {
        #region Constants
        private const string monthViewOption = "Vista Mensual";
        private const string weekViewOption = "Vista Semanal";
        private const string currentBodyContentResourceName = "bodyContent";
        private const string navBarMonthFormat = "MMMM yyyy";
        private const string dayNumberResourceKeyPrefix = "dayResource";
        private const string columnTitleResourceKeyPrefix = "WeekColumnTitle";
        private const string monthAndYearResourceName = "monthAndYear";
        private const string displayedDateResourceName = "displayedDate";
        private const string dayNumberResourceBlankValue = "";
        private const int iterationIndexOffset = 1;
        private const int gridRowIndexOffset = 1;
        private const int firstDayNumberInMonth = 1;
        private const int gridColumnIndexOffset = 1;
        private const int numberOfCellsInGrid = 42;
        private const int numberOfMonthsToAdvance = 1;
        private const int numberOfMonthToGoBack = -1;
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Methods
        public CalendarNavbar()
        {
            InitializeComponent();
            SetDisplayedDateResourceValue(DateTime.Now);
            AssignValueToMonthAndYearResource(GetDisplayedDateResourceValue());
        }

        private void CurrentCalendarViewOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedCalendarViewOption = GetSelectedCalendarView();
            SetBodyContentResourceValue(selectedCalendarViewOption);
        }

        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            string currentSelectedCalendarViewOption = GetSelectedCalendarView();
            DateTime dateToDisplay = GetDisplayedDateResourceValue();
            if (currentSelectedCalendarViewOption == monthViewOption)
            {
                dateToDisplay = dateToDisplay.AddMonths(numberOfMonthToGoBack);
            }
            else if (currentSelectedCalendarViewOption == weekViewOption)
            {
                dateToDisplay = dateToDisplay.AddDays(numberOfMonthToGoBack * Utilities.daysInWeek);
            }
            SetDisplayedDateAndAssingAllResources(dateToDisplay);
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            string currentSelectedCalendarViewOption = GetSelectedCalendarView();
            DateTime dateToDisplay = GetDisplayedDateResourceValue();
            if (currentSelectedCalendarViewOption == monthViewOption)
            {
                dateToDisplay = dateToDisplay.AddMonths(numberOfMonthsToAdvance);
            }
            else if (currentSelectedCalendarViewOption == weekViewOption)
            {
                dateToDisplay = dateToDisplay.AddDays(Utilities.daysInWeek);
            }
            SetDisplayedDateAndAssingAllResources(dateToDisplay);
        }

        private void SetDisplayedDateAndAssingAllResources(DateTime date)
        {
            SetDisplayedDateResourceValue(date);
            AssignValueToMonthAndYearResource(GetDisplayedDateResourceValue());
            AssingValuesToDayNumberResources();
            AssingValuesToDayColumnTitleResources();
        }

        private void AssignValueToMonthAndYearResource(DateTime date)
        {
            App.Current.Resources[monthAndYearResourceName] = date.ToString(navBarMonthFormat);
        }

        private void AssingValuesToDayNumberResources()
        {
            for (int i = 0; i < numberOfCellsInGrid; i++)
            {
                string dayNumberResourceKey = dayNumberResourceKeyPrefix + i.ToString();
                string dayNumberResourceValue = dayNumberResourceBlankValue;
                int candidateDayNumber = i - GetfirstDayGridColumnIndex() + iterationIndexOffset;
                Point dayElementGridCoordinates = GetGridCoordinatesByIterationIndex(i);
                if (IsDayNumberInDisplayedMonth(candidateDayNumber, dayElementGridCoordinates))
                {
                    dayNumberResourceValue = candidateDayNumber.ToString();
                }
                App.Current.Resources[dayNumberResourceKey] = dayNumberResourceValue;
            }
        }

        private void AssingValuesToDayColumnTitleResources()
        {
            for (int i = 1; i <= Utilities.daysInWeek; i++)
            {
                DateTime displayedDate = GetDisplayedDateResourceValue();
                int dayOfWeek = Utilities.GetDayNumberInWeek(displayedDate);
                displayedDate = displayedDate.AddDays(Utilities.negativeMultiplier * dayOfWeek + i);
                string columnTitleResourceKey = columnTitleResourceKeyPrefix + i.ToString();
                string columnTitleResourceValue = Utilities.GetNameOfDayInSpanish(displayedDate) + Utilities.blankSpace + displayedDate.Day.ToString();
                App.Current.Resources[columnTitleResourceKey] = columnTitleResourceValue;
            }
        }

        private bool IsDayNumberInDisplayedMonth(int candidateDayNumber, Point dayElementGridCoordinates)
        {
            const int firstDayRowIndex = 1;
            DateTime displayedDate = GetDisplayedDateResourceValue();
            bool isFirstDayRow = dayElementGridCoordinates.Y == firstDayRowIndex;
            bool isNotFirstDayRow = dayElementGridCoordinates.Y > firstDayRowIndex;
            bool isFirstDayColumnOrLater = dayElementGridCoordinates.X >= GetfirstDayGridColumnIndex();
            bool isDisplayableDayElementOfFirstRow = isFirstDayRow && isFirstDayColumnOrLater;
            bool isCandidateDayNumberInDisplayedMonth = candidateDayNumber <= GetNumberOfDaysOfMonth(displayedDate);
            bool isDisplayableDayElementOfRemainsRows = isNotFirstDayRow && isCandidateDayNumberInDisplayedMonth;
            return (isDisplayableDayElementOfFirstRow || isDisplayableDayElementOfRemainsRows);
        }

        private int GetNumberOfDaysOfMonth(DateTime date)
        {
            DateTime displayedDate = date;
            int numberOfDaysOfDisplayedMonth = DateTime.DaysInMonth(displayedDate.Year, displayedDate.Month);
            return numberOfDaysOfDisplayedMonth;
        }

        private int GetfirstDayGridColumnIndex()
        {
            DateTime displayedDate = GetDisplayedDateResourceValue();
            DateTime firstDayOfDisplayedMonth = new DateTime(displayedDate.Year, displayedDate.Month, firstDayNumberInMonth);
            int firstDayGridColumnIndex = Utilities.GetDayNumberInWeek(firstDayOfDisplayedMonth) - gridColumnIndexOffset;
            return firstDayGridColumnIndex;
        }

        private Point GetGridCoordinatesByIterationIndex(int iterationIndex)
        {
            int gridColumn = (iterationIndex) % Utilities.daysInWeek;
            int gridRow = (iterationIndex / Utilities.daysInWeek) + gridRowIndexOffset;
            return new Point(gridColumn, gridRow);
        }

        private DateTime GetDisplayedDateResourceValue()
        {
            return (DateTime)App.Current.Resources[displayedDateResourceName];
        }

        private void SetDisplayedDateResourceValue(DateTime dateToDisplay)
        {
            App.Current.Resources[displayedDateResourceName] = dateToDisplay;
        }

        private void SetBodyContentResourceValue(string selectedCalendarViewOption)
        {
            switch (selectedCalendarViewOption)
            {
                case weekViewOption:
                    WeekBody weekBody = new WeekBody();
                    App.Current.Resources[currentBodyContentResourceName] = weekBody;
                    break;
                default:
                    MonthBody monthBody = new MonthBody();
                    App.Current.Resources[currentBodyContentResourceName] = monthBody;
                    break;
            }

        }

        private string GetSelectedCalendarView()
        {
            const int initOfValueSubstring = 38;
            return CurrentCalendarViewOptions.SelectedValue.ToString().Substring(initOfValueSubstring);
        }
        #endregion
    }
}
