using System.Windows.Media;
using System.ComponentModel;

namespace MakaoGraphicsRepresentation
{
    //class for storing data of players from room
    public class ListViewPlayersList : INotifyPropertyChanged
    {
        #region Fields

        //private data
        private ImageSource imageSource;
        private string playerName;
        private int amountOfPlayedGames;
        private int amountOfPlayedAndWonGames;
        private bool thisPlayer;
        private int playerNumber;
        private SolidColorBrush backgroundColor;
        private bool readyToPlay;
        private readonly string playerID;

        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor

        //condtruvtor
        public ListViewPlayersList(ImageSource imageSource, string playerName, int amountOfPlayedGames, int amountOfPlayedAndWonGames,
            bool thisPlayer, int playerNumber, SolidColorBrush backgroundColor, bool readyToPlay, string playerID)
        {
            this.imageSource = imageSource;
            this.playerName = playerName;
            this.amountOfPlayedGames = amountOfPlayedGames;
            this.amountOfPlayedAndWonGames = amountOfPlayedAndWonGames;
            this.thisPlayer = thisPlayer;
            this.playerNumber = playerNumber;
            this.backgroundColor = backgroundColor;
            this.readyToPlay = readyToPlay;
            this.playerID = playerID;
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
            get { return amountOfPlayedAndWonGames; }
            set
            {
                amountOfPlayedAndWonGames = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("AmountOfPlayedAndWonGames");
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

        public bool ThisPlayer
        {
            get { return thisPlayer; }
            set
            {
                thisPlayer = value;
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

        public SolidColorBrush BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("BackgroundColor");
            }
        }

        public bool ReadyToPlay
        {
            get { return readyToPlay; }
            set
            {
                readyToPlay = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("ReadyToPlay");
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

    //class for string data of endpoint hostimg
    //Makao game host service
    public class ListViewGameHostList
    {
        public string HostName { get; set; }
        public int AmountOfPlayersInRoom { get; set; }
        public int AmountOfPlayers { get; set; }
        public string Endpoint { get; set; }
    }
}
