using System;
using System.Windows;
using System.Windows.Controls;

namespace MakaoGraphicsRepresentation.Windows
{
    /// <summary>
    /// Interaction logic for StartNewHostWindow.xaml
    /// </summary>
    public partial class StartNewHostWindow : Window
    {
        #region Fields and properties

        //corectness of the passwords
        private bool passwordCorrect;
        public bool PasswordCorrect{ get { return passwordCorrect; } }

        private int amountOfPlayers;
        private int amountOfDecks;
        private int amountOfJokers;
        private int amountOfStartCards;
        private string password;

        #endregion

        #region Constructing the window

        //constructor
        public StartNewHostWindow(int amountOfPlayers, int decksAmount, int jokersAmount, int startCardsAmount)
        {
            InitializeComponent();
            SetDefaultAmountOfPlayers(amountOfPlayers);
            SetDefaultAmountOfDecks(decksAmount);
            SetDefaultAmountOfJokers(jokersAmount);
            SetDefaultAmountOfCards(startCardsAmount);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("New game creation window opened, default amount of players set to: " + amountOfPlayers.ToString());
        }

        //method for selecting default amount of players while opening window
        private void SetDefaultAmountOfPlayers(int amount)
        {
            if (amount < 2 || amount > 4)
            {
                throw new ArgumentOutOfRangeException("Amount of players should be equal to 2, 3 or 4; but not to: " + amount.ToString());
            }
            else
            {
                AmountOfPlayersComboBox.SelectedIndex = amount - 2;
                this.amountOfPlayers = amount;
            }
        }

        //method for selecting default amount of decks in game while opening window
        private void SetDefaultAmountOfDecks(int amount)
        {
            if (amount < 1 || amount > 4)
            {
                throw new ArgumentOutOfRangeException("Amount of decks should be between to 1 and 4; but not to: " + amount.ToString());
            }
            else
            {
                AmountOfDecksComboBox.SelectedIndex = amount - 1;
                this.amountOfDecks = amount;
            }
        }

        //method for selecting default amount of jokers in game while opening window
        private void SetDefaultAmountOfJokers(int amount)
        {
            if (amount < 0 || amount > 3)
            {
                throw new ArgumentOutOfRangeException("Amount of jokers should be between to 0 and 3; but not to: " + amount.ToString());
            }
            else
            {
                AmountOfJokersComboBox.SelectedIndex = amount;
                this.amountOfJokers = amount;
            }
        }

        //method for selecting default amount of cards given to players at the game start
        private void SetDefaultAmountOfCards(int amount)
        {
            if (amount < 2 || amount > 10)
            {
                throw new ArgumentOutOfRangeException("Amount of start cardsshould be between to 2 and 10; but not to: " + amount.ToString());
            }
            else
            {
                AmountOfStartCardsComboBox.SelectedIndex = amount - 2;
                this.amountOfStartCards = amount;
            }
        }

        #endregion

        #region Selection of ComboBoxes changed

        //AmountOfPlayersComboBox - changed the selection
        private void AmountOfPlayersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            amountOfPlayers = AmountOfPlayersComboBox.SelectedIndex + 2;
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Current amount of players in new game changed to: " + amountOfPlayers.ToString());
        }

        //AmountOfDecksComboBox - changed the selection
        private void AmountOfDecksComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            amountOfDecks = AmountOfDecksComboBox.SelectedIndex + 1;
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Current amount of decks in new game changed to: " + amountOfDecks.ToString());
        }

        //AmountOfJokersComboBox - changed the selection
        private void AmountOfJokersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            amountOfJokers = AmountOfJokersComboBox.SelectedIndex;
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Current amount of jokers in new game changed to: " + amountOfJokers.ToString());
        }

        //AmountOfStartCardsComboBox - changed the selection
        private void AmountOfStartCardsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            amountOfStartCards = AmountOfStartCardsComboBox.SelectedIndex + 2;
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Current amount of cards at the new game start changed to: " + amountOfStartCards.ToString());
        }

        #endregion

        #region Clicking the confirm button (OK)

        //clicking the configm button
        private void ConformRoomCreationButton_Click(object sender, RoutedEventArgs e)
        {
            bool correct = CheckPasswordsCorrectness();
            if (!correct)
            {
                passwordCorrect = false;
                IncorrectPasswords.Visibility = Visibility.Visible;
            }
            else
            {
                passwordCorrect = true;
                IncorrectPasswords.Visibility = Visibility.Collapsed;

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Amount of players in new game (while clising window) is set to " + amountOfPlayers.ToString());

                OnNewRoomDataChanged(password, amountOfPlayers, amountOfDecks, amountOfJokers, amountOfStartCards);
                this.Close();
            }
        }

        //check correctness of passwords
        private bool CheckPasswordsCorrectness()
        {
            if (SecondPasswordBox.Password.Length > 7 || FirstPasswordBox.Password.Length > 7)
            {
                if (FirstPasswordBox.Password == SecondPasswordBox.Password)
                {
                    password = FirstPasswordBox.Password;
                    return true;
                }
                else
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info("Passwords in start new game window are not equal.");

                    MessageBox.Show("Wprowadzone hasła nie są takie same.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);

                    password = null;
                    return false;
                }
            }
            else
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Passwords in start new game window are not enough long. The should Have at least 8 characters.");

                MessageBox.Show("Hasło musi mieć co najmniej 8 znaków długości.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);

                return false;
            }
        }

        #endregion

        #region Event definition - passing data while clising

        //event for passing data of nev event to main window
        public delegate void NewRoomDataEventHandler(object sender, NewRoomDataEventArgs e);

        public event NewRoomDataEventHandler NewRoomDataChanged;

        protected virtual void OnNewRoomDataChanged(string password, int amountOfPlayers, int amountOfDecks, int amountOfJokers,
            int amountOfCards)
        {
            NewRoomDataChanged?.Invoke(null, new NewRoomDataEventArgs { Password = password, AmountOfPlayers = amountOfPlayers,
            AmountOfDecks = amountOfDecks, AmountOfJokers = amountOfJokers, AmountOfStartCards = amountOfCards,
            });
        }

        #endregion
    }

    #region Class holding data to pass it to main window

    public class NewRoomDataEventArgs : EventArgs
    {
        public string Password { get; set; }
        public int AmountOfPlayers { get; set; }
        public int AmountOfDecks { get; set; }
        public int AmountOfJokers { get; set; }
        public int AmountOfStartCards { get; set; }
    }

    #endregion
}
