using Calendar.Controllers;
using Calendar.Windows.Partials;
using System;
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
        private SessionController sessionController = new SessionController();
        #endregion


        #region Properties
        #endregion


        #region Methods
        public MainWindow()
        {
            InitializeComponent();

            ShowLogOnWindow();

            if (sessionController.IsSessionLogoned())
            {
                InsertCalendarLayout();
            }
            else
            {
                this.Close();
            }
        }

        private void ShowLogOnWindow()
        {
            LogOnWindow logOnWindow = new LogOnWindow(this.sessionController);
            logOnWindow.ShowDialog();
        }

        private void InsertCalendarLayout()
        {
            CalendarLayout calendarLayout = new CalendarLayout(this.sessionController);
            gridMainWindow.Children.Add(calendarLayout);
        }
        #endregion
    }
}
