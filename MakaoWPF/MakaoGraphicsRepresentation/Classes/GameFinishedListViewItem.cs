using System.ComponentModel;
using System.Windows.Media;

namespace MakaoGraphicsRepresentation
{
    class GameFinishedListViewItem : INotifyPropertyChanged
    {
        #region Private fields

        private ImageSource imageSource;
        private string playerName;
        private int amountOfPlayedGames;
        private int amountOfplayedAndWonGames;
        private int winnerPlayerNumber;
        private int playerNumber;
        private readonly string playerID;
        private int playerPosition;

        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor

        //constructor
        public GameFinishedListViewItem(ImageSource imageSource, string playerName, int amountOfPlayedGames, int amountOfplayedAndWonGames,
            int winnerPlayerNumber, int playerNumber, string playerID, int playerPosition)
        {
            this.imageSource = imageSource;
            this.playerName = playerName;
            this.amountOfPlayedGames = amountOfPlayedGames;
            this.amountOfplayedAndWonGames = amountOfplayedAndWonGames;
            this.winnerPlayerNumber = winnerPlayerNumber;
            this.playerNumber = playerNumber;
            this.playerID = playerID;
            this.playerPosition = playerPosition;
        }

        #endregion

        #region Properties

        public string PlayerID
        {
            get { return playerID; }
        }

        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("ImageSource");
            }
        }

        public string PlayerName
        {
            get { return playerName; }
            set
            {
                playerName = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("PlayerName");
            }
        }

        public int AmountOfPlayedAndWonGames
        {
            get { return amountOfplayedAndWonGames; }
            set
            {
                amountOfplayedAndWonGames = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("amountOfplayedAndWonGames");
            }
        }

        public int AmountOfPlayedGames
        {
            get { return amountOfPlayedGames; }
            set
            {
                amountOfPlayedGames = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("AmountOfPlayedGames");
            }
        }

        public int WinnerPlayerNumber
        {
            get { return winnerPlayerNumber; }
            set
            {
                winnerPlayerNumber = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("ThisPlayer");
            }
        }

        public int PlayerNumber
        {
            get { return playerNumber; }
            set
            {
                playerNumber = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("PlayerNumber");
            }
        }

        public int PlayerPosition
        {
            get { return playerPosition; }
            set
            {
                playerPosition = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("playerPosition");
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation - event firing

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
