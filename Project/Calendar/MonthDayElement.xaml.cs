using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
    /// Lógica de interacción para MonthDayElement.xaml
    /// </summary>
    public partial class MonthDayElement : UserControl
    {
        #region Constants
        #endregion

        #region Fields
        private Brush appointmentButtonBackground = Brushes.CornflowerBlue;
        private Brush appointmentButtonForeground = Brushes.White;
        private List<Appointment> dayAppointments = new List<Appointment>();
        private DateTime date;
        private Appointment selectedAppointment;
        #endregion

        #region Properties
        public List<Appointment> DayAppointments 
        {
            get 
            {
                return dayAppointments;
            }
            set 
            {
                dayAppointments = value;
            }
        }
        public DateTime Date 
        {
            get 
            {
                return date;
            }
            set 
            {
                date = value;
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
        private void RefreshDayNumber() 
        {
            int displayedDateMonth = Utilities.GetDisplayedDate().Month;
            int dayElementMonth = date.Month;
            if (dayElementMonth == displayedDateMonth)
            {
                textBlockDayNumber.Text = date.Day.ToString();
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
            Button buttonNewDayElementAppoinment = new Button
            {
                Content = Utilities.blankSpace
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
            string emptyTextField = Utilities.blankSpace;
            int appointmentYear = this.date.Year;
            int appointmentMonth = this.date.Month;
            int appointmentDay = this.date.Day;
            User currentUser = SessionController.GetCurrenUser();
            DateTime appointmentDate = new DateTime(appointmentYear, appointmentMonth, appointmentDay) + DateTime.Now.TimeOfDay;
            selectedAppointment = new Appointment(emptyTextField, emptyTextField, appointmentDate, appointmentDate, currentUser);
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
            List<Appointment> calendarAppointments = Utilities.GetCalendarAppointments();
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
            Utilities.SetCalendarAppointments(calendarAppointments);
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
            if (this.date.Year != 1)
            {
                return true;
            }
            return false;
        }
        private bool IsValidAppointment() 
        {
            if (selectedAppointment.Title.Trim().Length != 0)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
