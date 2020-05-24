using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (IsSessionLogined())
            {
                RefreshAsCalendar();
            }
            else 
            {
                RefreshAsLoginForm();
            }
        }
        private static bool IsSessionLogined() 
        {
            if (SessionController.GetCurrenUser() != null)
            {
                return true;
            }
            return false;
        }
        private static void RefreshAsLoginForm() 
        {
            LoginForm loginForm = new LoginForm();
            App.Current.Resources[contentResourceName] = loginForm;
        }
        private static void RefreshAsCalendar() 
        {
            CalendarLayout calendar = new CalendarLayout();
            App.Current.Resources[contentResourceName] = calendar;
        }
        private void BindingContent()
        {
            contenControl.SetResourceReference(ContentControl.ContentProperty, contentResourceName);
        }
        #endregion
    }
}
