using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private const int hoursInOneDay = 24;
        private const int minutesInOneHour = 60;
        private const string emptyTitleMessage = "Debe ingresar un título";
        private const string invalidEndTimeMessage = "Debe ingresar hora de fin válida";
        private const string invalidGuestsMessage = "Los siguientes invitados no son válidos:";
        private const string appointmentCollisionMessage = "Los siguientes invitados tienen un evento que colisiona:";
        private const string guestNamesFieldPlaceHolder = "Ej: un_nombre, otro_nombre";
        #endregion


        #region Fields
        private bool hasOwnerPermissions = false;
        private bool canSaveSourceAppointment = false;
        private string candidateTitle;
        private string candidateDescription;
        private DateTime candidateStart;
        private DateTime candidateEnd;
        private List<User> candidateGuests = new List<User>();
        private readonly Appointment sourceAppointment;
        private readonly List<string> validationMessages = new List<string>();

        #endregion


        #region Properties
        #endregion


        #region Methods
        public AppointmentWindow(Appointment sourceAppointment)
        {
            this.sourceAppointment = sourceAppointment;
            InitializeComponent();
            InsertTimeOptions();
            RefreshForm();
        }

        private void InsertTimeOptions()
        {
            InsertHoursOptions();
            InsertMinutesOptions();
        }

        private void InsertHoursOptions()
        {
            for (int i = 0; i < hoursInOneDay; i++)
            {
                ComboBoxItem comboBoxItemStartHour = new ComboBoxItem
                {
                    Content = i
                };
                comboBoxStartHour.Items.Add(comboBoxItemStartHour);

                ComboBoxItem comboBoxItemEndtHour = new ComboBoxItem
                {
                    Content = i
                };
                comboBoxEndHour.Items.Add(comboBoxItemEndtHour);
            }
        }

        private void InsertMinutesOptions()
        {
            for (int i = 0; i < minutesInOneHour; i++)
            {
                ComboBoxItem comboBoxItemStartMinute = new ComboBoxItem
                {
                    Content = i
                };
                comboBoxStartMinute.Items.Add(comboBoxItemStartMinute);

                ComboBoxItem comboBoxItemEndMinute = new ComboBoxItem
                {
                    Content = i
                };
                comboBoxEndMinute.Items.Add(comboBoxItemEndMinute);
            }
        }

        private void RefreshForm()
        {
            RefreshPermissions();
            RefreshFields();
            RefreshDeleteButton();
        }

        private void RefreshPermissions()
        {
            User currentUser = SessionController.CurrenUser;
            hasOwnerPermissions = sourceAppointment.IsOwner(currentUser);
        }

        private void RefreshFields()
        {
            string title = sourceAppointment.Title;
            string description = sourceAppointment.Description;
            string guestNames = GetAppointmentGuestNamesInFieldFormat();
            int startHour = sourceAppointment.Start.Hour;
            int startMinute = sourceAppointment.Start.Minute;
            int endHour = sourceAppointment.End.Hour;
            int endMinute = sourceAppointment.End.Minute;

            textBoxTitle.Text = title;
            textBoxDescription.Text = description;
            textBoxGuests.Text = guestNames;
            comboBoxStartHour.SelectedIndex = startHour;
            comboBoxStartMinute.SelectedIndex = startMinute;
            comboBoxEndHour.SelectedIndex = endHour;
            comboBoxEndMinute.SelectedIndex = endMinute;
            SuscribeGuestNamesFieldSelectionChanged();
        }

        private void SuscribeGuestNamesFieldSelectionChanged()
        {
            string guestNamesField = textBoxGuests.Text;
            bool isStillPlaceHodler = guestNamesField == guestNamesFieldPlaceHolder;
            if (isStillPlaceHodler)
            {
                textBoxGuests.SelectionChanged += GuestNamesField_SelectionChanged;
            }
        }

        private void GuestNamesField_SelectionChanged(object sender, RoutedEventArgs e)
        {
            textBoxGuests.SelectionChanged -= GuestNamesField_SelectionChanged;
            textBoxGuests.Text = string.Empty;
        }

        private void RefreshDeleteButton() 
        {
            if (IsFormForEditAppointment())
            {
                RefreshSaveButtonForEditForm();
                InsertDeleteButton();
            }
        }

        private void RefreshSaveButtonForEditForm() 
        {
            const int saveButtonColumnSpan = 3;
            buttonSave.SetValue(Grid.ColumnSpanProperty, saveButtonColumnSpan);
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            sourceAppointment.IsInGarbage = true;
            this.Close();
        }

        private bool IsFormForEditAppointment() 
        {
            string sourceAppointmentTitle = sourceAppointment.Title.Trim();
            bool isNotBlankTitle = sourceAppointmentTitle.Length != 0;
            return isNotBlankTitle;
        }

        private string GetAppointmentGuestNamesInFieldFormat()
        {
            const string prefix = ", ";
            string guestNames = guestNamesFieldPlaceHolder;

            for (int i = 0; i < sourceAppointment.Guests.Count; i++)
            {
                string guestName = sourceAppointment.Guests[i].Name;
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

        private void SaveButton_Click(object sender, RoutedEventArgs e) 
        {
            RunValidations();

            if (canSaveSourceAppointment)
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
            candidateGuests.RemoveAll(item => item == null);
            candidateGuests.RemoveAll(item => item.HasAppointmentCollision(sourceAppointment));

            sourceAppointment.Title = candidateTitle;
            sourceAppointment.Description = candidateDescription;
            sourceAppointment.Start = candidateStart;
            sourceAppointment.End = candidateEnd;
            sourceAppointment.AssignGuests(candidateGuests);
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
            candidateTitle = textBoxTitle.Text;
            candidateDescription = textBoxDescription.Text;
            candidateStart = sourceAppointment.Start.Date + GetCandidateTime("start");
            candidateEnd = sourceAppointment.End.Date + GetCandidateTime("end");
            candidateGuests = GetValidGuests();
        }

        private List<User> GetValidGuests()
        {
            List<User> result = new List<User>();
            List<string> candidateGuestNames = GetCandidateGuestNames();

            foreach (string name in candidateGuestNames)
            {
                try
                {
                    User candidateGuest = SessionController.GetUserByName(name);
                    bool isNotOwner = !sourceAppointment.IsOwner(candidateGuest);
                    if (isNotOwner)
                    {
                        result.Add(candidateGuest);
                    }
                }
                catch (ArgumentNullException)
                {
                    continue;
                }
            }

            return result;
        }

        private bool ExistsNullGuest()
        {
            List<string> candidateGuestNames = GetCandidateGuestNames();
            bool result = candidateGuestNames.Any(guestName => SessionController.GetUserByName(guestName) is null);
            return result;
        }

        private bool IsOwnerInvited()
        {
            bool result = false;
            List<string> candidateGuestNames = GetCandidateGuestNames();

            foreach (string name in candidateGuestNames)
            {
                try
                {
                    User candidateGuest = SessionController.GetUserByName(name);
                    bool isOwner = sourceAppointment.IsOwner(candidateGuest);
                    if (isOwner)
                    {
                        result = true;
                        break;
                    }
                }
                catch (ArgumentNullException)
                {
                    continue;
                }
            }
            return result;
        }

        private void ResetValidations() 
        {
            validationMessages.Clear();
            canSaveSourceAppointment = IsNotBlankTitle() & IsValidEndTime() & AreValidGuests() & !ExistingAppointmentCollision();
        }

        private bool AreValidGuests()
        {
            bool isGuestFieldEmpty = GetCandidateGuestNames().Count == 0;
            if (isGuestFieldEmpty | (!ExistsNullGuest() & !IsOwnerInvited()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ExistingAppointmentCollision() 
        {
            List<User> notNullCandidateGuests = candidateGuests.Where(guest => guest != null).ToList();
            bool existAppointmentCollision = notNullCandidateGuests.Any(guest => guest.HasAppointmentCollision(sourceAppointment));
            return existAppointmentCollision;
        }

        private void RefreshValidationMessages()
        {
            if (!IsNotBlankTitle())
            {
                validationMessages.Add(emptyTitleMessage);
            }
            if (!IsValidEndTime())
            {
                validationMessages.Add(invalidEndTimeMessage);
            }
            if (!AreValidGuests())
            {
                validationMessages.Add(invalidGuestsMessage);
                AddInvalidGuestNamesToValidationMessages();
            }
            if (ExistingAppointmentCollision())
            {
                validationMessages.Add(appointmentCollisionMessage);
                AddCollisionedGuestNamesToValidationMessages();
            }
        }

        private void AddCollisionedGuestNamesToValidationMessages()
        {
            const string prefix = "- ";
            foreach (string name in this.GetCandidateGuestNames())
            {
                User guest = SessionController.GetUserByName(name);
                bool isExistentUser = guest != null;
                if (isExistentUser & guest.HasAppointmentCollision(sourceAppointment))
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
                bool isOwnerUser = name == sourceAppointment.Owner.Name;
                bool isNonExistentUser = SessionController.GetUserByName(name) == null;
                bool isInvalidGuest = isOwnerUser | isNonExistentUser;
                if (isInvalidGuest)
                {
                    validationMessages.Add(prefix + name);
                }
            }
        }

        private List<string> GetCandidateGuestNames()
        {
            List<string> candidateGuestNames = textBoxGuests.Text.Split(',').ToList();

            List<string> placeHolderGuestNames = guestNamesFieldPlaceHolder.Split(',').ToList();
            candidateGuestNames.RemoveAll(candidateGuestName => placeHolderGuestNames.Contains(candidateGuestName));

            for (int i = 0; i < candidateGuestNames.Count; i++)
            {
                candidateGuestNames[i] = candidateGuestNames[i].Trim();
            }
            candidateGuestNames.RemoveAll(candidateGuestName => candidateGuestName.Length == 0);

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
