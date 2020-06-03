using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Lógica de interacción para LoginForm.xaml
    /// </summary>
    public partial class LoginForm : UserControl
    {
        #region Constants
        const string nonExistentUserMessage = "El usuario ingresado no existe";
        #endregion

        #region Fields
        private User currentUser;
        private List<User> calendarUsers;
        private string userNameProvided;
        #endregion

        #region Properties
        #endregion

        #region Methods
        public LoginForm()
        {
            InitializeComponent();
            calendarUsers = SessionController.GetCalendarUsers();
            currentUser = SessionController.GetUserByName(userNameProvided);
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            userNameProvided = textBoxUserNameProvided.Text;
            calendarUsers = SessionController.GetCalendarUsers();
            currentUser = SessionController.GetUserByName(userNameProvided);
            if (IsExistingUser())
            {
                SessionController.SetCurrenUser(currentUser);
                MainWindow.Refresh();
            }
            else 
            {
                ShowLoginValidationMessage();
            }
        }
        private void ShowLoginValidationMessage()
        {
            MessageBox.Show(nonExistentUserMessage);
        }
        private bool IsExistingUser()
        {
            if (currentUser != null)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
