using MakaoGameClientService.DataTransferObjects;
using MakaoGameClientService.ServiceImplementations;
using MakaoGameHostService.DataTransferObjects;
using MakaoGameHostService.Messages;
using MakaoGameHostService.ServiceContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MakaoGraphicsRepresentation.Pages
{
    /// <summary>
    /// Interaction logic for PlayersListPage.xaml
    /// </summary>
    public partial class PlayersListPage : Page
    {
        #region Fields and properties

        //actual list of players that will join new game
        private List<ListViewPlayersList> playersList;
        public List<ListViewPlayersList> PlayersList{ get { return playersList; } }

        //Synchronization context
        private SynchronizationContext SynchCont;

        //channel factory for retrieving data from hosts
        private ChannelFactory<IMakaoGameHostService> factory;

        //flag that indicates if player can join the game (client service started)
        private bool canJoinTheGame;
        public bool CanJoinTheGame { get { return canJoinTheGame; } }

        #endregion

        #region Constructor

        //constructor - starting new game as host player
        public PlayersListPage(string newRoomPassword)
        {
            //execute common stuff
            ConstructorsCommonOperations();

            //starting the Windows Service
            Task task = new Task(() => CreateNewGameRoom(5000, 10000, "MakaoGameHostService", newRoomPassword));
            task.Start();
        }

        //constructor - joining the room
        public PlayersListPage()
        {
            //execute common stuff
            ConstructorsCommonOperations();

            //get data of current users (players in the room)
            PlayersLIstActualizationAction();
        }

        //method that executes the same stuff in case of all constructors
        private void ConstructorsCommonOperations()
        {
            InitializeComponent();

            //enable possibility of leaving the room
            LeaveTheRoomButton.IsEnabled = true;

            //Synchronization context
            SynchCont = SynchronizationContext.Current;

            //initialize players list with empty list
            playersList = new List<ListViewPlayersList>();

            //check if client service is running
            CheckClientService();

            DataPlaceholder.RefreshCurrentPlayersList += DataPlaceholder_RefresgCurrentPlayersList;
            DataPlaceholder.PlayersReadinesChanged += DataPlaceholder_PlayersReadinesChanged;
        }

        #endregion

        #region Checking if Game client service is running

        //method wich starts the algorithm in another task
        private void CheckClientService()
        {
            MakaoGameClientServiceAlivenessChecker Checker = new MakaoGameClientServiceAlivenessChecker();
            Checker.ResponseReceived += AlivenessResponseHandler;
        }

        //Response received event handler
        private void AlivenessResponseHandler(object sender, ServiceRunningEventArgs e)
        {
            canJoinTheGame = e.IsRunning;
            if (!canJoinTheGame)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Internal Engine Client service did not start.");

                MessageBox.Show("Wewnętrzny klient gry nie uruchomił się!\nGra będzie niemożliwa.", "Błąd",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Controll of WS - Makao Game Host Service

        //method for starting the service
        private void CreateNewGameRoom(int localTimeout, int timeout, string serviceName, string password)
        {
            EngineHostHandler.EngineHostHandlerConstructor(localTimeout, timeout, serviceName, password, true);
            bool serviceStarted = EngineHostHandler.StartNewService();

            if (!serviceStarted)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Makao Engine Host Service did not sratt.");

                MessageBox.Show("Nie udało się uruchomić usługi: " + serviceName,
                    "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                bool setupDataSavedInHost = EngineHostHandler.SetGameSetup();
                //if setup succesfully written to host
                if (setupDataSavedInHost)
                {
                    //send request to WS to add host player to the room
                    EngineHostHandler.AddHostPlayerToRoom();
                }
                else
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Error("Saving game setup data to host failed");

                    MessageBox.Show("Nie udało się utworzyć gry z poprawnymi danymi.",
                    "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        //method that provides names of avatars of players
        public static ImageSource GetAvatarFullName(int playerNumber)
        {
            //get full path to the avater of player
            string name;
            if (playerNumber < 10) name = "0" + playerNumber.ToString();
            else name = playerNumber.ToString();
            string avatarName = MainWindow.AvatarsTempLocation + name + @".png";

            bool done = false;
            ImageSource source = null; ;
            while (!done)
            {
                try
                {
                    source = GetImageSourceAsFilestream(avatarName);
                    done = true;
                }
                catch
                {
                    done = false;
                }
            }            

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Getting full avatar name from temp location: " + avatarName);

            return source;
        }

        //method for obtaining image source for listView as BitmapFrame
        private static ImageSource GetImageSourceAsFilestream(string FileName)
        {
            using (var fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
            {
                return BitmapFrame.Create(fs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }

        #endregion

        #region Actualizing player readiness to play

        //event fired when got proper data from host
        private void DataPlaceholder_PlayersReadinesChanged(object sender, PlayersReadinessToPlayEventArgs e)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Actualizing players readiness to play game data received from host.");

            //actualizing data in list variable
            foreach (ListViewPlayersList item in playersList)
            {
                item.ReadyToPlay = UpdateReadinessDataInList(item, e).ReadyToPlay;
            }

            for (int i = 0; i < CurrentPlayersList.Items.Count; i++)
            {
                ActualizedDataOfPlayersReadiness newData = e.ReadinessData.ReadinessDataList.Single
                    (p => p.PlayerNumber == (CurrentPlayersList.Items[i] as ListViewPlayersList).PlayerNumber);
                (CurrentPlayersList.Items[i] as ListViewPlayersList).ReadyToPlay = newData.IsReadyToPlay;
            }
        }

        //common method for updating data in internal list and in list view table (graphical reprresentation)
        private ListViewPlayersList UpdateReadinessDataInList(ListViewPlayersList item,PlayersReadinessToPlayEventArgs e)
        {
            try
            {
                ActualizedDataOfPlayersReadiness playerData = e.ReadinessData.ReadinessDataList.Single(p => p.PlayerNumber == item.PlayerNumber);
                item.ReadyToPlay = playerData.IsReadyToPlay;
                return item;
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Couldn't find data of player with number: " + item.PlayerNumber + " in data received from Host. Exception" + ex.Message);
                return item;
            }
        }

        #endregion

        #region Getting actual players list from host

        //event handling an event which is fired when new player joins the room in host
        private void DataPlaceholder_RefresgCurrentPlayersList(object sender, EventArgs e)
        {
            SynchCont.Post(_ => PlayersLIstActualizationAction(), null);
        }

        private void PlayersLIstActualizationAction()
        {
            ObtainCurrentUserDataFromHost();
        }

        //updating listview with current players in the room
        private void ObtainCurrentUserDataFromHost()
        {
            Task.Run(() => ObtainCurrentRoomPlayersDataFromhost(MainWindow.MakaoGameHostEndpoint));
        }

        //method for implementation of obtaining players data from Host
        private void ObtainCurrentRoomPlayersDataFromhost(Uri endpoint)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Started obtaining players in room data from host.");

            StartNewFactoryChannel(endpoint);
            CurrentPlayersListDataResponse response = GetCurrentPlayersDataFromHost();
            List<int> numbersList = GetPlayersNumbersList(response);
            DownloadPLayersAvatars(numbersList);
            SynchCont.Post(_ => DisplayAllUsersDataInListView(response), null);

            //event for enabling the menus in main window
            if(!response.RoomIsFull)
            {
                OnChangeAvailabilityOfMenus();
                SynchCont.Post(_ => LeaveTheRoomButton.IsEnabled = true, null);
            }
        }

        //starting new factory channel (depends on endpoint addess, which is not constant)
        private void StartNewFactoryChannel(Uri endpoint)
        {
            factory = new ChannelFactory<IMakaoGameHostService>(new BasicHttpBinding(),
                new EndpointAddress(endpoint));
        }

        //method that gets data from host
        private CurrentPlayersListDataResponse GetCurrentPlayersDataFromHost()
        {
            IMakaoGameHostService proxy = factory.CreateChannel();
            CurrentPlayersListDataResponse response = proxy.GetCurrentPlayersInTheRoomData();
            return response;
        }

        //provide list of playersNumbers
        private List<int> GetPlayersNumbersList(CurrentPlayersListDataResponse response)
        {
            List<int> returnList = new List<int>();
            foreach (var item in response.CurrentPLayersData)
            {
                int newNumber = item.PlayerNumber;
                returnList.Add(newNumber);
            }
            return returnList;
        }

        //method thad downloads all images, for all players and saves as files
        private void DownloadPLayersAvatars(List<int> playersNumbers)
        {
            IMakaoGameHostService proxy = factory.CreateChannel();
            foreach (int item in playersNumbers)
            {
                Stream responseStream = proxy.DownloadAvatarImage(item);
                EngineHostHandler.SaveAnImage(responseStream, item);
            }
        }

        //displaying data aboout all users in the list view
        private void DisplayAllUsersDataInListView(CurrentPlayersListDataResponse response)
        {
            CurrentPlayersList.Items.Clear();
            playersList.Clear();

            foreach (PlayersInRoomData item in response.CurrentPLayersData)
            {
                AddPlayerToList(GetAvatarFullName(item.PlayerNumber), item.PlayerName, item.PlayedGames, item.PlayedAndWonGames,
                    CheckIfPassedPlayerDataIsThisPlayersData(item), item.PlayerNumber, item.ReadyToPlay, item.PlayerID);
            }
            CheckIfPlayersAmountIsProper();
        }

        //mathod that check if player is this player
        private bool CheckIfPassedPlayerDataIsThisPlayersData(PlayersInRoomData data)
        {
            if (MainWindow.PlayerID == data.PlayerID) return true;
            else return false;
        }

        #endregion

        #region Adding one player to the list

        //method for adding one new player to list
        public void AddPlayerToList (ImageSource image, string playerName, int playedGames, int playedAndWonGames,
            bool thisPlayer, int playerNumber, bool readytoPlay, string playerID)
        {
            //definig bacgrount of ListViewItem - if whis player, make it yellow
            Color color = Colors.White;
            if (thisPlayer) color = Colors.Yellow;
            SolidColorBrush newColor = new SolidColorBrush(color);

            //check if player with such a ID is already shown in the list
            List<ListViewPlayersList> players = playersList.FindAll(p => p.PlayerID == playerID);

            //only if there is no such a player in the list with ID same as passed to this method
            if (players.Count == 0)
            {
                //creating new element of playersList
                playersList.Add(new ListViewPlayersList(image, playerName, playedGames, playedAndWonGames, thisPlayer, playerNumber, newColor, readytoPlay, playerID));

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Added new player to list of players in the room - name of player: " + playerName);

                //adding new player to ListView
                CurrentPlayersList.Items.Add(playersList.Last());
            }   

            //check amount of player - if it is equal to planned amount
            CheckIfPlayersAmountIsProper();
        }

        //method for changing visibility of start game button and textblock
        //info about waiting for more players
        private void CheckIfPlayersAmountIsProper()
        {
            LeaveTheRoomButton.Visibility = Visibility.Visible;

            if (MainWindow.TotalAMountOfPlayerObtainedFromHost == playersList.Count)
            {
                WaitingForPlayerTextBlock.Visibility = Visibility.Collapsed;
                ReadyToPlayButton.Visibility = Visibility.Visible;
            }
            else
            {
                WaitingForPlayerTextBlock.Visibility = Visibility.Visible;
                ReadyToPlayButton.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Refreshing and deleting items from the list

        //method for refreshing content of current players list view control
        public void RefeshTheList()
        {
            CurrentPlayersList.Items.Clear();

            foreach (var item in playersList)
            {
                CurrentPlayersList.Items.Add(item);
            }
        }

        //method for clearing data stored in this class (list view)
        public void DeleteListContent()
        {
            CurrentPlayersList.Items.Clear();
            playersList.Clear();
        }

        #endregion

        #region ListView selected index reseting

        //method that doesn't allow to select any user in listview
        private void CurrentPlayersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentPlayersList.SelectedIndex = -1;
        }

        #endregion

        #region Readiness to start the game button handling
        
        //event handler for clicking button that indicates readiness of player to start
        //the game. This causes, that leaving the room is no more possible
        private void ReadyToPlayButton_Click(object sender, RoutedEventArgs e)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Sending info to host about readiness to start the game.");

            //disable possibility of leaving the room
            LeaveTheRoomButton.IsEnabled = false;

            //fire event
            OnReadinessToPlayConfirmed();

            //start algorithm method in new task
            Task.Run(() => StartGameReadinessHandlingMethod(MainWindow.MakaoGameHostEndpoint));
        }

        //algorithm method
        private void StartGameReadinessHandlingMethod(Uri endpoint)
        {
            //create factory channel
            StartNewFactoryChannel(endpoint);

            //creating the request
            PlayerIsReadyToPlayGameRequest request = new PlayerIsReadyToPlayGameRequest()
            {
                IsReadyToPlay = true,
                PlayerID = MainWindow.PlayerID,
                PlayerNumber = MainWindow.PlayerNumber,
            };

            //building proxy and obtaining the response
            IMakaoGameHostService proxy = factory.CreateChannel();
            bool response = proxy.ChangePlayerReadinessToPlay(request);

            //if response is true, deactivate the button
            if (response)
            {
                SynchCont.Post(_ => ReadyToPlayButton.IsEnabled = false, null);
            }
        }

        #endregion

        #region Leaving the room

        //event handler  - leaving the room before confirmation of joining the game
        private void LeaveTheRoomButton_Click(object sender, RoutedEventArgs e)
        {
            OnRoomLeftByUser();
        }

        //also leave the room when tha page is unloading
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if(!MainWindow.IsHostPlayer) OnRoomLeftByUser();
        }

        #endregion

        #region Events: confirm play in game; leave the room, actualize enabling of menus in main window

        //readiness to play event
        public delegate void ConfirmReadinessToPlayEventhandler(object sender, EventArgs e);
        public event ConfirmReadinessToPlayEventhandler ReadinessToPlayConfirmed;
        protected virtual void OnReadinessToPlayConfirmed()
        {
            ReadinessToPlayConfirmed?.Invoke(this, new EventArgs());
        }

        //leaving the room event
        public delegate void LeavingTheRoomPlayEventhandler(object sender, EventArgs e);
        public event LeavingTheRoomPlayEventhandler RoomLeftByUser;
        protected virtual void OnRoomLeftByUser()
        {
            RoomLeftByUser?.Invoke(this, new EventArgs());
        }

        //actualize manus in main windiw (enabling)
        public delegate void ActualizeMenuEnableEventhandler(object sender, EventArgs e);
        public event ActualizeMenuEnableEventhandler ChangeAvailabilityOfMenus;
        protected virtual void OnChangeAvailabilityOfMenus()
        {
            ChangeAvailabilityOfMenus?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}
