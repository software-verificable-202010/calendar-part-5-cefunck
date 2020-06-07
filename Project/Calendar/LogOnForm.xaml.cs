using System.Windows;
using System.Windows.Controls;

namespace Calendar
{
    /// <summary>
    /// Lógica de interacción para LogOnForm.xaml
    /// </summary>
    public partial class LogOnForm : UserControl
    {
        #region Constants
        private const string nonExistentUserMessage = "El usuario ingresado no existe";
        #endregion


        #region Fields
        private User currentUser;
        private string userNameProvided;
        #endregion


        #region Properties
        #endregion


        #region Methods
        public LogOnForm()
        {
            InitializeComponent();
            currentUser = SessionController.GetUserByName(userNameProvided);
        }

        private void LogOnButton_Click(object sender, RoutedEventArgs e)
        {
            userNameProvided = textBoxUserNameProvided.Text;
            currentUser = SessionController.GetUserByName(userNameProvided);
            if (IsExistingUser())
            {
                SessionController.CurrenUser = currentUser;
                MainWindow.Refresh();
            }
            else 
            {
                ShowLogOnValidationMessage();
            }
        }

        private static void ShowLogOnValidationMessage()
        {
            MessageBox.Show(nonExistentUserMessage);
        }

        private bool IsExistingUser()
        {
            return !(currentUser is null);
        }

        #endregion
    }
}
