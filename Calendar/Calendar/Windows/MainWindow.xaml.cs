using Calendar.Controllers;
using Calendar.Windows.Partials;
using System.Windows;

namespace Calendar.Windows
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constants
        private const string contentResourceName = "mainWindowContentResourceName";
        #endregion


        #region Fields
        #endregion


        #region Properties
        #endregion


        #region Methods
        public MainWindow()
        {
            InitializeComponent();

            SessionController sessionController = new SessionController();

            LogOnWindow logOnWindow = new LogOnWindow(sessionController);
            logOnWindow.ShowDialog();

            if (sessionController.IsSessionLogoned())
            {
                CalendarLayout calendarLayout = new CalendarLayout(sessionController);
                gridMainWindow.Children.Add(calendarLayout);
            }
            else
            {
                this.Close();
            }
        }
        #endregion
    }
}
