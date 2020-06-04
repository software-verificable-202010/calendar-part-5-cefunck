using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
        private readonly Brush appointmentButtonBackground = Brushes.CornflowerBlue;
        private readonly Brush appointmentButtonForeground = Brushes.White;
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
            ClearWeekColumnGrid();
            RefreshColumnsOfGrid();
            InsertTitle();
            InsertButtonsForNewAppointment();
            RefreshAppointments();
        }
        private void ClearWeekColumnGrid()
        {
            WeekColumnGrid.Children.Clear();
            WeekColumnGrid.ColumnDefinitions.Clear();
        }
        private void RefreshColumnsOfGrid()
        {
            RefreshAmountOfColumns();
            InsertColumnDefinitions();
        }
        private void InsertColumnDefinitions()
        {
            const double newAppointmentColumnWidth = 0.1;
            for (int i = 0; i < amountOfColumns; i++)
            {
                WeekColumnGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            WeekColumnGrid.ColumnDefinitions.Last().Width = new GridLength(newAppointmentColumnWidth, GridUnitType.Star);
        }
        private void RefreshAmountOfColumns()
        {
            const int columnOfNewAppointmentButtons = 1;
            List<List<Appointment>> appointmentsColumns = new List<List<Appointment>>();
            foreach (Appointment appointment in dayAppointments)
            {
                bool isAlreadyAdded = false;
                foreach (List<Appointment> existingColumn in appointmentsColumns)
                {
                    bool canInsertWithoutColliding = !existingColumn.Any(i => i.IsCollidingWith(appointment));
                    if (canInsertWithoutColliding)
                    {
                        existingColumn.Add(appointment);
                        isAlreadyAdded = true;
                        break;
                    }
                }
                if (!isAlreadyAdded)
                {
                    List<Appointment> newColumn = new List<Appointment>()
                    {
                        appointment
                    };
                    appointmentsColumns.Add(newColumn);
                }
            }
            amountOfColumns = appointmentsColumns.Count + columnOfNewAppointmentButtons;
        }

        internal void AssignDayAppointments(List<Appointment> appointments)
        {
            dayAppointments = appointments;
        }

        private void InsertTitle()
        {
            const double thicknessValue = 0.2;
            const int rowSpanValue = 2;
            int columnSpanValue = amountOfColumns;
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
            textBlockTitle.SetValue(Grid.ColumnSpanProperty, columnSpanValue);
            textBlockTitle.SetValue(Grid.RowSpanProperty, rowSpanValue);
            borderTitle.SetValue(Grid.ColumnSpanProperty, columnSpanValue);
            borderTitle.SetValue(Grid.RowSpanProperty, rowSpanValue);
            WeekColumnGrid.Children.Add(borderTitle);
            WeekColumnGrid.Children.Add(textBlockTitle);
        }
        private void InsertButtonsForNewAppointment()
        {
            int spanValue = amountOfColumns;
            for (int i = 2; i < 50; i++)
            {
                Button buttonNewDayElementAppoinment = new Button
                {
                    Content = Utilities.BlankSpace
                };
                buttonNewDayElementAppoinment.Click += NewAppointmentButton_Click;
                buttonNewDayElementAppoinment.SetValue(Grid.RowProperty, i);
                buttonNewDayElementAppoinment.SetValue(Grid.ColumnSpanProperty, spanValue);
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
                int columnValue = GetAppointmentColumn(appointment);
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
            Appointment buttonSourceAppointment = GetAppointmentOfButton(sender);
            int appointmentIndexInDayAppointents = dayAppointments.IndexOf(buttonSourceAppointment);
            selectedAppointment = dayAppointments[appointmentIndexInDayAppointents];
        }
        private void RefreshSelectedAppointmentAsNew(object sender)
        {
            const int defaultDurationAppointmentInMinutes = 30;
            string emptyTextField = Utilities.BlankSpace;
            int appointmentYear = this.date.Year;
            int appointmentMonth = this.date.Month;
            int appointmentDay = this.date.Day;
            TimeSpan rowTime = GetRowTime(sender);
            User currentUser = SessionController.CurrenUser;
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
        private static TimeSpan GetRowTime(object sender)
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
        private static Appointment GetAppointmentOfButton(object sender)
        {
            BindingExpression bindingExpressionOfAppointmentButton = (sender as Button).GetBindingExpression(Button.ContentProperty);
            Binding bindingAppointmentButton = bindingExpressionOfAppointmentButton.ParentBinding;
            Appointment buttonSourceAppointment = bindingAppointmentButton.Source as Appointment;
            return buttonSourceAppointment;
        }
        private string GetColumnTitle() 
        {
            string dayName;
            switch (dayNumberInWeek)
            {
                case Utilities.MondayNumberInweek:
                    dayName = Utilities.MondayName;
                    break;
                case Utilities.TuesdayNumberInweek:
                    dayName = Utilities.TuesdayName;
                    break;
                case Utilities.WednesdayNumberInweek:
                    dayName = Utilities.WednesdayName;
                    break;
                case Utilities.ThursdayNumberInweek:
                    dayName = Utilities.ThursdayName;
                    break;
                case Utilities.FridayNumberInweek:
                    dayName = Utilities.FridayName;
                    break;
                case Utilities.SaturdayNumberInweek:
                    dayName = Utilities.SaturdayName;
                    break;
                default:
                    dayName = Utilities.SundayName;
                    break;
            }
            return dayName + Utilities.BlankSpace + date.Day.ToString(CultureInfo.CurrentCulture);
        }
        private static int GetAppointmentRowSpan(Appointment appointment) 
        {
            double appointmentDurationInMinutes = (appointment.End - appointment.Start).TotalMinutes;
            int appointmentRowSpan = (int)Math.Ceiling(appointmentDurationInMinutes / minutesPerRow);
            return appointmentRowSpan;
        }
        private static int GetAppointmentColumnSpan()
        {
            return 1;
        }
        private static int GetAppointmentRow(Appointment appointment) 
        {
            int appointmentStartTimeInMinutes = (int)(appointment.Start.TimeOfDay - appointment.Start.Date.TimeOfDay).TotalMinutes;
            int appointmentRow = (int)Math.Floor((double)appointmentStartTimeInMinutes / minutesPerRow) + rowOffSetByTitleRows;
            return appointmentRow;
        }
        private int GetAppointmentColumn(Appointment appointment)
        {
            const int defaultColumns = 2;
            int columnIndex = 0;
            for (int i = 0; i < amountOfColumns - defaultColumns; i++)
            {
                bool isCollisionFreeColumn = true;
                List<UIElement> gridChildrenInColumnI = WeekColumnGrid.Children.Cast<UIElement>().Where(child => Grid.GetColumn(child) == i & Grid.GetColumnSpan(child) == 1 ).ToList();
                foreach (UIElement child in gridChildrenInColumnI.Where(j=>j.GetType() == typeof(Button)))
                {
                    Appointment otherAppointment = GetAppointmentOfButton(child as Button);
                    if (appointment.IsCollidingWith(otherAppointment))
                    {
                        isCollisionFreeColumn = false;
                        break;
                    }
                }
                if (isCollisionFreeColumn)
                {
                    columnIndex = i;
                    break;
                }
                else
                {
                    columnIndex = amountOfColumns - defaultColumns;
                }
            }
            return columnIndex;
        }
        #endregion
    }
}
