using System.Windows;
using Calendar.Controllers;

namespace Calendar.Windows
{
    /// <summary>
    /// Lógica de interacción para LogOnWindow.xaml
    /// </summary>
    public partial class LogOnWindow : Window
    {
        #region Constants
        private const string nonExistentUserMessage = "El usuario ingresado no existe";
        #endregion


        #region Fields
        private string userNameProvided;
        private SessionController sessionController;
        #endregion


        #region Properties
        #endregion


        #region Methods
        public LogOnWindow(SessionController sourceSessionController)
        {
            sessionController = sourceSessionController;
            InitializeComponent();
        }

        private void LogOnButton_Click(object sender, RoutedEventArgs e)
        {
            userNameProvided = textBoxUsernameProvided.Text;
            UserController userController = new UserController(userNameProvided);

            sessionController.LogOn(userController);

            if (sessionController.IsSessionLogoned())
            {
                this.Close();
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
        #endregion
    }
}
