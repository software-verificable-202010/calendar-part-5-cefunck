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
    /// Lógica de interacción para WeekBody.xaml
    /// </summary>
    public partial class WeekBody : UserControl
    {
        #region Constants
        private const int saturdayGridColumnIndex = 6;
        private const int sundayGridColumnIndex = 7;
        #endregion

        #region Fields
        private readonly Brush highlightColor = Brushes.Red;
        private List<WeekColumn> dayColumns = new List<WeekColumn>();
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
        public WeekBody()
        {
            InitializeComponent();

        }
        public void Refresh() 
        {
            GenerateDayColumnElements();
            InsertBodyElements();
            RefreshDayColumnElements();
            HighLightWeekends();
        }
        private void InsertBodyElements()
        {
            WeekBodyGrid.Children.Clear();
            InsertHoursColumnToGrid();
            InsertDayColumnElementsToGrid();
        }
        private void InsertDayColumnElementsToGrid()
        {
            foreach (WeekColumn dayColumnElement in dayColumns)
            {
                WeekBodyGrid.Children.Add(dayColumnElement);
            }
        }
        private void InsertHoursColumnToGrid()
        {
            int hoursColumnIndex = 0;
            WeekHoursColumn weekHoursColumn = new WeekHoursColumn();
            weekHoursColumn.SetValue(Grid.ColumnProperty, hoursColumnIndex);
            WeekBodyGrid.Children.Add(weekHoursColumn);
        }
        private void HighLightWeekends()
        {
            foreach (var children in WeekBodyGrid.Children)
            {
                if (IsDayColumnElement(children))
                {
                    int childrenColumnIndex = (int)(children as WeekColumn).GetValue(Grid.ColumnProperty);
                    if (IsInWeekendColumn(childrenColumnIndex))
                    {
                        (children as WeekColumn).Foreground = highlightColor;
                    }
                }
            }
        }
        private bool IsDayColumnElement(object children)
        {
            if (children.GetType() == typeof(WeekColumn))
            {
                return true;
            }
            return false;
        }
        private bool IsInWeekendColumn(int childrenColumnIndex)
        {
            if (childrenColumnIndex == saturdayGridColumnIndex || childrenColumnIndex == sundayGridColumnIndex)
            {
                return true;
            }
            return false;
        }
        private void RefreshDayColumnElements()
        {
            foreach (WeekColumn dayColumnElement in dayColumns)
            {
                dayColumnElement.DayAppointments = GetDayAppointments(dayColumnElement);
                dayColumnElement.Refresh();
            }
        }
        private List<Appointment> GetDayAppointments(WeekColumn dayElement)
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
        private bool IsAppointmentOfDay(Appointment appointment, WeekColumn dayColumnElement)
        {
            if (appointment.Start.Date == dayColumnElement.Date.Date)
            {
                return true;
            }
            return false;
        }
        public void GenerateDayColumnElements()
        {
            dayColumns = new List<WeekColumn>();
            for (int dayNumberInWeek = 1; dayNumberInWeek <= Utilities.daysInWeek; dayNumberInWeek++)
            {
                DateTime displayedDate = Utilities.GetDisplayedDate();
                int displayedDateDayOfWeek = Utilities.GetDayNumberInWeek(displayedDate);
                int daysFromDisplayedDate = Utilities.negativeMultiplier * displayedDateDayOfWeek + dayNumberInWeek;
                DateTime dayColumnDate = displayedDate.AddDays(daysFromDisplayedDate);
                WeekColumn weekColumnElement = new WeekColumn(dayColumnDate, dayNumberInWeek);
                weekColumnElement.SetValue(Grid.ColumnProperty, dayNumberInWeek);
                dayColumns.Add(weekColumnElement);
            }
        }
        #endregion
    }
}
