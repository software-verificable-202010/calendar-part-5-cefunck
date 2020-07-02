using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Calendar.Controllers;
using Calendar.Interfaces;
using Calendar.Models;


namespace Calendar.Windows.Partials
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
        private List<IAppointment> calendarAppointments;
        private string selectedCalendarViewOption;
        private SessionController sourceSessionController;
        private MonthBody monthBody;
        private WeekBody weekBody;
        #endregion


        #region Properties
        #endregion


        #region Methods
        public CalendarNavbar(SessionController sourceSessionController)
        {
            DateTime now = DateTime.Now;
            DateUtilities.SetDisplayedDateTo(now);
            this.sourceSessionController = sourceSessionController;
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
            DateTime newDisplayedDate = DateUtilities.DisplayedDate;
            string currentSelectedCalendarViewOption = GetSelectedCalendarView();
            switch (currentSelectedCalendarViewOption)
            {
                case weekViewOption:
                    newDisplayedDate = newDisplayedDate.AddDays(amountToMove * DateUtilities.DaysInWeek);
                    break;
                default:
                    newDisplayedDate = newDisplayedDate.AddMonths(amountToMove);
                    break;
            }
            DateUtilities.DisplayedDate = newDisplayedDate;
            RefreshCalendar();
        }

        private void RefreshCalendar()
        {
            RefreshCalendarAppointments();
            RefreshDisplayedNavbarDate();
            RefreshCalendarBody();
        }        

        private static void RefreshDisplayedNavbarDate()
        {
            DateTime displayedDate = DateUtilities.DisplayedDate;
            App.Current.Resources[monthAndYearResourceName] = displayedDate.ToString(navBarMonthFormat, CultureInfo.CurrentCulture);
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
            monthBody = new MonthBody(sourceSessionController);
            monthBody.AssignMonthAppointments(GetMonthAppointmentsFromCalendarAppointments());
            monthBody.Refresh();
            App.Current.Resources[currentBodyContentResourceName] = monthBody;
        }

        private void RefreshWeekBody() 
        {
            weekBody = new WeekBody(sourceSessionController);
            weekBody.AssignMonthAppointments(GetMonthAppointmentsFromCalendarAppointments());
            weekBody.Refresh();
            App.Current.Resources[currentBodyContentResourceName] = weekBody;
        }

        private void RefreshCalendarAppointments() 
        {
            AppointmentController.LoadPersistentAppointments();
            calendarAppointments = AppointmentController.CalendarAppointments;
        }

        private List<Appointment> GetMonthAppointmentsFromCalendarAppointments()
        {
            List<Appointment> monthAppointmens = new List<Appointment>();
            foreach (Appointment appointment in calendarAppointments)
            {
                bool isOfDisplayedMonth = IsAppointmentOfDisplayedMonth(appointment);
                if (isOfDisplayedMonth)
                {
                    monthAppointmens.Add(appointment);
                }
            }
            return monthAppointmens;
        }

        private static bool IsAppointmentOfDisplayedMonth(Appointment appointment)
        {
            int displayedMonth = DateUtilities.DisplayedDate.Month;
            int appointmentMonth = appointment.StartTime.Month;
            bool areTheSameMonth = appointmentMonth == displayedMonth;
            return areTheSameMonth;
        }

        private string GetSelectedCalendarView()
        {
            const int initOfValueSubstring = 38;
            return CurrentCalendarViewOptions.SelectedValue.ToString().Substring(initOfValueSubstring);
        }

        #endregion
    }
}
