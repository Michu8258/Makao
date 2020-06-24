using MakaoGameClientService.ServiceImplementations;
using MakaoGameHostService.Messages;
using MakaoGameHostService.ServiceContracts;
using MakaoGraphicsRepresentation.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MakaoGraphicsRepresentation.Pages
{
    /// <summary>
    /// Interaction logic for EngineHostsListPage.xaml
    /// </summary>
    public partial class EngineHostsListPage : Page
    {
        #region Fields and properties

        //list of data showed in the table (founded wndpoints)
        private readonly List<ListViewGameHostList> listViewData;
        public List<ListViewGameHostList> ListViewData { get { return listViewData; } }

        //list of founded endpoints
        private readonly List<Uri> endpointsList;
        public List<Uri> EndpointsList { get { return endpointsList; } }

        //channel factory for retrieving data from hosts
        private ChannelFactory<IMakaoGameHostService> factory;

        //Synchronization context
        private readonly SynchronizationContext SynchCont;

        //flag that indicates if player can join the game (client service started)
        private bool canJoinTheGame;
        public bool CanJoinTheGame { get { return canJoinTheGame; } }

        //password input window
        private PasswordInputWindow passwordWindow;

        //mainWindow instance
        private readonly MainWindow mainWindowInstance;

        //posobility of connection to host - impossible while endpoints
        //searching is still not finished
        private bool possibilityOfConnection;

        //selected host by double-click of listView item
        private ListViewGameHostList currentHost;

        #endregion

        #region Constructor

        public EngineHostsListPage(MainWindow mainWindow)
        {
            InitializeComponent();

            //assigning main window instance
            mainWindowInstance = mainWindow;

            //Synchronization context
            SynchCont = SynchronizationContext.Current;

            //inicialize Lists - keeping the data
            listViewData = new List<ListViewGameHostList>();
            endpointsList = new List<Uri>();

            //subscribing Makao game host endpoints obtainer class events
            MakaoGameHostServiceEndpointObtainer.FoundedNewEndpoint += FoundedNewEndpointChanged;
            MakaoGameHostServiceEndpointObtainer.SearchingForEndpointsFinished += FinishedSearchingForEndpoints;

            //start searching for endpoints of Host service
            StartSearchingForHostEndpoints();

            //check if client service is running
            CheckClientService();

            //hide button for adding new endpoints manually
            AdEndpointManuallyButton.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Searchin in progress text and refresh buttin visibility

        //method for handling visibility of text that informs
        //about fact that searching is in progress
        private void ChangeVisibilityOfSearchInProgressText(bool searchInProgress)
        {
            if (searchInProgress)
            {
                NoHostsFoundInfo.Visibility = Visibility.Collapsed;
                SearchingForEndpointsInProgressTextBlock.Visibility = Visibility.Visible;
                RefreshEndpointsListButton.IsEnabled = false;
            }
            else
            {
                SearchingForEndpointsInProgressTextBlock.Visibility = Visibility.Collapsed;
                RefreshEndpointsListButton.IsEnabled = true;
            }
        }

        #endregion

        #region Refresh button action

        //method for refreshing data in hosts List
        private void RefreshEndpointsListButton_Click(object sender, RoutedEventArgs e)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Refreshing the list of endpoints that host Makao Engine Service.");

            AdEndpointManuallyButton.Visibility = Visibility.Collapsed;

            listViewData.Clear();
            endpointsList.Clear();
            FoundedGameHostEndpointList.Items.Clear();
            StartSearchingForHostEndpoints();
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

        #region Service discovery Handling

        //method for fire up searching for endpoints
        private void StartSearchingForHostEndpoints()
        {
            ChangeVisibilityOfSearchInProgressText(true);
            ManagePossibilityOfConnectionToHost(false);
            MakaoGameHostServiceEndpointObtainer.StartSearchingHostEndpointWhileNotBeingHost();
        }

        //event for passing in new founded endpoint
        private void FoundedNewEndpointChanged(object sender, EndpointEventArgs e)
        {
            endpointsList.Add(e.EndpointsList[0]);
            StartObtainingData(e.EndpointsList[0]);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Found new endpoint: " + e.EndpointsList[0]);
        }

        //event for finished process of searching for makao host service endpoints
        private void FinishedSearchingForEndpoints(object sender, EndpointEventArgs e)
        {
            if (endpointsList.Count == 0)
            {
                NoHostsFoundInfo.Visibility = Visibility.Visible;
                AdEndpointManuallyButton.Visibility = Visibility.Visible;
            }
            else
            {
                AdEndpointManuallyButton.Visibility = Visibility.Collapsed;
            }
            ChangeVisibilityOfSearchInProgressText(false);
            ManagePossibilityOfConnectionToHost(true);
        }

        //method that changed allowing for connection to host
        private void ManagePossibilityOfConnectionToHost(bool available)
        {
            possibilityOfConnection = available;
        }

        #endregion

        #region Obtaining detailed data about host

        //start obtaining task as new task
        private void StartObtainingData(Uri endpoint)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Start obtaining data of just founded new endpoint address.");

            Task.Run(() => ObtainData(endpoint));
        }

        //method with obtaining data from host algorithm
        private void ObtainData(Uri endpoint)
        {
            StartNewFactoryChannel(endpoint);
            GetRoomDetailsWhenJoiningRoomResponse response = ObtainHostDetailedData();
            SynchCont.Post(_ => DisplayObtainedDataInListView(response, endpoint), null);
        }

        //starting new factory channel (depends on endpoint addess, which is not constant)
        private void StartNewFactoryChannel(Uri endpoint)
        {
            factory = new ChannelFactory<IMakaoGameHostService>(new BasicHttpBinding(),
                new EndpointAddress(endpoint));
        }

        //retrieving data from host
        private GetRoomDetailsWhenJoiningRoomResponse ObtainHostDetailedData()
        {
            IMakaoGameHostService proxy = factory.CreateChannel();
            GetRoomDetailsWhenJoiningRoomResponse response = proxy.GetHostRoomDetails();
            return response;
        }

        //display data in the listViev
        private void DisplayObtainedDataInListView(GetRoomDetailsWhenJoiningRoomResponse response, Uri endpoint)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Obtained data from host: Name - " + response.HostName + ", Room capacity - " +
                response.RoomCapacity + ", Amount of players already in room - " + response.AmountOfPlayersInRoom +
                ", Endpoint - " + endpoint);

            AddNewEndpointItemToListView(response.HostName, response.RoomCapacity, response.AmountOfPlayersInRoom,
                endpoint);
        }

        #endregion

        #region Joining to room - password input window

        //event handler - double clicck of the list
        private void FoundedGameHostEndpointList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if some element is selected
            if (FoundedGameHostEndpointList.SelectedIndex > -1)
            {
                /*check if:
                 * 1. Client serwice is running,
                 * 2. players amount is more than 0,
                 * 3. capacity of room is more than 0,
                 * 4. Searching for endpoints finished
                 */
                if (canJoinTheGame == true && (FoundedGameHostEndpointList.SelectedItem as ListViewGameHostList).AmountOfPlayers > 0 &&
                    (FoundedGameHostEndpointList.SelectedItem as ListViewGameHostList).AmountOfPlayersInRoom > 0 &&
                    possibilityOfConnection == true)
                {
                    currentHost = (FoundedGameHostEndpointList.SelectedItem as ListViewGameHostList);

                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info("Assigning the endpoint that will be the host ebdpoint: " + currentHost.Endpoint);

                    //assign host endpoint in main window
                    MainWindow.AssignHostEndpoint(new Uri(currentHost.Endpoint));

                    passwordWindow = new PasswordInputWindow();
                    passwordWindow.PasswordInputFinished += PasswordInputFinished;
                    passwordWindow.Owner = mainWindowInstance;
                    passwordWindow.ShowDialog();
                }
            }
        }

        //catch password when window is closing
        private void PasswordInputFinished(object sender, PasswordInputWindowClosingArgs e)
        {
            StartAddiingPlayerToTheRoom(e.Password);
        }

        #endregion

        #region Connecting to host (joining the room)

        //start the process in new task
        private void StartAddiingPlayerToTheRoom(string password)
        {
            Task.Run(() => JoinTheGame(MainWindow.MakaoGameHostEndpoint,
                MainWindow.ThisClientEndpoint, password));
        }

        //task algorithm
        private void JoinTheGame(Uri hostEndpoint, Uri clientEndpoint, string password)
        {
            StartNewFactoryChannel(hostEndpoint);
            AddNewPlayerResponse response = ObtainJoiningResult(clientEndpoint, password);
            ProcessJoiningTheRoomResponse(response);
        }

        //sending the request to the host
        private AddNewPlayerResponse ObtainJoiningResult(Uri clientEndpoint, string password)
        {
            AddNewPlayerRequest request = new AddNewPlayerRequest()
            {
                PlayerName = MainWindow.SavedData.CurrentPlayerName,
                PlayedGames = MainWindow.SavedData.SavedAmountOfPlayedGames,
                PlayedAndWonGames = MainWindow.SavedData.SavedAmountOfPlayedAndWonGames,
                PlayerEndpoint = clientEndpoint,
                Password = password,
            };

            IMakaoGameHostService proxy = factory.CreateChannel();

            try
            {
                AddNewPlayerResponse response = proxy.AddNotHostPlayerToTheRoom(request);
                return response;
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while trying to send password to host: {ex.Message}");
                return null;
            }
        }

        //processing response obtained from host
        private void ProcessJoiningTheRoomResponse(AddNewPlayerResponse response)
        {
            if (response != null)
            {
                //not added to game
                if (!response.AddedToGame)
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info("The endpoint didn't joi the player to the room");

                    //if host did not add player to the room, return to main thread and
                    //show message
                    SynchCont.Post(_ => NotAddedToTheRoomMessageBoxShow(), null);
                }
                else
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info("Player successfully joined the room. Player Number: " + response.PlayerNumber.ToString());

                    //assign player daya
                    MainWindow.IsHostPlayer = response.IsHostPlayer;
                    MainWindow.PlayerID = response.PlayerID;
                    MainWindow.PlayerNumber = response.PlayerNumber;
                    MainWindow.TotalAMountOfPlayerObtainedFromHost = response.TotalAMountOfPlayers;

                    //assign app type
                    MainWindow.ChangeGameStatus(response.IsHostPlayer);

                    //send an avatar image to the host
                    SendAvatarImageToHost(response.PlayerNumber);
                }
            }
        }

        //method that causes show messagebox with information that player was not added
        //to the room
        private void NotAddedToTheRoomMessageBoxShow()
        {
            MessageBox.Show("Host hry nie dołączył Cię do pokoju.\n" + 
                "Możliwe, że podałeś nieprawidłowe hasło,\nlub w pokoju nie ma już miejsc.",
                "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //method for sending an avatar image to the host
        private void SendAvatarImageToHost(int playerNumber)
        {
            //sending data with image
            MemoryStream avatarMemoryStream = new MemoryStream();
            byte[] bytes = File.ReadAllBytes(MainWindow.SavedData.CurrentAvatarPicture);
            avatarMemoryStream.Write(bytes, 0, bytes.Length);
            avatarMemoryStream.Position = 0;

            IMakaoGameHostService proxy = factory.CreateChannel();

            //actual sending
            bool success = EngineHostHandler.SendAvatarToHost(avatarMemoryStream, playerNumber, proxy);

            if(!success)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Player's avatar not assigned in the host implementation");

                MessageBox.Show("Twojego avatara nie udało się przesłać do hosta.",
                    "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Player's avatar successfully assigned in the host implementation");

                //if successfully joined the game, fire the event - it sholud be
                //served in the main window.

                SynchCont.Post(_ => OnSuccessfullyJoinedToTheRoom(), null);
            }
        }

        #endregion

        #region Adding new endpoint to the list

        //Add new endpoint with name of host and other data to tha list view
        private void AddNewEndpointItemToListView(string hostName, int amountOfPlayers, int amountOfPlayersInRoom, Uri endpoint)
        {
            listViewData.Add(new ListViewGameHostList() {HostName = hostName, AmountOfPlayers = amountOfPlayers,
                AmountOfPlayersInRoom = amountOfPlayersInRoom, Endpoint = endpoint.ToString() });
            FoundedGameHostEndpointList.Items.Add(listViewData.Last());
        }

        #endregion

        #region Joined to the room event

        public delegate void JoinedToTheRoomEventHandler(object sender, EventArgs e);
        public event JoinedToTheRoomEventHandler SuccessfullyJoinedToTheRoom;
        protected virtual void OnSuccessfullyJoinedToTheRoom()
        {
            SuccessfullyJoinedToTheRoom?.Invoke(this, new EventArgs ());
        }

        #endregion

        private void AdEndpointManuallyButton_Click(object sender, RoutedEventArgs e)
        {
            AddEndpointManuallyWindow endpointWindow = new AddEndpointManuallyWindow()
            {
                Owner = mainWindowInstance,
            };

            endpointWindow.ManualHostInputed += EndpointWindow_ManualHostInputed;

            endpointWindow.ShowDialog();
            MemoryManagement.FlushMemory();
        }

        private void EndpointWindow_ManualHostInputed(object sender, IPAddresEventArgs e)
        {
            AddNewEndpointItemToListView("Host", 2, 1, new Uri($"http://{e.IPAddress}:9500/MakaoGameHostWindowsService"));
            ManagePossibilityOfConnectionToHost(true);
            NoHostsFoundInfo.Visibility = Visibility.Collapsed;
        }
    }
}
