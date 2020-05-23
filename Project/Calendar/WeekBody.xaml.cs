using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para WeekBody.xaml
    /// </summary>
    public partial class WeekBody : UserControl
    {
        #region Constants
        private const string columnTitleResourceKeyPrefix = "WeekColumnTitle";
        private const int firstColumnIndex = 1;
        private const int lastColumnIndex = 7;
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Methods
        public WeekBody()
        {
            InitializeComponent();
            List<WeekColumn> dayColumnElements = CreateDayColumnElements();
            InsertDayColumnElementsToWeekBody(dayColumnElements);

        }
        public void Refresh() 
        {
            RefreshWeekBodyDayColumnTitles();
        }
        private void RefreshWeekBodyDayColumnTitles()
        {
            for (int i = 1; i <= Utilities.daysInWeek; i++)
            {
                DateTime displayedDate = Utilities.GetDisplayedDate();
                int dayOfWeek = Utilities.GetDayNumberInWeek(displayedDate);
                displayedDate = displayedDate.AddDays(Utilities.negativeMultiplier * dayOfWeek + i);
                string columnTitleResourceKey = columnTitleResourceKeyPrefix + i.ToString();
                string columnTitleResourceValue = Utilities.GetNameOfDayInSpanish(displayedDate) + Utilities.blankSpace + displayedDate.Day.ToString();
                App.Current.Resources[columnTitleResourceKey] = columnTitleResourceValue;
            }
        }
        public List<WeekColumn> CreateDayColumnElements()
        {
            List<WeekColumn> dayColumns = new List<WeekColumn>();
            for (int weekColumnIndex = firstColumnIndex; weekColumnIndex <= lastColumnIndex; weekColumnIndex++)
            {
                WeekColumn weekColumnElement = new WeekColumn(weekColumnIndex);
                weekColumnElement.SetValue(Grid.ColumnProperty, weekColumnIndex);
                dayColumns.Add(weekColumnElement);
            }
            return dayColumns;
        }
        public void InsertDayColumnElementsToWeekBody(List<WeekColumn> dayColumnElements)
        {
            foreach (WeekColumn dayColumnElement in dayColumnElements)
            {
                WeekBodyGrid.Children.Add(dayColumnElement);
            }
        }
        #endregion
    }
}
