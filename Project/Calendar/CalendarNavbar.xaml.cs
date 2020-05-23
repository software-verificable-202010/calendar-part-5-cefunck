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
        private const string weekViewOption = "Vista Semanal";
        private const string currentBodyContentResourceName = "bodyContent";
        private const string navBarMonthFormat = "MMMM yyyy";
        private const string monthAndYearResourceName = "monthAndYear";
        private const int numberToAdvance = 1;
        private const int numberToGoBack = -1;
        #endregion

        #region Fields
        private List<Appointment> calendarAppointments;
        private string selectedCalendarViewOption;
        private MonthBody monthBody;
        private WeekBody weekBody;
        #endregion

        #region Properties
        #endregion

        #region Methods
        public CalendarNavbar()
        {
            Utilities.SetDisplayedDateToNow();
            RefreshCalendar();
            InitializeComponent();
        }
        private void CurrentCalendarViewOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCalendarViewOption = GetSelectedCalendarView();
            RefreshCalendarBody();
        }
        private void BackwardNavigation_Click(object sender, RoutedEventArgs e)
        {
            MoveCalendar(numberToGoBack);
        }
        private void ForwardNavigation_Click(object sender, RoutedEventArgs e)
        {
            MoveCalendar(numberToAdvance);
        }
        private void MoveCalendar(int amountToMove)
        {
            DateTime newDisplayedDate = Utilities.GetDisplayedDate();
            string currentSelectedCalendarViewOption = GetSelectedCalendarView();
            switch (currentSelectedCalendarViewOption)
            {
                case weekViewOption:
                    newDisplayedDate = newDisplayedDate.AddDays(amountToMove * Utilities.daysInWeek);
                    break;
                default:
                    newDisplayedDate = newDisplayedDate.AddMonths(amountToMove);
                    break;
            }
            Utilities.SetDisplayedDate(newDisplayedDate);
            RefreshCalendar();
        }
        private void RefreshCalendar()
        {
            RefreshCalendarAppointments();
            RefreshDisplayedNavbarDate();
            RefreshCalendarBody();
        }        
        private void RefreshDisplayedNavbarDate()
        {
            DateTime displayedDate = Utilities.GetDisplayedDate();
            App.Current.Resources[monthAndYearResourceName] = displayedDate.ToString(navBarMonthFormat);
        }
        private void RefreshCalendarBody() 
        {
            switch (selectedCalendarViewOption)
            {
                case weekViewOption:
                    RefreshWeekBody();
                    break;
                default:
                    RefreshMonthBody();
                    break;
            }
        }
        private void RefreshMonthBody()
        {
            monthBody = new MonthBody
            {
                MonthAppointments = GetMonthAppointments()
            };
            monthBody.Refresh();
            App.Current.Resources[currentBodyContentResourceName] = monthBody;
        }
        private void RefreshWeekBody() 
        {
            weekBody = new WeekBody();
            weekBody.Refresh();
            App.Current.Resources[currentBodyContentResourceName] = weekBody;
        }
        private void RefreshCalendarAppointments() 
        {
            Utilities.LoadPersistentAppointments();
            calendarAppointments = Utilities.GetCalendarAppointments();
        }
        private List<Appointment> GetMonthAppointments()
        {
            List<Appointment> monthAppointmens = new List<Appointment>();
            foreach (Appointment appointment in calendarAppointments)
            {
                if (IsAppointmentOfDisplayedMonth(appointment))
                {
                    monthAppointmens.Add(appointment);
                }
            }
            return monthAppointmens;
        }
        private bool IsAppointmentOfDisplayedMonth(Appointment appointment)
        {
            int displayedMonth = Utilities.GetDisplayedDate().Month;
            int appointmentMonth = appointment.Start.Month;
            if (appointmentMonth == displayedMonth)
            {
                return true;
            }
            return false;
        }
        private string GetSelectedCalendarView()
        {
            const int initOfValueSubstring = 38;
            return CurrentCalendarViewOptions.SelectedValue.ToString().Substring(initOfValueSubstring);
        }
        #endregion
    }
}
