using System;
using System.Windows;
using System.Windows.Controls;

namespace MakaoGraphicsRepresentation.UserAdministrationWindows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region Constructor

        public LoginWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Reaction for buttons (OK and cancel)

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsData successfullyLogInData;
            int playerID;
            bool loginOK = CheckIfThisLoginIsInDatabase();
            if (loginOK)
            {
                (successfullyLogInData, playerID) = Login();
                if (playerID > 0 && successfullyLogInData != null)
                {
                    OnLoginSuccedeed(successfullyLogInData, playerID, LoginTextBox.Text);
                    this.Close();
                }
                else
                {
                    LoginTextBox.Text = "";
                    PasswordBox.Password = "";
                    MessageBox.Show("Logowanie nieudane - nieodpowiednie hasło.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Checking data (from realm DB)

        private bool CheckIfThisLoginIsInDatabase()
        {
            RealmUserHandler handler = new RealmUserHandler();
            bool exists = !handler.CheckIfLoginIsAvailable(LoginTextBox.Text);
            if (!exists)
            {
                MessageBox.Show("Nie ma takiego Loginu", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
                LoginTextBox.Text = "";
                PasswordBox.Password = "";
            }
            return exists;
        }

        private (SettingsData, int) Login()
        {
            RealmUserHandler handler = new RealmUserHandler();
            return handler.Login(LoginTextBox.Text, PasswordBox.Password);
        }

        #endregion

        #region Login succedeed event

        public delegate void LoginSuccedeedEventHandler(object sender, LoginSuccedeedEventArgs e);
        public event LoginSuccedeedEventHandler LoginSuccedeed;
        protected virtual void OnLoginSuccedeed(SettingsData settings, int playerID, string login)
        {
            LoginSuccedeed?.Invoke(this, new LoginSuccedeedEventArgs()
            { PlayerID = playerID, Success = true, Login = login, Settings = settings, });
        }

        #endregion
    }

    #region Class with arguments of event for successfull login

    public class LoginSuccedeedEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public int PlayerID { get; set; }
        public string Login { get; set; }
        public SettingsData Settings { get; set; }
    }

    #endregion
}
