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
            set 
            {
                monthAppointmens = value;
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
                    int year = Utilities.GetDisplayedDate().Year;
                    int month = Utilities.GetDisplayedDate().Month;
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
                List<string> dayNames = Utilities.GetDayNames();
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
        private void InsertDayElementsToGrid()
        {
            foreach (MonthDayElement dayElement in dayElements)
            {
                BodyGrid.Children.Add(dayElement);
            }
        }
        private void HighLightWeekends()
        {
            foreach (var children in BodyGrid.Children)
            {
                if (IsDayElement(children))
                {
                    int childrenColumnIndex = (int)(children as MonthDayElement).GetValue(Grid.ColumnProperty);
                    if (IsInWeekendColumn(childrenColumnIndex))
                    {
                        (children as MonthDayElement).Foreground = highlightColor;
                    }
                }
                else if(IsWeekDayNameElement(children))
                {
                    int childrenColumnIndex = (int)(children as TextBlock).GetValue(Grid.ColumnProperty);
                    if (IsInWeekendColumn(childrenColumnIndex))
                    {
                        (children as TextBlock).Foreground = highlightColor;
                    }
                }
            }
        }
        private void RefreshDayElements()
        {
            foreach (MonthDayElement dayElement in dayElements)
            {
                dayElement.DayAppointments = GetDayAppointments(dayElement);
                dayElement.Refresh();
            }
        }
        private List<Appointment> GetDayAppointments(MonthDayElement dayElement) 
        {
            List<Appointment> dayElementAppointments = new List<Appointment>();
            foreach (Appointment appointment in monthAppointmens)
            {
                bool hasReadPermission = appointment.IsOwnerOrGuest(SessionController.GetCurrenUser());
                if (IsAppointmentOfDay(appointment, dayElement) & hasReadPermission)
                {
                    dayElementAppointments.Add(appointment);
                }
            }
            return dayElementAppointments;
        }
        private bool IsInWeekendColumn(int childrenColumnIndex)
        {
            if (childrenColumnIndex == saturdayGridColumnIndex || childrenColumnIndex == sundayGridColumnIndex)
            {
                return true;
            }
            return false;
        }
        private bool IsDayElement(object children)
        {
            if (children.GetType() == typeof(MonthDayElement))
            {
                return true;
            }
            return false;
        }
        private bool IsWeekDayNameElement(object children)
        {
            if (children.GetType() == typeof(TextBlock))
            {
                return true;
            }
            return false;
        }
        private bool IsDayNumberInDisplayedMonth(int candidateDayNumber, Point dayElementGridCoordinates)
        {
            const int firstDayRowIndex = 1;
            DateTime displayedDate = Utilities.GetDisplayedDate();
            bool isFirstDayRow = dayElementGridCoordinates.Y == firstDayRowIndex;
            bool isNotFirstDayRow = dayElementGridCoordinates.Y > firstDayRowIndex;
            bool isFirstDayColumnOrLater = dayElementGridCoordinates.X >= GetfirstDayGridColumnIndex();
            bool isDisplayableDayElementOfFirstRow = isFirstDayRow && isFirstDayColumnOrLater;
            bool isCandidateDayNumberInDisplayedMonth = candidateDayNumber <= GetNumberOfDaysOfMonth(displayedDate);
            bool isDisplayableDayElementOfRemainsRows = isNotFirstDayRow && isCandidateDayNumberInDisplayedMonth;
            return (isDisplayableDayElementOfFirstRow || isDisplayableDayElementOfRemainsRows);
        }
        private bool IsAppointmentOfDay(Appointment appointment, MonthDayElement dayElement) 
        {
            if (appointment.Start.Date == dayElement.Date.Date)
            {
                return true;
            }
            return false;
        }
        private int GetNumberOfDaysOfMonth(DateTime date)
        {
            DateTime displayedDate = date;
            int numberOfDaysOfDisplayedMonth = DateTime.DaysInMonth(displayedDate.Year, displayedDate.Month);
            return numberOfDaysOfDisplayedMonth;
        }
        private int GetfirstDayGridColumnIndex()
        {
            DateTime displayedDate = Utilities.GetDisplayedDate();
            DateTime firstDayOfDisplayedMonth = new DateTime(displayedDate.Year, displayedDate.Month, firstDayNumberInMonth);
            int firstDayGridColumnIndex = Utilities.GetDayNumberInWeek(firstDayOfDisplayedMonth) - gridColumnIndexOffset;
            return firstDayGridColumnIndex;
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
