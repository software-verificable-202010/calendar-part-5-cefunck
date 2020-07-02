using Calendar.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Calendar.Models;


namespace Calendar.Windows
{
    /// <summary>
    /// Lógica de interacción para AppointmentWindow.xaml
    /// </summary>
    public partial class AppointmentWindow : Window
    {
        #region Constants
        private const int hoursInOneDay = 24;
        private const int minutesInOneHour = 60;
        private const string guestNamesFieldPlaceHolder = "Ej: un_nombre, otro_nombre";
        #endregion


        #region Fields
        private AppointmentController sourceAppointmentController;
        private SessionController sessionController;
        #endregion


        #region Properties
        #endregion


        #region Methods
        
        public AppointmentWindow(
            AppointmentController sourceAppointmentController,
            SessionController sourceSessionController)
        {
            this.sourceAppointmentController = sourceAppointmentController;
            this.sessionController = sourceSessionController;
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
            string currentUsername = sessionController.CurrentUserName;
            sourceAppointmentController.RefreshPermissions(currentUsername);

            RefreshUIFields();

            if (sourceAppointmentController.IsEditingExistingAppointment())
            {
                ResizeSaveButtonForEditForm();
                InsertDeleteButtonForEditForm();
            }
        }

        private void RefreshUIFields()
        {
            string title = sourceAppointmentController.GetSourceAppointmentTitle();
            string description = sourceAppointmentController.GetSourceAppointmentDescription();
            string guestNames = GetAppointmentGuestNamesInFieldFormat();
            int startHour = sourceAppointmentController.GetSourceAppointmentStart().Hour;
            int startMinute = sourceAppointmentController.GetSourceAppointmentStart().Minute;
            int endHour = sourceAppointmentController.GetSourceAppointmentEnd().Hour;
            int endMinute = sourceAppointmentController.GetSourceAppointmentEnd().Minute;

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
            if (IsGuestNamesFieldStillPlaceHolder(guestNamesField))
            {
                textBoxGuests.SelectionChanged += GuestNamesField_SelectionChanged;
            }
        }
        
        private void GuestNamesField_SelectionChanged(object sender, RoutedEventArgs e)
        {
            textBoxGuests.SelectionChanged -= GuestNamesField_SelectionChanged;
            textBoxGuests.Text = string.Empty;
        }
        
        private void ResizeSaveButtonForEditForm() 
        {
            const int saveButtonColumnSpan = 3;
            buttonSave.SetValue(Grid.ColumnSpanProperty, saveButtonColumnSpan);
            buttonSave.IsEnabled = sourceAppointmentController.HasOwnerPermissions;
        }
        
        private void InsertDeleteButtonForEditForm() 
        {
            const int deleteButtonColumnSpan = 3;
            const int deleteButtonColumn = 3;
            const int deleteButtonRow = 4;
            const string deleteButtonContent = "Eliminar";

            Button buttonDelete = new Button
            {
                Content = deleteButtonContent,
                IsEnabled = sourceAppointmentController.HasOwnerPermissions
            };
            buttonDelete.SetValue(Grid.ColumnSpanProperty, deleteButtonColumnSpan);
            buttonDelete.SetValue(Grid.ColumnProperty, deleteButtonColumn);
            buttonDelete.SetValue(Grid.RowProperty, deleteButtonRow);
            buttonDelete.Click += DeleteButton_Click;

            grid.Children.Add(buttonDelete);
        }
        
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            sourceAppointmentController.DeleteSourceAppointment();
            this.Close();
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e) 
        {
            // TODO: isolate in one step methods
            const string startSymbol = "start";
            const string endSymbol = "end";

            string candidateTitle = textBoxTitle.Text;
            string candidateDescription = textBoxDescription.Text;
            List<string> candidateGuestsNames = GetCandidateGuestNames(textBoxGuests.Text);
            TimeSpan candidateStart = GetCandidateTime(startSymbol);
            TimeSpan candidateEnd = GetCandidateTime(endSymbol);

            sourceAppointmentController.RefreshCandidateData(candidateTitle, candidateDescription, candidateGuestsNames, candidateStart, candidateEnd);
            sourceAppointmentController.RefreshValidationMessages();

            if (sourceAppointmentController.CanSaveSourceAppointment())
            {
                sourceAppointmentController.SaveAppointmentData();
                this.Close();
            }
            else 
            {
                ShowValidationMessages();
            }
        }
        
        private void ShowValidationMessages() 
        {
            const string messageFormat = "{0}{1}\n";
            string validationMessage = "";
            
            foreach (string message in sourceAppointmentController.ValidationMessages)
            {
                validationMessage = string.Format(CultureInfo.CurrentCulture,messageFormat, validationMessage, message);
            }

            MessageBox.Show(validationMessage);
        }

        private TimeSpan GetCandidateTime(string requiredTime)
        {
            const string startSymbol = "start";
            const int defaultSeconds = 0;
            int startHour = comboBoxStartHour.SelectedIndex;
            int startMinute = comboBoxStartMinute.SelectedIndex;
            int endHour = comboBoxEndHour.SelectedIndex;
            int endMinute = comboBoxEndMinute.SelectedIndex;

            bool isRequiredStartTime = requiredTime == startSymbol;
            if (isRequiredStartTime)
            {
                TimeSpan startTime = new TimeSpan(startHour, startMinute, defaultSeconds);
                return startTime;
            }
            else
            {
                TimeSpan endTime = new TimeSpan(endHour, endMinute, defaultSeconds);
                return endTime;
            }
        }

        private List<string> GetCandidateGuestNames(string guestsFieldText)
        {
            List<string> candidateGuestNames = guestsFieldText.Split(',').ToList();

            List<string> placeHolderGuestNames = guestNamesFieldPlaceHolder.Split(',').ToList();
            candidateGuestNames.RemoveAll(name => placeHolderGuestNames.Contains(name));

            for (int i = 0; i < candidateGuestNames.Count; i++)
            {
                candidateGuestNames[i] = candidateGuestNames[i].Trim();
            }
            candidateGuestNames.RemoveAll(name => name.Length == 0);

            return candidateGuestNames;
        }

        private string GetAppointmentGuestNamesInFieldFormat()
        {
            const string guestNamesFormat = "{0}, {1}";
            bool isFirstIteration = true;
            string guestNames = guestNamesFieldPlaceHolder;
            List<string> sourceAppointmentGuestsUserNames = sourceAppointmentController.GetSourceAppointmentGuestsUserNames();

            for (int i = 0; i < sourceAppointmentGuestsUserNames.Count; i++)
            {
                string guestName = sourceAppointmentGuestsUserNames[i];
                if (isFirstIteration)
                {
                    guestNames = guestName;
                    isFirstIteration = false;
                }
                else
                {
                    guestNames = string.Format(CultureInfo.CurrentCulture, guestNamesFormat, guestNames, guestName);
                }
            }

            return guestNames;
        }

        private bool IsGuestNamesFieldStillPlaceHolder(string guestNamesFieldText)
        {
            return guestNamesFieldText == guestNamesFieldPlaceHolder;
        }
        #endregion
    }
}
