using Calendar.Controllers;
using System.Windows.Controls;

namespace Calendar.Windows.Partials
{
    /// <summary>
    /// Lógica de interacción para CalendarLayout.xaml
    /// </summary>
    public partial class CalendarLayout : UserControl
    {
        #region Constants
        #endregion


        #region Fields
        private SessionController sourceSessionController;
        private CalendarNavbar calendarNavbar;
        #endregion


        #region Properties
        #endregion


        #region Methods
        public CalendarLayout(SessionController sourceSessionController)
        {
            InitializeComponent();
            this.sourceSessionController = sourceSessionController;
            CreateCalendarNavbar();
            InsertCalendarNavbar();
        }

        private void CreateCalendarNavbar()
        {
            calendarNavbar = new CalendarNavbar(sourceSessionController);
        }

        private void InsertCalendarNavbar()
        {
            calendarNavbar.SetValue(Grid.ColumnProperty, 0);
            calendarNavbar.SetValue(Grid.RowProperty, 0);
            GridCalendarLayout.Children.Add(calendarNavbar);
        }
        #endregion
    }
}
