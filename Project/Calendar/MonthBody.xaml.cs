using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private List<MonthDayElement> dayElements = new List<MonthDayElement>();
        private List<Appointment> monthAppointmens;
        #endregion

        #region Properties
        public List<Appointment> MonthAppointments
        {
            get
            {
                return monthAppointmens;
            }
        }
        #endregion

        #region Methods
        public MonthBody()
        {
            InitializeComponent();
        }
        public void Refresh() 
        {
            GenerateDayElements();
            InsertBodyElements();
            RefreshDayElements();
            HighLightWeekends();
        }
        private void GenerateDayElements()
        {
            dayElements = new List<MonthDayElement>();
            for (int i = 0; i < numberOfCellsInGrid; i++)
            {
                int candidateDayNumber = i - GetfirstDayGridColumnIndex() + iterationIndexOffset;
                Point dayElementGridCoordinates = GetGridCoordinatesByIterationIndex(i);
                if (IsDayNumberInDisplayedMonth(candidateDayNumber, dayElementGridCoordinates))
                {
                    int year = Utilities.DisplayedDate.Year;
                    int month = Utilities.DisplayedDate.Month;
                    int day = candidateDayNumber;
                    DateTime dayElementDate = new DateTime(year, month, day);
                    MonthDayElement dayElement = new MonthDayElement(dayElementDate);
                    dayElement.SetValue(Grid.ColumnProperty, (int)dayElementGridCoordinates.X);
                    dayElement.SetValue(Grid.RowProperty, (int)dayElementGridCoordinates.Y);
                    dayElements.Add(dayElement);
                }
                else 
                {
                    MonthDayElement dayElementBlank = new MonthDayElement();
                    dayElementBlank.SetValue(Grid.ColumnProperty, (int)dayElementGridCoordinates.X);
                    dayElementBlank.SetValue(Grid.RowProperty, (int)dayElementGridCoordinates.Y);
                    dayElements.Add(dayElementBlank);
                }
            }
        }
        private void InsertBodyElements() 
        {
            BodyGrid.Children.Clear();
            InsertWeekDaysToGrid();
            InsertDayElementsToGrid();
        }
        private void InsertWeekDaysToGrid() 
        {
            for (int i = 0; i < 7; i++)
            {
                Collection<string> dayNames = Utilities.DayNames;
                TextBlock textBlockDayName = new TextBlock
                {
                    Text = dayNames[i]
                };
                Border borderDayName = new Border();
                textBlockDayName.SetValue(Grid.ColumnProperty, i);
                borderDayName.SetValue(Grid.ColumnProperty, i);
                BodyGrid.Children.Add(textBlockDayName);
                BodyGrid.Children.Add(borderDayName);
            }
        }

        internal void AssignMonthAppointments(List<Appointment> appointments)
        {
            monthAppointmens = appointments;
        }

        private void InsertDayElementsToGrid()
        {
            foreach (MonthDayElement dayElement in dayElements)
            {
                BodyGrid.Children.Add(dayElement);
            }
        }
        private void HighLightWeekends()
        {
            foreach (var child in BodyGrid.Children)
            {
                if (IsDayElement(child))
                {
                    MonthDayElement dayElementChild = child as MonthDayElement;
                    int childColumnIndex = (int)dayElementChild.GetValue(Grid.ColumnProperty);
                    if (IsInWeekendColumn(childColumnIndex))
                    {
                        dayElementChild.Foreground = highlightColor;
                    }
                }
                else if(IsWeekDayNameElement(child))
                {
                    TextBlock textBlockDayNameElement = child as TextBlock;
                    int childrenColumnIndex = (int)textBlockDayNameElement.GetValue(Grid.ColumnProperty);
                    if (IsInWeekendColumn(childrenColumnIndex))
                    {
                        textBlockDayNameElement.Foreground = highlightColor;
                    }
                }
            }
        }
        private void RefreshDayElements()
        {
            foreach (MonthDayElement dayElement in dayElements)
            {
                dayElement.AssignDayAppointments(GetDayAppointments(dayElement));
                dayElement.Refresh();
            }
        }
        private List<Appointment> GetDayAppointments(MonthDayElement dayElement)
        {
            List<Appointment> dayElementAppointments = new List<Appointment>();
            foreach (Appointment appointment in monthAppointmens)
            {
                bool hasReadPermission = appointment.IsOwnerOrGuest(SessionController.CurrenUser);
                if (IsAppointmentOfDay(appointment, dayElement) & hasReadPermission)
                {
                    dayElementAppointments.Add(appointment);
                }
            }
            return dayElementAppointments;
        }
        private static bool IsInWeekendColumn(int childrenColumnIndex)
        {
            if (childrenColumnIndex == saturdayGridColumnIndex || childrenColumnIndex == sundayGridColumnIndex)
            {
                return true;
            }
            return false;
        }
        private static bool IsDayElement(object children)
        {
            if (children.GetType() == typeof(MonthDayElement))
            {
                return true;
            }
            return false;
        }
        private static bool IsWeekDayNameElement(object children)
        {
            if (children.GetType() == typeof(TextBlock))
            {
                return true;
            }
            return false;
        }
        private static bool IsDayNumberInDisplayedMonth(int candidateDayNumber, Point dayElementGridCoordinates)
        {
            const int firstDayRowIndex = 1;
            DateTime displayedDate = Utilities.DisplayedDate;
            bool isFirstDayRow = dayElementGridCoordinates.Y == firstDayRowIndex;
            bool isNotFirstDayRow = dayElementGridCoordinates.Y > firstDayRowIndex;
            bool isFirstDayColumnOrLater = dayElementGridCoordinates.X >= GetfirstDayGridColumnIndex();
            bool isDisplayableDayElementOfFirstRow = isFirstDayRow && isFirstDayColumnOrLater;
            bool isCandidateDayNumberInDisplayedMonth = candidateDayNumber <= GetNumberOfDaysOfMonth(displayedDate);
            bool isDisplayableDayElementOfRemainsRows = isNotFirstDayRow && isCandidateDayNumberInDisplayedMonth;
            return (isDisplayableDayElementOfFirstRow || isDisplayableDayElementOfRemainsRows);
        }
        private static bool IsAppointmentOfDay(Appointment appointment, MonthDayElement dayElement) 
        {
            if (appointment.Start.Date == dayElement.Date.Date)
            {
                return true;
            }
            return false;
        }
        private static int GetNumberOfDaysOfMonth(DateTime date)
        {
            DateTime displayedDate = date;
            int numberOfDaysOfDisplayedMonth = DateTime.DaysInMonth(displayedDate.Year, displayedDate.Month);
            return numberOfDaysOfDisplayedMonth;
        }
        private static int GetfirstDayGridColumnIndex()
        {
            DateTime displayedDate = Utilities.DisplayedDate;
            DateTime firstDayOfDisplayedMonth = new DateTime(displayedDate.Year, displayedDate.Month, firstDayNumberInMonth);
            int firstDayGridColumnIndex = Utilities.GetDayNumberInWeek(firstDayOfDisplayedMonth) - gridColumnIndexOffset;
            return firstDayGridColumnIndex;
        }
        private static Point GetGridCoordinatesByIterationIndex(int iterationIndex)
        {
            int gridColumn = (iterationIndex) % Utilities.DaysInWeek;
            int gridRow = (iterationIndex / Utilities.DaysInWeek) + gridRowIndexOffset;
            return new Point(gridColumn, gridRow); ;
        }
        #endregion
    }
}
