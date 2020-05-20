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
    /// Lógica de interacción para MonthBody.xaml
    /// </summary>
    public partial class MonthBody : UserControl
    {
        #region Constants
        private const string dayElementNamePrefix = "dayElement";
        private const string dayNumberResourceKeyPrefix = "dayResource";
        private const string displayedDateResourceName = "displayedDate";
        private const string dayNumberResourceBlankValue = "";
        private const int iterationIndexOffset = 1;
        private const int gridRowIndexOffset = 1;
        private const int gridColumnIndexOffset = 1;
        private const int firstDayNumberInMonth = 1;
        private const int numberOfCellsInGrid = 42;
        private const int saturdayGridColumnIndex = 5;
        private const int sundayGridColumnIndex = 6;
        #endregion

        #region Fields
        private readonly Brush highlightColor = Brushes.Red;
        #endregion

        #region Properties
        #endregion

        #region Methods
        public MonthBody()
        {
            InitializeComponent();
            GenerateDayNumberResources();
            AssingValuesToDayNumberResources(GetDisplayedDateResourceValue());
            List<TextBlock> dayElements = CreateDayElements();
            InsertDayElementsToGrid(dayElements);
            HighLightWeekends();
        }

        private void GenerateDayNumberResources()
        {
            for (int i = 0; i < numberOfCellsInGrid; i++)
            {
                string dayNumberResourceKey = dayNumberResourceKeyPrefix + i.ToString();
                string dayNumberResourceValue = dayNumberResourceBlankValue;
                if (App.Current.Resources[dayNumberResourceKey] == null)
                {
                    App.Current.Resources.Add(dayNumberResourceKey, dayNumberResourceValue);
                }
            }
        }

        private void AssingValuesToDayNumberResources(DateTime displayedDate)
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

        private List<TextBlock> CreateDayElements()
        {
            List<TextBlock> dayElements = new List<TextBlock>();
            for (int i = 0; i < numberOfCellsInGrid; i++)
            {
                TextBlock dayElement = new TextBlock();
                dayElement.Name = dayElementNamePrefix + i.ToString();
                string dayNumberResourceKey = dayNumberResourceKeyPrefix + i.ToString();
                dayElement.SetResourceReference(TextBlock.TextProperty, dayNumberResourceKey);
                Point dayElementGridCoordinates = GetGridCoordinatesByIterationIndex(i);
                dayElement.SetValue(Grid.ColumnProperty, (int)dayElementGridCoordinates.X);
                dayElement.SetValue(Grid.RowProperty, (int)dayElementGridCoordinates.Y);
                dayElements.Add(dayElement);
            }
            return dayElements;
        }

        private void InsertDayElementsToGrid(List<TextBlock> dayElements)
        {
            foreach (TextBlock dayElement in dayElements)
            {
                BodyGrid.Children.Add(dayElement);
            }
        }

        private void HighLightWeekends()
        {
            foreach (TextBlock dayElement in BodyGrid.Children)
            {
                int dayElementGridColumnIndex = (int)dayElement.GetValue(Grid.ColumnProperty);
                if (dayElementGridColumnIndex == saturdayGridColumnIndex || dayElementGridColumnIndex == sundayGridColumnIndex)
                {
                    dayElement.Foreground = highlightColor;
                }
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

        private DateTime GetDisplayedDateResourceValue()
        {
            return (DateTime)App.Current.Resources[displayedDateResourceName];
        }

        private Point GetGridCoordinatesByIterationIndex(int iterationIndex)
        {
            int gridColumn = (iterationIndex) % Utilities.daysInWeek;
            int gridRow = (iterationIndex / Utilities.daysInWeek) + gridRowIndexOffset;
            return new Point(gridColumn, gridRow); ;
        }
        #endregion
    }
}
