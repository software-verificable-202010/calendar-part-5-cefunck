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
using System.Windows.Navigation;
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
        private const string invalidGuestNameExistMessage = "Los siguientes invitados no son válidos:";
        private const string existAppointmentCollisionMessage = "Los siguientes invitados tienen un evento que colisiona:";
        private const string guestNamesFormPlaceHolder = "Ej: username1, username2";
        #endregion

        #region Fields
        private Appointment appointment;
        private readonly List<string> validationMessages = new List<string>();
        private List<User> candidateGuests = new List<User>();
        private bool canSaveAppointment = false;
        private string candidateTitle;
        private string candidateDescription;
        private DateTime candidateStart;
        private DateTime candidateEnd;
        private bool hasOwnerPermissions;
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
            RefreshForm();
        }
        private void RefreshDeleteButton() 
        {
            if (IsUpdatingApponinment())
            {
                RefreshSaveButton();
                InsertDeleteButton();
            }
        }
        private void RefreshSaveButton() 
        {
            const int newSaveButtonColumnSpan = 3;
            buttonSave.SetValue(Grid.ColumnSpanProperty, newSaveButtonColumnSpan);
            buttonSave.IsEnabled = hasOwnerPermissions;
        }
        private void InsertDeleteButton() 
        {
            const int deleteButtonColumnSpan = 3;
            const int deleteButtonColumn = 3;
            const int deleteButtonRow = 4;
            const string deleteButtonContent = "Eliminar";
            Button buttonDelete = new Button
            {
                Content = deleteButtonContent,
                IsEnabled = hasOwnerPermissions
            };
            buttonDelete.SetValue(Grid.ColumnSpanProperty, deleteButtonColumnSpan);
            buttonDelete.SetValue(Grid.ColumnProperty, deleteButtonColumn);
            buttonDelete.SetValue(Grid.RowProperty, deleteButtonRow);
            buttonDelete.Click += DeleteButton_Click;
            grid.Children.Add(buttonDelete);
        }
        private bool IsUpdatingApponinment() 
        {
            if (appointment.Title.Trim().Length != 0)
            {
                return true;
            }
            return false;
        }
        private void RefreshForm() 
        {
            RefreshPermissions();
            RefreshFields();
            RefreshDeleteButton();
        }
        private void RefreshPermissions()
        {
            hasOwnerPermissions = appointment.Owner.Name == SessionController.GetCurrenUser().Name;
        }
        private void RefreshFields() 
        {
            string title = appointment.Title;
            string description = appointment.Description;
            int startHour = appointment.Start.Hour;
            int startMinute = appointment.Start.Minute;
            int endHour = appointment.End.Hour;
            int endMinute = appointment.End.Minute;
            textBoxTitle.Text = title;
            textBoxDescription.Text = description;
            textBoxGuests.Text = GetAppointmentGuestNames();
            comboBoxStartHour.SelectedIndex = startHour;
            comboBoxStartMinute.SelectedIndex = startMinute;
            comboBoxEndHour.SelectedIndex = endHour;
            comboBoxEndMinute.SelectedIndex = endMinute;
            AddGuestNamesPlaceHolderLogic();
        }
        private void AddGuestNamesPlaceHolderLogic()
        {
            if (textBoxGuests.Text == guestNamesFormPlaceHolder)
            {
                textBoxGuests.SelectionChanged += TextBoxGuests_SelectionChanged;
            }
        }
        private void TextBoxGuests_SelectionChanged(object sender, RoutedEventArgs e)
        {
            textBoxGuests.SelectionChanged -= TextBoxGuests_SelectionChanged;
            const string empty = "";
            textBoxGuests.Text = empty;
        }
        private string GetAppointmentGuestNames()
        {
            const string prefix = ", ";
            string guestNames = guestNamesFormPlaceHolder;
            for (int i = 0; i < appointment.Guests.Count; i++)
            {
                string guestName = appointment.Guests[i].Name;
                if (i == 0)
                {
                    guestNames = guestName;
                }
                else
                {
                    guestNames += prefix + guestName;
                }
            }
            return guestNames;
        }
        private void InsertTimeOptions() 
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
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            appointment.IsInGarbage = true;
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
            this.candidateGuests.RemoveAll(item => item == null);
            this.candidateGuests.RemoveAll(item => item.HasAppointmentCollision(appointment));
            appointment.Guests = this.candidateGuests;
        }
        private void ShowValidations() 
        {
            string validationMessage = "";
            foreach (string message in validationMessages)
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
            this.candidateGuests = GetCandidateGuests();
        }
        private List<User> GetCandidateGuests()
        {
            List<User> result = new List<User>();
            List<string> candidateGuestNames = GetCandidateGuestNames();
            foreach (string name in candidateGuestNames)
            {
                User candidateGuest = SessionController.GetUserByName(name);
                result.Add(candidateGuest);
            }
            return result;
        }
        private void ResetValidations() 
        {
            validationMessages.Clear();
            this.canSaveAppointment = IsNotBlankTitle() & IsValidEndTime() & AreValidGuests() & !ExistingAppointmentCollision();
        }
        private bool AreValidGuests()
        {
            bool isPlaceHolderInGuestNamesField = textBoxGuests.Text == guestNamesFormPlaceHolder;
            bool isBlankGuestNamesField = textBoxGuests.Text.Trim().Length == 0;
            bool existsNullGuest = this.candidateGuests.Contains(null);
            if (existsNullGuest & !(isBlankGuestNamesField | isPlaceHolderInGuestNamesField))
            {
                return false;
            }
            return true;
        }
        private bool ExistingAppointmentCollision() 
        {
            List<User> notNullCandidateGuests = this.candidateGuests.Where(i => i != null).ToList();
            return notNullCandidateGuests.Any(i => i.HasAppointmentCollision(this.appointment));
        }
        private void RefreshValidationMessages()
        {
            if (!IsNotBlankTitle())
            {
                validationMessages.Add(blankTitleMessage);
            }
            if (!IsValidEndTime())
            {
                validationMessages.Add(invalidEndTimeMessage);
            }
            if (!AreValidGuests())
            {
                validationMessages.Add(invalidGuestNameExistMessage);
                AddInvalidGuestNamesToValidationMessages();
            }
            if (ExistingAppointmentCollision())
            {
                validationMessages.Add(existAppointmentCollisionMessage);
                AddCollisionedGuestNamesToValidationMessages();
            }
        }
        private void AddCollisionedGuestNamesToValidationMessages()
        {
            const string prefix = "- ";
            foreach (string name in this.GetCandidateGuestNames())
            {
                User Guest = SessionController.GetUserByName(name);
                if (Guest != null && Guest.HasAppointmentCollision(appointment))
                {
                    validationMessages.Add(prefix + name);
                }
            }
        }
        private void AddInvalidGuestNamesToValidationMessages()
        {
            const string prefix = "- ";
            foreach (string name in this.GetCandidateGuestNames())
            {
                if (SessionController.GetUserByName(name) == null)
                {
                    validationMessages.Add(prefix + name);
                }
            }
        }
        private List<string> GetCandidateGuestNames()
        {
            List<string> candidateGuestNames = textBoxGuests.Text.Split(',').ToList();
            for (int i = 0; i < candidateGuestNames.Count; i++)
            {
                candidateGuestNames[i] = candidateGuestNames[i].Trim();
            }
            return candidateGuestNames;
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
