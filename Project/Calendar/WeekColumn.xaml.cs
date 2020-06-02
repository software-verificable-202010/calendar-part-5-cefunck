using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
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
    /// Lógica de interacción para WeekColumn.xaml
    /// </summary>
    public partial class WeekColumn : UserControl
    {
        #region Constants
        const int rowOffSetByTitleRows = 2;
        const int minutesPerRow = 30;
        #endregion

        #region Fields
        //TODO: hacer globales estos colores para que lo usen login y apointment es vista mensual y semanal
        private Brush appointmentButtonBackground = Brushes.CornflowerBlue;
        private Brush appointmentButtonForeground = Brushes.White;
        private List<Appointment> dayAppointments = new List<Appointment>();
        private DateTime date;
        private Appointment selectedAppointment;
        private int dayNumberInWeek;
        private int amountOfColumns;
        #endregion

        #region Properties
        public int DayNumberInWeek
        {
            get 
            {
                return dayNumberInWeek;
            }
            set 
            {
                dayNumberInWeek = value;
            }
        }
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
        public WeekColumn(DateTime date, int dayNumberInWeek) 
        {
            this.date = date;
            this.dayNumberInWeek = dayNumberInWeek;
            InitializeComponent();
        }
        public void Refresh()
        {
            RefreshColumnsOfGrid();
            InsertTitle();
            InsertButtonsForNewAppointment();
            RefreshAppointments();
        }
        private void RefreshColumnsOfGrid()
        {
            WeekColumnGrid.Children.Clear();
            refreshAmountOfColumns();

        }
        private void refreshAmountOfColumns()
        {
            const int defaultColumns = 2;
            int maxAppointmentCollisions = 0;
            foreach (Appointment appointment in dayAppointments)
            {
                int appointmentCollisions = 0;
                foreach (Appointment otherAppointment in dayAppointments)
                {
                    if (appointment != otherAppointment & appointment.IsCollidingWith(otherAppointment))
                    {
                        appointmentCollisions++;
                    }
                }
                if (appointmentCollisions > maxAppointmentCollisions)
                {
                    maxAppointmentCollisions = appointmentCollisions;
                }
            }
            amountOfColumns = maxAppointmentCollisions + defaultColumns;

        }
        private void InsertTitle()
        {
            const int spanValue = 2;
            const double thicknessValue = 0.2;
            Brush brushTitleBorderBackgroundColor = Brushes.White;
            Thickness thicknessTitleBorder = new Thickness(thicknessValue);
            Border borderTitle = new Border()
            {
                Background = brushTitleBorderBackgroundColor,
                BorderThickness = thicknessTitleBorder
            };
            TextBlock textBlockTitle = new TextBlock()
            {
                Text = GetColumnTitle(),
                VerticalAlignment = VerticalAlignment.Center
            };
            textBlockTitle.SetValue(Grid.ColumnSpanProperty, spanValue);
            textBlockTitle.SetValue(Grid.RowSpanProperty, spanValue);
            borderTitle.SetValue(Grid.ColumnSpanProperty, spanValue);
            borderTitle.SetValue(Grid.RowSpanProperty, spanValue);
            WeekColumnGrid.Children.Add(borderTitle);
            WeekColumnGrid.Children.Add(textBlockTitle);
        }
        private void InsertButtonsForNewAppointment()
        {
            for (int i = 2; i < 50; i++)
            {
                Button buttonNewDayElementAppoinment = new Button
                {
                    Content = Utilities.blankSpace
                };
                buttonNewDayElementAppoinment.Click += NewAppointmentButton_Click;
                buttonNewDayElementAppoinment.SetValue(Grid.RowProperty, i);                
                WeekColumnGrid.Children.Add(buttonNewDayElementAppoinment);
            }
        }
        private void RefreshAppointments()
        {

            const string bindingPropertyName = "Title";
            foreach (Appointment appointment in dayAppointments)
            {
                const double fontSizeAppointmentButtonValue = 9;
                int rowValue = GetAppointmentRow(appointment);
                int columnValue = GetAppointmentColumn();
                int rowSpanValue = GetAppointmentRowSpan(appointment);
                int columnSpanValue = GetAppointmentColumnSpan();
                Binding appointmentBinding = new Binding(bindingPropertyName)
                {
                    Source = appointment
                };
                Button buttonDayElementAppoinment = new Button();
                buttonDayElementAppoinment.SetBinding(ContentProperty, appointmentBinding);
                buttonDayElementAppoinment.Click += EditAppointmentButton_Click;
                buttonDayElementAppoinment.SetValue(Grid.RowProperty, rowValue);
                buttonDayElementAppoinment.SetValue(Grid.ColumnProperty, columnValue);
                buttonDayElementAppoinment.SetValue(Grid.RowSpanProperty, rowSpanValue);
                buttonDayElementAppoinment.SetValue(Grid.ColumnSpanProperty, columnSpanValue);
                buttonDayElementAppoinment.SetValue(Button.BackgroundProperty, appointmentButtonBackground);
                buttonDayElementAppoinment.SetValue(Button.ForegroundProperty, appointmentButtonForeground);
                buttonDayElementAppoinment.SetValue(Button.FontSizeProperty, fontSizeAppointmentButtonValue);
                WeekColumnGrid.Children.Add(buttonDayElementAppoinment);
            }
        }
        private void NewAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshSelectedAppointmentAsNew(sender);
            ShowAppointmentForm();
            SaveNewAppointment();
            Refresh();
        }
        private void EditAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshSelectedAppointment(sender);
            ShowAppointmentForm();
            SaveAppointmentChanges();
            Refresh();
        }
        private void RefreshSelectedAppointment(object sender)
        {
            BindingExpression bindingExpressionOfAppointmentButton = (sender as Button).GetBindingExpression(Button.ContentProperty);
            Binding bindingAppointmentButton = bindingExpressionOfAppointmentButton.ParentBinding;
            Appointment buttonSourceAppointment = bindingAppointmentButton.Source as Appointment;
            int appointmentIndexInDayAppointents = dayAppointments.IndexOf(buttonSourceAppointment);
            selectedAppointment = dayAppointments[appointmentIndexInDayAppointents];
        }
        private void RefreshSelectedAppointmentAsNew(object sender)
        {
            const int defaultDurationAppointmentInMinutes = 30;
            string emptyTextField = Utilities.blankSpace;
            int appointmentYear = this.date.Year;
            int appointmentMonth = this.date.Month;
            int appointmentDay = this.date.Day;
            TimeSpan rowTime = GetRowTime(sender);
            User currentUser = SessionController.GetCurrenUser();
            DateTime appointmentStartDate = new DateTime(appointmentYear, appointmentMonth, appointmentDay) + rowTime;
            DateTime appointmentEndDate = appointmentStartDate.AddMinutes(defaultDurationAppointmentInMinutes);
            selectedAppointment = new Appointment(emptyTextField, emptyTextField, appointmentStartDate, appointmentEndDate, currentUser);
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
        private void SaveNewAppointment()
        {
            if (IsValidAppointment())
            {
                UpdateDayAppointments();
                UpdateCalendarAppointments();
            }
        }
        private bool IsValidAppointment()
        {
            if (selectedAppointment.Title.Trim().Length != 0)
            {
                return true;
            }
            return false;
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
        private TimeSpan GetRowTime(object sender)
        {
            const double minutesInOneHour = 60;
            const int rowSeconds = 0;
            int appointmentButtonRow = (int)(sender as Button).GetValue(Grid.RowProperty);
            int minutesOfAppontmentStarFromZeroHours = (appointmentButtonRow - rowOffSetByTitleRows) * minutesPerRow;
            int rowHour = (int)Math.Floor(minutesOfAppontmentStarFromZeroHours / minutesInOneHour);
            int rowMinutes = (int)(minutesOfAppontmentStarFromZeroHours - rowHour * minutesInOneHour);
            TimeSpan rowTime = new TimeSpan(rowHour, rowMinutes, rowSeconds);
            return rowTime;
        }
        private string GetColumnTitle() 
        {
            string dayName;
            switch (dayNumberInWeek)
            {
                case Utilities.mondayNumberInweek:
                    dayName = Utilities.mondayName;
                    break;
                case Utilities.tuesdayNumberInweek:
                    dayName = Utilities.tuesdayName;
                    break;
                case Utilities.wednesdayNumberInweek:
                    dayName = Utilities.wednesdayName;
                    break;
                case Utilities.thursdayNumberInweek:
                    dayName = Utilities.thursdayName;
                    break;
                case Utilities.fridayNumberInweek:
                    dayName = Utilities.fridayName;
                    break;
                case Utilities.saturdayNumberInweek:
                    dayName = Utilities.saturdayName;
                    break;
                default:
                    dayName = Utilities.sundayName;
                    break;
            }
            return dayName + Utilities.blankSpace + date.Day.ToString();
        }
        private int GetAppointmentRowSpan(Appointment appointment) 
        {
            double appointmentDurationInMinutes = (appointment.End - appointment.Start).TotalMinutes;
            int appointmentRowSpan = (int)Math.Ceiling(appointmentDurationInMinutes / minutesPerRow);
            return appointmentRowSpan;
        }
        private int GetAppointmentColumnSpan()
        {
            return 1;
        }
        private int GetAppointmentRow(Appointment appointment) 
        {
            int appointmentStartTimeInMinutes = (int)(appointment.Start.TimeOfDay - appointment.Start.Date.TimeOfDay).TotalMinutes;
            int appointmentRow = (int)Math.Floor((double)appointmentStartTimeInMinutes / minutesPerRow) + rowOffSetByTitleRows;
            return appointmentRow;
        }
        private int GetAppointmentColumn()
        {
            return 0;
        }
        #endregion
    }
}
