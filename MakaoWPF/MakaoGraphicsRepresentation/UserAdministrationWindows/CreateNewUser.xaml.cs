using System;
using System.Windows;

namespace MakaoGraphicsRepresentation.UserAdministrationWindows
{
    /// <summary>
    /// Interaction logic for CreateNewUser.xaml
    /// </summary>
    public partial class CreateNewUser : Window
    {
        #region Constructor

        public CreateNewUser()
        {
            InitializeComponent();
        }

        #endregion

        #region Button clicking reaction

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            int newID = 0;
            bool inputDataOK = CheckInputDataCorrectness();
            if (inputDataOK) newID = AddNewPlayerToRealm();

            if (newID == 0) MessageBox.Show("Nie udało się dodać nowego użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                OnAdditionSuccedeed(newID);
                this.Close();
            }
        }

        #endregion

        #region Checking inputData

        private bool CheckInputDataCorrectness()
        {
            bool keepGoing;

            keepGoing = CheckLoginLength();
            if (keepGoing) keepGoing = CheckLoginIndivituality();
            if (keepGoing) keepGoing = CheckNameLength();
            if (keepGoing) keepGoing = CheckPasswordLength();
            if (keepGoing) keepGoing = CheckPasswordEquality();

            return keepGoing;
        }

        #endregion

        #region Password input check

        private bool CheckPasswordLength()
        {
            bool output;
            if (PasswordBoxI.Password.Length <= 7 || PasswordBoxII.Password.Length <= 7) output = false;
            else output = true;

            if (!output)
            {
                MessageBox.Show("Hasło musi mieć co najmniej 8 znaków długości.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                PasswordBoxI.Password = "";
                PasswordBoxII.Password = "";
            }
            return output;
        }

        private bool CheckPasswordEquality()
        {
            bool output;
            if (PasswordBoxI.Password == PasswordBoxII.Password) output = true;
            else output = false;

            if (!output)
            {
                MessageBox.Show("Podane przez Ciebie hasła nie zgadzają się ze sobą.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                PasswordBoxI.Password = "";
                PasswordBoxII.Password = "";
            }
            return output;
        }

        #endregion

        #region Name and login checking

        private bool CheckLoginLength()
        {
            bool output;
            if (LoginTextBox.Text.Length < 5) output = false;
            else output = true;

            if (!output)
            {
                MessageBox.Show("Login musi mieć co najmniej 5 znaków długości.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                LoginTextBox.Text = "";
            }
            return output;
        }

        private bool CheckLoginIndivituality()
        {
            bool output;
            RealmUserHandler handler = new RealmUserHandler();
            output = handler.CheckIfLoginIsAvailable(LoginTextBox.Text);

            if (!output)
            {
                MessageBox.Show("Niestety, podany przez Ciebie login już istnieje.\nPodaj inny login.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                LoginTextBox.Text = "";
            }
            return output;
        }

        private bool CheckNameLength()
        {
            bool output;
            if (NameTextBox.Text.Length < 5) output = false;
            else output = true;

            if (!output)
            {
                MessageBox.Show("Imię musi mieć co najmniej 5 znaków długości.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                NameTextBox.Text = "";
            }
            return output;
        }

        #endregion

        #region Adding New Player Data

        private int AddNewPlayerToRealm()
        {
            int newID = 0;

            try
            {
                RealmUserHandler handler = new RealmUserHandler();
                newID = handler.AddNewPlayerToDB(NameTextBox.Text, PasswordBoxI.Password, LoginTextBox.Text);
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while trying to add new player to DataBase: {ex.Message}.");
            }

            return newID;
        }

        #endregion

        #region Creation succedeed event

        public delegate void AddingNesPlayerSuccedeedEventHandler(object sender, CreationOfNewPlayerSuccedeedEventArgs e);
        public event AddingNesPlayerSuccedeedEventHandler AdditionSuccedeed;
        protected virtual void OnAdditionSuccedeed(int playerID)
        {
            AdditionSuccedeed?.Invoke(this, new CreationOfNewPlayerSuccedeedEventArgs()
            { PlayerID = playerID, Success = true, });
        }

        #endregion
    }

    #region Class with arguments of event for successfull addition of new player

    public class CreationOfNewPlayerSuccedeedEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public int PlayerID { get; set; }
    }

    #endregion
}
