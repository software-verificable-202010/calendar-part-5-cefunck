using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Calendar
{
    /// <summary>
    /// Lógica de interacción para MonthDayElement.xaml
    /// </summary>
    public partial class MonthDayElement : UserControl
    {
        #region Constants
        #endregion


        #region Fields
        private readonly DateTime date;
        private readonly Brush appointmentButtonBackground = Brushes.CornflowerBlue;
        private readonly Brush appointmentButtonForeground = Brushes.White;
        private List<Appointment> dayAppointments = new List<Appointment>();
        private Appointment selectedAppointment;
        #endregion


        #region Properties
        public DateTime Date 
        {
            get 
            {
                return date;
            }
        }
        #endregion


        #region Methods
        public MonthDayElement()
        {
            InitializeComponent();
        }

        public MonthDayElement(DateTime date)
        {
            this.date = date;
            InitializeComponent();
        }

        public void Refresh() 
        {
            if (IsNotBlankDayElement())
            {
                RefreshDayNumber();
                RefreshAppointments();
                AddButtonForNewAppointment();
            }
        }

        public void AssignDayAppointments(List<Appointment> appointments)
        {
            dayAppointments = appointments;
        }

        private void RefreshDayNumber() 
        {
            int displayedDateMonth = Utilities.DisplayedDate.Month;
            int dayElementMonth = date.Month;
            bool dayElementIsOfDisplayedMonth = dayElementMonth == displayedDateMonth;

            if (dayElementIsOfDisplayedMonth)
            {
                textBlockDayNumber.Text = date.Day.ToString(CultureInfo.CurrentCulture);
            }
        }

        private void RefreshAppointments()
        {
            const string bindingPropertyName = "Title";
            stackPanelMonthDayElement.Children.Clear();
            foreach (Appointment appointment in dayAppointments)
            {
                Binding appointmentBinding = new Binding(bindingPropertyName)
                {
                    Source = appointment
                };
                Button buttonDayElementAppoinment = new Button() 
                { 
                    Background = appointmentButtonBackground,
                    Foreground = appointmentButtonForeground
                };
                buttonDayElementAppoinment.SetBinding(ContentProperty, appointmentBinding);
                buttonDayElementAppoinment.Click += EditAppointmentButton_Click;
                stackPanelMonthDayElement.Children.Add(buttonDayElementAppoinment);
            }
        }

        private void AddButtonForNewAppointment() 
        {
            const string empty = "";
            Button buttonNewDayElementAppoinment = new Button
            {
                Content = empty
            };
            buttonNewDayElementAppoinment.Click += NewAppointmentButton_Click;
            stackPanelMonthDayElement.Children.Add(buttonNewDayElementAppoinment);
        }

        private void EditAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshSelectedAppointment(sender);
            ShowAppointmentForm();
            SaveAppointmentChanges();
            Refresh();
        }

        private void NewAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshSelectedAppointmentAsNew();
            ShowAppointmentForm();
            SaveNewAppointment();
            Refresh();
        }

        private void RefreshSelectedAppointment(object sender)
        {
            int appointmentIndexInDayAppointents = stackPanelMonthDayElement.Children.IndexOf(sender as UIElement);
            selectedAppointment = dayAppointments[appointmentIndexInDayAppointents];
        }

        private void RefreshSelectedAppointmentAsNew()
        {
            int appointmentYear = this.date.Year;
            int appointmentMonth = this.date.Month;
            int appointmentDay = this.date.Day;
            TimeSpan nowTime = DateTime.Now.TimeOfDay;
            User currentUser = SessionController.CurrenUser;
            DateTime appointmentStartDate = new DateTime(appointmentYear, appointmentMonth, appointmentDay) + nowTime;
            selectedAppointment = new Appointment(appointmentStartDate, currentUser);
        }

        private void ShowAppointmentForm()
        {
            AppointmentWindow newAppointmentWindow = new AppointmentWindow(selectedAppointment);
            newAppointmentWindow.ShowDialog();
        }

        private void SaveAppointmentChanges()
        {
            if (IsValidAppointment())
            {
                UpdateCalendarAppointments();
                UpdateDayAppointments();
            }
        }

        private void UpdateDayAppointments() 
        {
            if (dayAppointments.Contains(selectedAppointment))
            {
                if (selectedAppointment.IsInGarbage)
                {
                    dayAppointments.Remove(selectedAppointment);
                }
                else 
                {
                    int appointmentIndexInDayAppointments = dayAppointments.IndexOf(selectedAppointment);
                    dayAppointments[appointmentIndexInDayAppointments] = selectedAppointment;
                }
            }
            else
            {
                dayAppointments.Add(selectedAppointment);
            }
        }

        private void UpdateCalendarAppointments() 
        {
            List<Appointment> calendarAppointments = Utilities.CalendarAppointments;
            if (calendarAppointments.Contains(selectedAppointment))
            {
                if (selectedAppointment.IsInGarbage)
                {
                    calendarAppointments.Remove(selectedAppointment);
                }
                else 
                {
                    int appointmentIndexInCalendarAppointments = calendarAppointments.IndexOf(selectedAppointment);
                    calendarAppointments[appointmentIndexInCalendarAppointments] = selectedAppointment;
                }
            }
            else 
            {
                calendarAppointments.Add(selectedAppointment);
            }
            Utilities.AssignCalendarAppointments(calendarAppointments);
            Utilities.SavePersistentAppointments();
        }

        private void SaveNewAppointment() 
        {
            if (IsValidAppointment())
            {
                UpdateDayAppointments();
                UpdateCalendarAppointments();
            }
        }

        private bool IsNotBlankDayElement()
        {
            bool isUnassignedDate = date.Year != 1;
            return isUnassignedDate;
        }

        private bool IsValidAppointment() 
        {
            bool isNotBlankTitle = selectedAppointment.Title.Trim().Length != 0;
            return isNotBlankTitle;
        }

        #endregion
    }
}
