using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Calendar
{
    /// <summary>
    /// Lógica de interacción para WeekColumn.xaml
    /// </summary>
    public partial class WeekColumn : UserControl
    {
        #region Constants
        private const int rowOffSetByTitleRows = 2;
        private const int minutesPerRow = 30;
        #endregion


        #region Fields   
        private int amountOfColumns;
        private readonly int dayNumberInWeek;
        private DateTime date;
        private Appointment selectedAppointment;
        private List<Appointment> dayAppointments = new List<Appointment>();
        private readonly Brush appointmentButtonBackground = Brushes.CornflowerBlue;
        private readonly Brush appointmentButtonForeground = Brushes.White;
        #endregion


        #region Properties
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

        public void AssignDayAppointments(List<Appointment> appointments)
        {
            dayAppointments = appointments;
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
                    bool canInsertWithoutColliding = !existingColumn
                        .Any(columnAppointment => columnAppointment.IsCollidingWith(appointment));
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
            const string empty = "";
            int spanValue = amountOfColumns;
            for (int i = 2; i < 50; i++)
            {
                Button buttonNewDayElementAppoinment = new Button
                {
                    Content = empty
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
            int appointmentYear = this.date.Year;
            int appointmentMonth = this.date.Month;
            int appointmentDay = this.date.Day;
            TimeSpan rowTime = GetRowTime(sender);
            User currentUser = SessionController.CurrenUser;
            DateTime appointmentStartDate = new DateTime(appointmentYear, appointmentMonth, appointmentDay) + rowTime;
            selectedAppointment = new Appointment(appointmentStartDate,currentUser);
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

        private bool IsValidAppointment()
        {
            bool isNotBlankTitle = selectedAppointment.Title.Trim().Length != 0;
            return isNotBlankTitle;
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
            TimeSpan startTimeOfAppointment = appointment.Start.TimeOfDay;
            TimeSpan startTimeOfDay = appointment.Start.Date.TimeOfDay;
            int appointmentStartTimeInMinutes = (int)(startTimeOfAppointment - startTimeOfDay).TotalMinutes;
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
                List<UIElement> gridChildrenInColumnI = WeekColumnGrid.Children.Cast<UIElement>()
                    .Where(child => Grid.GetColumn(child) == i & Grid.GetColumnSpan(child) == 1 ).ToList();
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

        private string GetColumnTitle()
        {
            const string columnTitleFormat = "{0} {1}";

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

            string dayNumberInMonth = date.Day.ToString(CultureInfo.CurrentCulture);
            string columnTitle = string.Format(CultureInfo.CurrentCulture, columnTitleFormat, dayName, dayNumberInMonth);

            return columnTitle;
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
            Button buttonSender = sender as Button;
            BindingExpression bindingExpressionOfAppointmentButton = buttonSender.GetBindingExpression(Button.ContentProperty);
            Binding bindingAppointmentButton = bindingExpressionOfAppointmentButton.ParentBinding;
            Appointment buttonSourceAppointment = bindingAppointmentButton.Source as Appointment;
            return buttonSourceAppointment;
        }

        #endregion
    }
}
