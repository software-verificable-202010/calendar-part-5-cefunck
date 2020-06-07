using System.Windows;
using System.Windows.Controls;

namespace Calendar
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
            Utilities.SetDisplayedDateToNow();
            InitializeComponent();
            BindingContent();
            Refresh();
        }

        public static void Refresh()
        {
            if (IsSessionLogoned())
            {
                RefreshAsCalendar();
            }
            else 
            {
                RefreshAsLogOnForm();
            }
        }

        private static bool IsSessionLogoned() 
        {
            bool isLogoned = SessionController.CurrenUser != null;
            return isLogoned;
        }

        private static void RefreshAsLogOnForm() 
        {
            LogOnForm logOnForm = new LogOnForm();
            App.Current.Resources[contentResourceName] = logOnForm;
        }

        private static void RefreshAsCalendar() 
        {
            CalendarLayout calendarLayout = new CalendarLayout();
            App.Current.Resources[contentResourceName] = calendarLayout;
        }

        private void BindingContent()
        {
            contenControl.SetResourceReference(ContentControl.ContentProperty, contentResourceName);
        }

        #endregion
    }
}
