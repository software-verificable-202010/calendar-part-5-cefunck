using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Calendar
{
    /// <summary>
    /// Lógica de interacción para AppointmentWindow.xaml
    /// </summary>
    public partial class AppointmentWindow : Window
    {
        #region Constants
        private const string blankTitleMessage = "Debe ingresar un título";
        private const string invalidEndTimeMessage = "Debe ingresar hora de fin válida";
        #endregion

        #region Fields
        private Appointment appointment;
        private List<string> ValidationMessages = new List<string>();
        private bool canSaveAppointment = false;
        private string candidateTitle;
        private string candidateDescription;
        private DateTime candidateStart;
        private DateTime candidateEnd;
        #endregion

        #region Properties
        public Appointment Appointment 
        {
            get 
            {
                return appointment;
            }
            set 
            {
                appointment = value;
            }
        }
        #endregion

        #region Methods
        public AppointmentWindow(Appointment appointment)
        {
            this.appointment = appointment;
            InitializeComponent();
            InsertTimeOptions();
            SelectDefaultTimeOptions();
            RefreshFormFields();
        }
        public void RefreshFormFields() 
        {
            string title = appointment.Title;
            string description = appointment.Description;
            int startHour = appointment.Start.Hour;
            int startMinute = appointment.Start.Minute;
            int endHour = appointment.End.Hour;
            int endMinute = appointment.End.Minute;
            textBoxTitle.Text = title;
            textBoxDescription.Text = description;
            comboBoxStartHour.SelectedIndex = startHour;
            comboBoxStartMinute.SelectedIndex = startMinute;
            comboBoxEndHour.SelectedIndex = endHour;
            comboBoxEndMinute.SelectedIndex = endMinute;
        }
        public void InsertTimeOptions() 
        {
            InsertHoursOptions();
            InsertMinutesOptions();
        }
        private void InsertHoursOptions()
        {
            for (int i = 0; i < 24; i++)
            {
                ComboBoxItem comboBoxItemStartHour = new ComboBoxItem
                {
                    Content = i
                };
                ComboBoxItem comboBoxItemEndtHour = new ComboBoxItem
                {
                    Content = i
                };
                comboBoxStartHour.Items.Add(comboBoxItemStartHour);
                comboBoxEndHour.Items.Add(comboBoxItemEndtHour);
            }
        }
        private void InsertMinutesOptions()
        {
            for (int i = 0; i < 60; i++)
            {
                ComboBoxItem comboBoxItemStartMinute = new ComboBoxItem
                {
                    Content = i
                };
                ComboBoxItem comboBoxItemEndMinute = new ComboBoxItem
                {
                    Content = i
                };
                comboBoxStartMinute.Items.Add(comboBoxItemStartMinute);
                comboBoxEndMinute.Items.Add(comboBoxItemEndMinute);
            }
        }
        private void SelectDefaultTimeOptions()
        {
            comboBoxStartHour.SelectedIndex = 0;
            comboBoxStartMinute.SelectedIndex = 0;
            comboBoxEndHour.SelectedIndex = 0;
            comboBoxEndMinute.SelectedIndex = 0;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e) 
        {
            RunValidations();
            if (canSaveAppointment)
            {
                SaveAppointmentData();
                this.Close();
            }
            else 
            {
                ShowValidations();
            }
        }
        private void RunValidations()
        {
            RefreshCandidateData();
            ResetValidations();
            RefreshValidationMessages();
        }
        private void SaveAppointmentData()
        {
            appointment.Title = this.candidateTitle;
            appointment.Description = this.candidateDescription;
            appointment.Start = this.candidateStart;
            appointment.End = this.candidateEnd;
        }
        private void ShowValidations() 
        {
            string validationMessage = "";
            foreach (string message in ValidationMessages)
            {
                validationMessage += message + "\n";
            }
            MessageBox.Show(validationMessage);
        }
        private void RefreshCandidateData()
        {
            this.candidateTitle = textBoxTitle.Text;
            this.candidateDescription = textBoxDescription.Text;
            this.candidateStart = appointment.Start.Date + GetCandidateTime("start");
            this.candidateEnd = appointment.End.Date + GetCandidateTime("end");
        }
        private void ResetValidations() 
        {
            ValidationMessages.Clear();
            this.canSaveAppointment = IsNotBlankTitle() & IsValidEndTime();
        }
        private void RefreshValidationMessages()
        {
            if (!IsNotBlankTitle())
            {
                ValidationMessages.Add(blankTitleMessage);
            }
            if (!IsValidEndTime())
            {
                ValidationMessages.Add(invalidEndTimeMessage);
            }
        }
        private bool IsNotBlankTitle() 
        {
            string titleCandidate = textBoxTitle.Text.Trim();
            if (titleCandidate.Length != 0)
            {
                return true;
            }
            return false;
        }
        private bool IsValidEndTime() 
        {
            if (this.candidateEnd > this.candidateStart)
            {
                return true;
            }
            return false;
        }
        private TimeSpan GetCandidateTime(string requiredTime) 
        {
            const int defaultSeconds = 0;
            int startHour = comboBoxStartHour.SelectedIndex;
            int startMinute = comboBoxStartMinute.SelectedIndex;
            int endHour = comboBoxEndHour.SelectedIndex;
            int endMinute = comboBoxEndMinute.SelectedIndex;
            TimeSpan startTime = new TimeSpan(startHour, startMinute, defaultSeconds);
            TimeSpan endTime = new TimeSpan(endHour, endMinute, defaultSeconds);
            if (requiredTime == "start")
            {
                return startTime;
            }
            return endTime;
        }
        #endregion
    }
}
