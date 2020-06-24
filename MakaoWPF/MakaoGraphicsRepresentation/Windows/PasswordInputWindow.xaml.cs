using System;
using System.Windows;
using System.Windows.Controls;

namespace MakaoGraphicsRepresentation.Windows
{
    /// <summary>
    /// Interaction logic for PasswordInputWindow.xaml
    /// </summary>
    public partial class PasswordInputWindow : Window
    {
        #region Constructor

        //constructor
        public PasswordInputWindow()
        {
            InitializeComponent();

            //set starting password to null
            PasswordBox.Password = null;

            //log opening of the window
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Password input window opened.");
        }

        #endregion

        #region GUI handling

        //when closing window
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //log opening of the window
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Password input window is closing.");
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            //fire the event if inputed password is not equal to null
            if (PasswordBox.Password != null && PasswordBox.Password != "")
            {
                OnPasswordInputFinished(PasswordBox.Password);
                this.Close();
            }
            else
            {
                //log opening of the window
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Password input window  - empty password box while closing.");

                //show message to user
                MessageBox.Show("Pole do wprowadzania hasła nie może pozostac puste.",
                    "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region Event that fires when password was not null

        public delegate void PasswordWindowPasswordConfirmedEventHandler(object sender, PasswordInputWindowClosingArgs e);
        public event PasswordWindowPasswordConfirmedEventHandler PasswordInputFinished;
        protected virtual void OnPasswordInputFinished(string password)
        {
            PasswordInputFinished?.Invoke(this, new PasswordInputWindowClosingArgs { Password = password });
        }

        #endregion
    }

    #region Class for clising event args

    public class PasswordInputWindowClosingArgs : EventArgs
    {
        public string Password;
    }

    #endregion
}
