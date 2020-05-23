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
        private List<Appointment> dayAppointments = new List<Appointment>();
        private DateTime date;
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
                Button buttonDayElementAppoinment = new Button();
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
            int appointmentIndex = stackPanelMonthDayElement.Children.IndexOf(sender as UIElement);
            Appointment selectedAppointment = dayAppointments[appointmentIndex];
            AppointmentWindow newAppointmentWindow = new AppointmentWindow(selectedAppointment);
            newAppointmentWindow.ShowDialog();
            SaveValidUpdatedAppointment(selectedAppointment, appointmentIndex);
            Refresh();
        }
        private void NewAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            Appointment appointment = new Appointment("","",DateTime.Now, DateTime.Now);
            AppointmentWindow newAppointmentWindow = new AppointmentWindow(appointment);
            newAppointmentWindow.ShowDialog();
            SaveValidCreatedAppointment(appointment);
            Refresh();
        }
        private void SaveValidUpdatedAppointment(Appointment appointment, int appointmentIndex)
        {
            if (IsValidAppointment(appointment))
            {
                dayAppointments[appointmentIndex] = appointment;
            }
        }
        private void SaveValidCreatedAppointment(Appointment appointment) 
        {
            if (IsValidAppointment(appointment))
            {
                dayAppointments.Add(appointment);
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
        private bool IsValidAppointment(Appointment appointment) 
        {
            if (appointment.Title.Trim().Length != 0)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
