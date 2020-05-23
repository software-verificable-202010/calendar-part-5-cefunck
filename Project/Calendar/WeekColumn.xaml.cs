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
        private const string displayedDateResourceName = "displayedDate";
        private const string columnTitleResourceKeyPrefix = "WeekColumnTitle";
        private const string columnTitleNamePrefix = "WeekColumnTitleElement";
        #endregion

        #region Fields
        private string title;
        private int index;
        #endregion

        #region Properties
        public string Title 
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }
        public int Index
        {
            get 
            {
                return index;
            }
            set 
            {
                index = value;
            }
        }
        #endregion

        #region Methods
        public WeekColumn(int columnIndex)
        {
            InitializeComponent();
            Index = columnIndex;
            GenerateTitleResource();
            AssingValuesToTitleResource();
            InsertTitleElementToColumn();
        }
        private void GenerateTitleResource()
        {
            if (App.Current.Resources[GetTitleResourceKey()] == null)
            {
                App.Current.Resources.Add(GetTitleResourceKey(), GetTitleResourceValue());
            }
        }
        private void AssingValuesToTitleResource()
        {
            App.Current.Resources[GetTitleResourceKey()] = GetTitleResourceValue();
        }
        private void InsertTitleElementToColumn()
        {
            TextBlock textBlockTitle = new TextBlock();
            textBlockTitle.Name = columnTitleNamePrefix + Index.ToString();
            textBlockTitle.SetResourceReference(TextBlock.TextProperty, GetTitleResourceKey());
            textBlockTitle.SetValue(Grid.ColumnProperty, 0);
            textBlockTitle.SetValue(Grid.RowProperty, 0);
            textBlockTitle.SetValue(Grid.ColumnSpanProperty, 2);
            textBlockTitle.SetValue(Grid.RowSpanProperty, 2);
            WeekColumnGrid.Children.Add(textBlockTitle);
        }
        private DateTime GetDisplayedDateResourceValue()
        {
            return (DateTime)App.Current.Resources[displayedDateResourceName];
        }
        private DateTime GetDateOfColumn()
        {
            DateTime displayedDate = GetDisplayedDateResourceValue();
            int year = displayedDate.Year;
            int month = displayedDate.Month;
            int day = (int)GetDayNumber();
            return new DateTime(year, month, day);
        }
        private int GetRowIndex()
        {
            // HACK: Temporary fix until able to Refactor.
            const int fixedRowNumber = 2;
            return fixedRowNumber;
        }
        private int GetColumIndex()
        {
            // HACK: Temporary fix until able to Refactor.
            const int fixedColumnNumber = 0;
            return fixedColumnNumber;
        }
        private int GetRowSpan()
        {
            // HACK: Temporary fix until able to Refactor.
            const int fixedRowSpan = 20;
            return fixedRowSpan;
        }
        private int GetColumnSpan()
        {
            // HACK: Temporary fix until able to Refactor.
            const int fixedColumnSpan = 1;
            return fixedColumnSpan;
        }
        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            // TODO: to complete
            Button pressedCellOfWeekColumn = (Button)sender;
            int pressedCellRow = (int)pressedCellOfWeekColumn.GetValue(Grid.RowProperty);
            Appointment appointment = new Appointment("","",DateTime.Now,DateTime.Now);
            AppointmentWindow appointmenWindow = new AppointmentWindow(appointment);
            appointmenWindow.ShowDialog();
        }
        private string GetDayName()
        {
            string dayName;
            switch (Index)
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
            return dayName;
        }
        private int GetDayNumber()
        {
            DateTime selectedDate = GetDisplayedDateResourceValue();
            int dayOfWeek = Utilities.GetDayNumberInWeek(selectedDate);
            selectedDate = selectedDate.AddDays(Utilities.negativeMultiplier * dayOfWeek + Index);
            return selectedDate.Day;
        }
        private string GetTitleResourceKey()
        {
            return columnTitleResourceKeyPrefix + Index;
        }
        private string GetTitleResourceValue()
        {
            return GetDayName() + Utilities.blankSpace + GetDayNumber().ToString();
        }
        #endregion
    }
}
