using CardGraphicsLibraryHandler;
using CardsRepresentation;
using MakaoGameClientService.DataTransferObjects;
using MakaoGameClientService.Messages;
using MakaoGameClientService.ServiceImplementations;
using MakaoGameHostService.DataTransferObjects;
using MakaoGameHostService.Messages;
using MakaoGameHostService.ServiceContracts;
using MakaoGraphicsRepresentation.MainWindowData;
using MakaoGraphicsRepresentation.Pages;
using MakaoGraphicsRepresentation.UserAdministrationWindows;
using MakaoGraphicsRepresentation.Windows;
using MakaoInterfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MakaoGraphicsRepresentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private fields

        //varaible for storing saved data
        private static SavedDataClass SavedDataPlaceholder;

        //storage directory of temparary game avatars
        private static string avatarTempLocation;

        //variables necessary for sreating new game room
        private string newRoomPassword;

        //string ID of current player
        private static string playerID;

        //variable for storig currently showed page
        private PlayersListPage CurrentPlayersListPage;
        private EngineHostsListPage CurrentEngineHostsListPage;

        //Synchronization context
        private readonly SynchronizationContext SynchCont;

        //nlog file location new log file for every startup of the app
        private static readonly string logLocationString = System.Reflection.Assembly.GetExecutingAssembly().
            Location.Substring(0, System.Reflection.Assembly.GetExecutingAssembly().Location.Length - 31) + @"Logs\";

        //Makao game client service endpoint
        private static Uri thisClientEndpoint;

        //Makao game host service endpoint
        private static Uri makaoGameHostEndpoint;

        //variable for remember which page is opened
        private int openedPageNumber; //1 - PlayersListPage, 2 - EngineHostsListPage

        //timer for stoping the WS makao game host service with delay
        System.Timers.Timer StopServiceTimer;

        //variable of currently logged player ID from database
        private static int playerDBID;

        //rules window
        private RulesWindow.RulesWindow rulesWindow;

        #endregion

        #region Public properties

        //property with location string
        public static string LogLocation { get { return logLocationString; } }
        public static string AvatarsTempLocation { get { return avatarTempLocation; } }
        public static string PlayerID { get { return playerID; } set { playerID = value; } }
        public static int PlayerNumber { get; set; }
        public static bool IsHostPlayer { get; set; }
        public static Uri ThisClientEndpoint { get { return thisClientEndpoint; } }
        public static Uri MakaoGameHostEndpoint { get { return makaoGameHostEndpoint; } }
        public static int TotalAMountOfPlayerObtainedFromHost { get; set; }
        public static SavedDataClass SavedData { get { return SavedDataPlaceholder; } }

        #endregion

        #region Main Window constructor

        //constructor
        public MainWindow()
        {
            InitializeComponent();

            //seting default user data (ID and login)
            playerDBID = 0;

            //inicialize datapleceholders
            SavedDataPlaceholder = new SavedDataClass();

            //variables for logging unhandled exceptions
            AppDomain currentDomain = default;
            currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;

            //Synchronization context
            SynchCont = SynchronizationContext.Current;

            //NLOG configutation methos
            string logsLocation = logLocationString + @"MakaoGameLogs" + DateTime.Now.ToString() + ".txt";
            NLogConfigMethod(logsLocation);

            //setting directory of avatars used in game location
            avatarTempLocation = logLocationString.Substring(0, logLocationString.Length - 5) + @"AvatarsTemp\";

            //reading saved data at the start of application
            ReadDataFromSettings(DefaultUserSettings.GetDefaultSettings());

            //Avatar
            AssignAvatar(SavedDataPlaceholder.CurrentAvatarPicture);

            //assign background
            AssignMainWindowBackgroundGraphics();

            //generate cliend endpoint Uri
            thisClientEndpoint = MakaoGameClientServiceAddresObtainer.GetClientServiceEndpoint();

            //delete host endpoint - assign null
            makaoGameHostEndpoint = null;

            //deleting old logs in new task
            Task deleteOldLogs = new Task(new Action(DeleteOldLogFiles));
            deleteOldLogs.Start();

            //subscription of event - the event is fired, when host user deleted the room
            DataPlaceholder.TheRoomWasDeleted += DataPlaceholder_TheRoomWasDeleted;

            //subscription for an event of opening new window game command from game host service
            DataPlaceholder.NewGameStarted += DataPlaceholder_NewGameStarted;

            //changing status of application (host, client or none)
            ApplicationStatusChanged += MainWindow_ApplicationStatusChanged;

            //start client service
            StartMakaoGameClientService();

            //assign rules window as null
            rulesWindow = null;

            //assign local IP address of endpoint of this devce
            UserIPString.Text = $"IP: {MakaoGameClientServiceAddresObtainer.GetLocalIPAddressString()}, ";
        }

        //method for assigning image as a main window background - at the app start (constructor of main window)
        private void AssignMainWindowBackgroundGraphics()
        {
            BackgroundImageObtainer obtainer = new BackgroundImageObtainer();
            BackgroundTheme.Source = obtainer.GetBackgroundGraphics();
        }

        //method for saving info about application crash exception to log file (when app is crashing)
        private void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Error($"Application crashed: {ex.Message}.");
            logger.Error($"Inner exception: {ex.InnerException}.");
            logger.Error($"Stack trace: {ex.StackTrace}.");
        }

        #endregion

        #region User management

        //reaction for clicking creation of new user option from window menu
        private void MenuCreateNewUser_Click(object sender, RoutedEventArgs e)
        {
            CreateNewUser newUserCreationWindow = new CreateNewUser()
            {
                Owner = this,
            };
            newUserCreationWindow.AdditionSuccedeed += NewUserCreationWindow_AdditionSuccedeed;
            newUserCreationWindow.ShowDialog();
            MemoryManagement.FlushMemory();
        }

        //addition of new player ended successfully
        private void NewUserCreationWindow_AdditionSuccedeed(object sender, CreationOfNewPlayerSuccedeedEventArgs e)
        {
            OpenLogingWIndow();
        }

        //reaction for clicking loging menu item
        private void MenuLogin_Click(object sender, RoutedEventArgs e)
        {
            OpenLogingWIndow();
        }

        //method for opening loging window
        private void OpenLogingWIndow()
        {
            LoginWindow loginWindow = new LoginWindow()
            {
                Owner = this,
            };
            loginWindow.LoginSuccedeed += LoginWindow_LoginSuccedeed;
            loginWindow.ShowDialog();
            MemoryManagement.FlushMemory();
        }

        //event of succedeed login
        private void LoginWindow_LoginSuccedeed(object sender, LoginSuccedeedEventArgs e)
        {
            if (e.Success)
            {
                ReadDataFromSettings(e.Settings);
                playerDBID = e.PlayerID;
                menuLogout.IsEnabled = true;
            }
        }

        //loging out event
        private void MenoLogout_Click(object sender, RoutedEventArgs e)
        {
            Logout();
        }

        #endregion

        #region Application info window

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            AuthorInfoWindow authorWindow = new AuthorInfoWindow()
            {
                Owner = this,
            };
            authorWindow.ShowDialog();
        }

        #endregion

        #region Starting new game window with data from host service

        //event handler for opening game page command from host
        private void DataPlaceholder_NewGameStarted(object sender, OpenNewGameWindowEventArgs e)
        {
            SynchCont.Post(_ => EnableOrDisableMenu(true), null);
            SynchCont.Post(_ => StartNewgameWindow(SavedDataPlaceholder.LocationOfThirdPlayersCards, e.ReceivedData), null);
        }

        //open new game window
        private void StartNewgameWindow(ThirdPlayerLocation location, PersonalizedForSpecificPlayerStartGameDataRequest data)
        {
            Windows.GameWindow gameWindow = new Windows.GameWindow(location, data, SavedDataPlaceholder.CardsBackColor)
            {
                Owner = this,
            };

            gameWindow.WindowWasClosedByUser += GameWindow_WindowWasClosedByUser;
            gameWindow.GameWindowClosedByHost += GameWindow_GameWindowClosedByHost;
            gameWindow.ShowDialog();
            MemoryManagement.FlushMemory();
        }

        //game ended - someone won the game
        private void GameWindow_GameWindowClosedByHost(object sender, GameFinishedEventArgs e)
        {
            //assigning amount of played games data - if player is logged in
            if (playerDBID > 0)
            {
                SavedDataPlaceholder.SavedAmountOfPlayedGames = e.PlayedGames;
                SavedDataPlaceholder.SavedAmountOfPlayedAndWonGames = e.PlayedAndWonGames;
            }

            //reseting main window page
            openedPageNumber = 0;
            MainWindowFrame.Content = null;

            //if the player is host player - stop the makao game service.
            if (IsHostPlayer == true) Task.Run(() => StopTheHostServiceWithDelay(3));
        }

        private void GameWindow_WindowWasClosedByUser(object sender, EventArgs e)
        {
            StartDeletionAlgorithm(LeavingTheRoomWindowType.GameWindow);
        }

        #endregion

        #region Changing application type written in status bar

        //method to change app by is host property value
        public static void ChangeGameStatus(bool host)
        {
            if (host) OnApplicationStatusChanged(AppType.Host);
            else OnApplicationStatusChanged(AppType.Client);
        }

        //method for changing app by enum type
        public static void ChangeGameStatus(AppType appType)
        {
            OnApplicationStatusChanged(appType);
        }

        //changing app status
        private void ChangeStatusOfApplication(AppType appType)
        {
            switch (appType)
            {
                case AppType.Host:
                    MakeAppStatusVisibleOrNot(Visibility.Visible);
                    SynchCont.Post(_ => applicationType.Text = "Hostem", null);
                    break;
                case AppType.Client:
                    MakeAppStatusVisibleOrNot(Visibility.Visible);
                    SynchCont.Post(_ => applicationType.Text = "Klientem", null);
                    break;
                case AppType.None:
                    MakeAppStatusVisibleOrNot(Visibility.Collapsed);
                    SynchCont.Post(_ => applicationType.Text = "-----", null);
                    break;
            }
        }

        //showing or hiding app stasus
        private void MakeAppStatusVisibleOrNot(Visibility vis)
        {
            SynchCont.Post(_ => hostOrClientText.Visibility = vis, null);
            SynchCont.Post(_ => applicationType.Visibility = vis, null);
        }

        //event handler
        private void MainWindow_ApplicationStatusChanged(object sender, ChangeAppStatusEventArgs e)
        {
            ChangeStatusOfApplication(e.NewStatus);
        }

        //event declaration
        private delegate void ChangeApplicationStatusEventHandler(object sender, ChangeAppStatusEventArgs e);
        private static event ChangeApplicationStatusEventHandler ApplicationStatusChanged;
        private static void OnApplicationStatusChanged(AppType newStatus)
        {
            ApplicationStatusChanged?.Invoke(null, new ChangeAppStatusEventArgs { NewStatus = newStatus });
        }

        #endregion

        #region Reaction of deletion of the room

        //when room was deleted by host
        private void DataPlaceholder_TheRoomWasDeleted(object sender, RoomDeletionReasonsEventArgs e)
        {
            //close game window if it is opened
            SynchCont.Post(_ => CloseGameWindow(), null);

            if (e.DeletionReason == DeletionReason.ClosedByHost)
            {
                //not being host
                if (openedPageNumber == 1 && !IsHostPlayer)
                {
                    SynchCont.Post(_ => openedPageNumber = 0, null);
                    SynchCont.Post(_ => MainWindowFrame.Content = null, null);
                    Task.Run(() => RoomDeletionReasonMessageBox(e.DeletionReason));
                }
            }
            else
            {
                SynchCont.Post(_ => openedPageNumber = 0, null);
                SynchCont.Post(_ => MainWindowFrame.Content = null, null);
                SynchCont.Post(_ => EngineHostHandler.Dispose(), null);
                Task.Run(() => RoomDeletionReasonMessageBox(e.DeletionReason));
            }
        }

        //method for closing the game window
        private void CloseGameWindow()
        {
            foreach (Window win in Application.Current.Windows)
            {
                if (win.GetType() == typeof(GameWindow))
                {
                    (win as GameWindow).Close();
                }
            }
        }

        //room was deleted by the host
        private void RoomDeletionReasonMessageBox(DeletionReason reason)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("The room was closed. Reason: " + reason.ToString());

            switch (reason)
            {
                case DeletionReason.ClosedByHost:
                    MessageBox.Show("Host tej gry usunął pokój.\nNastąpiło przeniesienie do okna głównego.",
                    "Informacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case DeletionReason.JoiningTimeout:
                    MessageBox.Show("Przekroczono dozwolony czas podłączania graczy do pokoju.\nPokój został zamknięty",
                    "Informacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case DeletionReason.ReadinessTimeout:
                    MessageBox.Show("Przekroczono dozwolony czas potwierdzenia gotowości do gry.\nPokój został zamknięty",
                    "Informacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case DeletionReason.LostConnection:
                    MessageBox.Show("Jeden z graczy nie odpowiada.\nPokój został zamknięty.",
                    "Informacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case DeletionReason.PlayerLeftGame:
                    MessageBox.Show("Jeden z graczy zamknął okno z grą.\nPokój został zamknięty.",
                    "Informacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                default:
                    MessageBox.Show("Pokój został zamknięty.",
                    "Informacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }

            SynchCont.Post(_ => EnableOrDisableMenu(true), null);
        }

        #endregion

        #region Assign Makao Game Host Endpoint

        //method for assigning Uri to private field, that can be accessed
        //through public property
        public static void AssignHostEndpoint(Uri endpoint)
        {
            makaoGameHostEndpoint = endpoint;
        }

        #endregion

        #region Delete old Log files

        private void DeleteOldLogFiles()
        {
            //List<DirectoryInfo> filesToDelete = new List<DirectoryInfo>();
            DirectoryInfo info = new DirectoryInfo(logLocationString);
            FileInfo[] files = info.GetFiles("*.txt");
            //deleting files 3 days older than the day at which the app starts
            DateTime deletionDate = DateTime.Today.AddDays(-3);
            for (int i = 0; i < files.Length; i++)
            {
                if ((DateTime.Compare(files[i].CreationTime, deletionDate) < 0))
                {
                    File.Delete(files[i].FullName);
                }
            }
        }

        #endregion

        #region NLog configuration

        //Nlog configuration
        private void NLogConfigMethod(string source)
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = source };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;

            //log construction of the engine
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Application started");
            logger.Info($"LOG LOCATION: {logLocationString}");
        }

        #endregion

        #region Saving and reading data (App settings)

        //method for reading data at start of the app
        private void ReadDataFromSettings(SettingsData data)
        {
            //SettingsData data = Settings.ReadSettings();
            SavedDataPlaceholder.CurrentPlayerName = data.UserName;
            SavedDataPlaceholder.CurrentAvatarPicture = data.TypeOfAvatar;
            SavedDataPlaceholder.SavedAmountOfPlayers = data.AmountOfPlayers;
            SavedDataPlaceholder.SavedAmountOfDecks = data.AmountOfDecks;
            SavedDataPlaceholder.SavedAmountOfJokers = data.AmountOfJokers;
            SavedDataPlaceholder.SavedAmountOfStartCards = data.AmountOfStartCards;
            SavedDataPlaceholder.SavedAmountOfPlayedGames = data.PlayedGames;
            SavedDataPlaceholder.SavedAmountOfPlayedAndWonGames = data.PlayedAndWonGames;
            SavedDataPlaceholder.ReadinessTimeoutEnabled = data.ReadinessTimeoutEnabled;
            SavedDataPlaceholder.ReadinessTimeoutMinutesAmount = data.WaitingForPlayersReadinessTimeout;
            SavedDataPlaceholder.JoiningTimeoutEnabled = data.JoiningTimeoutEnabled;
            SavedDataPlaceholder.JoiningTimeoutMinutesAmount = data.WaitingForPlayersJoiningTimeout;
            SavedDataPlaceholder.CardsBackColor = data.CardsBackColor;
            SavedDataPlaceholder.LocationOfThirdPlayersCards = data.LocationOfThirdPlayer;

            UsernameText.Text = data.UserName;
            UserNameText.Text = data.UserName;

            //reload avatar
            AssignAvatar(SavedDataPlaceholder.CurrentAvatarPicture);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Reading saved user data ended successfully.");
        }

        //while closing app - save the data
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (rulesWindow == null)
            {
                //delete user from room when clising the app
                DeleteUserFromRoomAlgorithmMethod(LeavingTheRoomWindowType.MainWindow);

                //logging out
                Logout();

                //stop the engine host service if it is running
                EngineHostHandler.StopService("MakaoGameHostService");

                //stop the game client service
                EngineClientHandler.Dispose();

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Saving user data ended successfully.");
            }
            else
            {
                e.Cancel = true;
                if (rulesWindow.WindowState == WindowState.Minimized) rulesWindow.WindowState = WindowState.Normal;
                rulesWindow.Activate();
            }
        }

        //method for logging out of current player
        private void Logout()
        {
            if (playerDBID > 0)
            {
                SettingsData data = new SettingsData
                {
                    TypeOfAvatar = SavedDataPlaceholder.CurrentAvatarPicture,
                    UserName = SavedDataPlaceholder.CurrentPlayerName,
                    AmountOfPlayers = SavedDataPlaceholder.SavedAmountOfPlayers,
                    AmountOfJokers = SavedDataPlaceholder.SavedAmountOfJokers,
                    AmountOfDecks = SavedDataPlaceholder.SavedAmountOfDecks,
                    AmountOfStartCards = SavedDataPlaceholder.SavedAmountOfStartCards,
                    PlayedGames = SavedDataPlaceholder.SavedAmountOfPlayedGames,
                    PlayedAndWonGames = SavedDataPlaceholder.SavedAmountOfPlayedAndWonGames,
                    ReadinessTimeoutEnabled = SavedDataPlaceholder.ReadinessTimeoutEnabled,
                    WaitingForPlayersReadinessTimeout = SavedDataPlaceholder.ReadinessTimeoutMinutesAmount,
                    JoiningTimeoutEnabled = SavedDataPlaceholder.JoiningTimeoutEnabled,
                    WaitingForPlayersJoiningTimeout = SavedDataPlaceholder.JoiningTimeoutMinutesAmount,
                    CardsBackColor = SavedDataPlaceholder.CardsBackColor,
                    LocationOfThirdPlayer = SavedDataPlaceholder.LocationOfThirdPlayersCards,
                };
                RealmUserHandler handler = new RealmUserHandler();
                handler.UpdatePlayerName(playerDBID, SavedDataPlaceholder.CurrentPlayerName);
                handler.UpdatePlayerData(data, playerDBID);
            }

            playerDBID = 0;
            menuLogout.IsEnabled = false;

            //reading saved data at the start of application
            ReadDataFromSettings(DefaultUserSettings.GetDefaultSettings());

            //reload avatar
            AssignAvatar(SavedDataPlaceholder.CurrentAvatarPicture);
        }

        #endregion

        #region Adjustnment of user

        //action for pressing button for changing avatar - open window
        private void ChangeAvatatarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Windows.AvatarChoice avatarWindow = new Windows.AvatarChoice(SavedDataPlaceholder.CurrentAvatarPicture)
                {
                    Owner = this
                };
                avatarWindow.AvatarAssignmentChanged += AvatarVindowConfirm_Click;

                //logging
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Opening avatar choice window");

                avatarWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Opening avatar choice window failure: " + ex.Message);
            }

            MemoryManagement.FlushMemory();
        }

        //confirmation of avatar choice
        private void AvatarVindowConfirm_Click(object sender, AvatarAssigningEventArgs e)
        {
            SavedDataPlaceholder.CurrentAvatarPicture = e.TypeOfAvatar;
            AssignAvatar(SavedDataPlaceholder.CurrentAvatarPicture);
        }

        //assign image to avatar - from full fire directory - path in the file explorer
        private void AssignAvatar(string imagePath)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(imagePath);
                image.EndInit();
                AvatarImage.Source = image;

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Current player avatar changed to: " + imagePath);
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Couldn't load avatar graphics in main window: " + ex.Message);
            }
        }

        //change of user
        private void UsernameText_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SavedDataPlaceholder.CurrentPlayerName = (sender as TextBox).Text;
                UserNameText.Text = SavedDataPlaceholder.CurrentPlayerName;

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Current player name changed to: " + (sender as TextBox).Text);
            }
        }

        #endregion

        #region Settings window handling

        //opening the settings window
        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {
            Windows.UserSettingsWindow settingsWindow = new UserSettingsWindow
                (SavedDataPlaceholder.CardsBackColor, SavedDataPlaceholder.JoiningTimeoutEnabled, SavedDataPlaceholder.JoiningTimeoutMinutesAmount,
                SavedDataPlaceholder.ReadinessTimeoutEnabled, SavedDataPlaceholder.ReadinessTimeoutMinutesAmount, SavedDataPlaceholder.LocationOfThirdPlayersCards)
            {
                Owner = this,
            };
            settingsWindow.SettingsChanged += SettingsWindow_SettingsChanged;
            settingsWindow.ShowDialog();
            MemoryManagement.FlushMemory();
        }

        //catching args frorom event - closing window with confirm button
        private void SettingsWindow_SettingsChanged(object sender, SettingsDataEventArgs e)
        {
            SavedDataPlaceholder.ReadinessTimeoutMinutesAmount = e.ReadinessTimeout;
            SavedDataPlaceholder.JoiningTimeoutMinutesAmount = e.JoiningTimeout;
            SavedDataPlaceholder.JoiningTimeoutEnabled = e.JoiningEnabled;
            SavedDataPlaceholder.ReadinessTimeoutEnabled = e.ReadinessEnabled;
            SavedDataPlaceholder.CardsBackColor = e.CardBackColor;
            SavedDataPlaceholder.LocationOfThirdPlayersCards = e.LocationOfThirdPLayersCards;
        }

        #endregion

        #region Starting new game (New game window)

        //event handler for clicking menu item - cretion of the new room
        private void MenuStartNewGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Windows.StartNewHostWindow newGameWindow = new Windows.StartNewHostWindow
                    (SavedDataPlaceholder.SavedAmountOfPlayers, SavedDataPlaceholder.SavedAmountOfDecks,
                    SavedDataPlaceholder.SavedAmountOfJokers, SavedDataPlaceholder.SavedAmountOfStartCards)
                {
                    Owner = this
                };
                newGameWindow.NewRoomDataChanged += NewRoomDataConfirm_Click;

                //logging
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Opening creation of new game window");

                newGameWindow.ShowDialog();
                MemoryManagement.FlushMemory();
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Opening creation of new game window failure: " + ex.Message);
            }
        }

        //confirmation of avatar choice
        private void NewRoomDataConfirm_Click(object sender, NewRoomDataEventArgs e)
        {
            newRoomPassword = e.Password;
            SavedDataPlaceholder.SavedAmountOfPlayers = e.AmountOfPlayers;
            SavedDataPlaceholder.SavedAmountOfDecks = e.AmountOfDecks;
            SavedDataPlaceholder.SavedAmountOfJokers = e.AmountOfJokers;
            SavedDataPlaceholder.SavedAmountOfStartCards = e.AmountOfStartCards;

            ShowNewMainWindowPage(typeof(PlayersListPage), true);
        }

        #endregion

        #region Join new game (Endpoint page list)

        //event for opening page of EngineHostListPage
        private void MenuJoinGame_Click(object sender, RoutedEventArgs e)
        {
            ShowNewMainWindowPage(typeof(EngineHostsListPage), false);
        }

        //event handler that is fired when player has successfully joined the room
        private void CurrentEngineHostsListPage_SuccessfullyJoinedToTheRoom(object sender, EventArgs e)
        {
            ShowNewMainWindowPage(typeof(PlayersListPage), false);
        }

        #endregion

        #region Handling pages of frame in main window

        //method for displaying proper page in main window frame
        //MainWindowFrame
        private void ShowNewMainWindowPage(Type pageType, bool startPlayersListAsHost)
        {
            //MainWindow.ChangeGameStatus(AppType.None);

            if (pageType == typeof(PlayersListPage))
            {   openedPageNumber = 1;
                /* Different page constructors. If starting this page when user is host,
                 * page's constructor starts also the service with makao game host, but 
                 * when creating this page as not host player, there is no need of
                 * starting mentioned service, which is Windows Service
                 */
                if (startPlayersListAsHost) CurrentPlayersListPage = new PlayersListPage(newRoomPassword);
                else CurrentPlayersListPage = new PlayersListPage();
                CurrentPlayersListPage.ReadinessToPlayConfirmed += CurrentPlayersListPage_ReadinessToPlayConfirmed;
                CurrentPlayersListPage.RoomLeftByUser += CurrentPlayersListPage_RoomLeftByUser;
                CurrentPlayersListPage.ChangeAvailabilityOfMenus += CurrentPlayersListPage_ChangeAvailabilityOfMenus;
                CurrentEngineHostsListPage = null;
                MainWindowFrame.Content = CurrentPlayersListPage;
            }
            else if (pageType == typeof(EngineHostsListPage))
            {
                openedPageNumber = 2;
                //if Engine Host Handler started the service, stop it
                StopHostServiceAndDisposeEngineHostHandler();

                CurrentEngineHostsListPage = new EngineHostsListPage(this);
                CurrentEngineHostsListPage.SuccessfullyJoinedToTheRoom += CurrentEngineHostsListPage_SuccessfullyJoinedToTheRoom;
                CurrentPlayersListPage = null;
                MainWindowFrame.Content = CurrentEngineHostsListPage;
            }

            MemoryManagement.FlushMemory();
        }
        //event from players list page - someone left the room by closing app
        private void CurrentPlayersListPage_ChangeAvailabilityOfMenus(object sender, EventArgs e)
        {
            SynchCont.Post(_ => EnableOrDisableMenu(true), null);
        }

        //PlayersListPage - event when user click lave the room button
        private void CurrentPlayersListPage_RoomLeftByUser(object sender, EventArgs e)
        {
            StartDeletionAlgorithm(LeavingTheRoomWindowType.MainWindow);
        }

        //method for starting deletion room algorithm
        private void StartDeletionAlgorithm(LeavingTheRoomWindowType windowType)
        {
            //restore default main window view
            MainWindowFrame.Content = null;

            //reset application type to none
            MainWindow.ChangeGameStatus(AppType.None);

            Task.Run(() => DeleteUserFromRoomAlgorithmMethod(windowType));
        }

        //deletion from room algorthm
        private void DeleteUserFromRoomAlgorithmMethod(LeavingTheRoomWindowType windowType)
        {
            LeaveTheRoomRequest request = null;
            IMakaoGameHostService proxy = null;

            try //try to connect to service host - may be problem when didin't connect before and endpoint is not assigned
            {
                if (makaoGameHostEndpoint == null) AssignHostEndpoint(MakaoGameHostServiceEndpointObtainer.GetHostEndpointWhileBeeingHost());
                ChannelFactory<IMakaoGameHostService> factory = new ChannelFactory<IMakaoGameHostService>(new BasicHttpBinding(),
                    new EndpointAddress(MakaoGameHostEndpoint));

                //creating request
                request = new LeaveTheRoomRequest()
                {
                    IsHostPlayer = IsHostPlayer,
                    PlayerID = PlayerID,
                    PlayerName = SavedDataPlaceholder.CurrentPlayerName,
                    PlayerNumber = PlayerNumber,
                    ClosedWindowType = windowType,
                };
                proxy = factory.CreateChannel();
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Error while making attemp to connect to host to delete player " + ex.Message + ex.StackTrace);
            }
            
            bool response = false;
            try
            {
                response = proxy.DeletePlayerFromRoom(request);
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Error while obtaning response from host about deletion of player: " + ex.Message + ex.StackTrace);
            }

            if (response)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Current player was deleted from room");
            }

            if (IsHostPlayer == true) Task.Run(() => StopTheHostServiceWithDelay(3));
        }

        //timer for stopping host service with delay
        private void StopTheHostServiceWithDelay (int seconds)
        {
            StopServiceTimer = new System.Timers.Timer()
            {
                Interval = seconds * 1000 + 1,
                AutoReset = false,
                Enabled = true,
            };

            StopServiceTimer.Elapsed += StopServiceTimer_Elapsed;
        }

        //event when timer counts the time
        private void StopServiceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //stop the timer
            if (StopServiceTimer != null)
            {
                StopServiceTimer.Stop();
                StopServiceTimer.Dispose();
                StopServiceTimer = null;
            }

            EngineHostHandler.StopService("MakaoGameHostService");
        }

        //event fired when player click the button that confirms joining the game
        private void CurrentPlayersListPage_ReadinessToPlayConfirmed(object sender, EventArgs e)
        {
            //disable possibility of creating the room or joining the room
            EnableOrDisableMenu(false);
        }

        //method for enabling and disabling the menu
        private void EnableOrDisableMenu(bool enable)
        {
            menuSettings.IsEnabled = enable;
            menuStartNewGame.IsEnabled = enable;
            menuJoinGame.IsEnabled = enable;
        }

        //method for starting game client service
        private void StartMakaoGameClientService()
        {
            EngineClientHandler.StartTheMakaoGameClientService(thisClientEndpoint);
        }

        public static void SetThisClientEndpoint(Uri endpoint)
        {
            thisClientEndpoint = endpoint;
        }

        private void StopHostServiceAndDisposeEngineHostHandler()
        {
            if (EngineHostHandler.ServiceName != "" || EngineHostHandler.ServiceName != null)
            {
                EngineHostHandler.Dispose();
            }
        }

        #endregion

        #region Window with game rules handling

        //opening window with rules of this game
        private void MenuRules_Click(object sender, RoutedEventArgs e)
        {
            if (rulesWindow == null)
            {
                rulesWindow = new RulesWindow.RulesWindow()
                {
                    Owner = this,
                };
                rulesWindow.WindowClosing += RulesWindow_WindowClosing;
                rulesWindow.Show();
            }
            else
            {
                rulesWindow.Activate();
            }
            MemoryManagement.FlushMemory();
        }

        //event for closing rulew window
        private void RulesWindow_WindowClosing(object sender, EventArgs e)
        {
            rulesWindow.WindowClosing -= RulesWindow_WindowClosing;
            rulesWindow = null;
        }

        #endregion

        //TODO : delete all below this after tests

        #region Testing controls in other window

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SavedDataPlaceholder.SavedAmountOfPlayedGames = 0;
            SavedDataPlaceholder.SavedAmountOfPlayedAndWonGames = 0;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            List<PlayingCard> cardsList = new List<PlayingCard>
            {
                new PlayingCard(CardSuits.None, CardRanks.Joker, 1),
                new PlayingCard(CardSuits.Diamond, CardRanks.Seven, 1),
                new PlayingCard(CardSuits.Spade, CardRanks.Seven, 1),
                new PlayingCard(CardSuits.Heart, CardRanks.Ace, 1),
                new PlayingCard(CardSuits.Heart, CardRanks.Jack, 1),
            };

            ThisPlayerData thisPlayerData = new ThisPlayerData
            {
                ThisPlayerID = "dwefkwbfwhbfwhbfb",
                ThisPlayerName = "Michał",
                ThisPlayerNumber = 0,
                ThisPlayerCards = cardsList
            };

            List<OtherPlayerData> otherPlayerData = new List<OtherPlayerData>();

            OtherPlayerData opd1 = new OtherPlayerData
            {
                OtherPlayerAmountOfCards = 5,
                OtherPlayerID = "fwefergergerg",
                OtherPlayerName = "Marek",
                OtherPlayerNumber = 1,
            };

            OtherPlayerData opd2 = new OtherPlayerData
            {
                OtherPlayerAmountOfCards = 4,
                OtherPlayerID = "ffvebtnyukio.io.u,",
                OtherPlayerName = "Gosia",
                OtherPlayerNumber = 2,
            };
            
            OtherPlayerData opd3 = new OtherPlayerData
            {
                OtherPlayerAmountOfCards = 6,
                OtherPlayerID = "fwewevrtntyyu",
                OtherPlayerName = "Basia",
                OtherPlayerNumber = 3,
            };

            otherPlayerData.Add(opd1);
            otherPlayerData.Add(opd2);
            otherPlayerData.Add(opd3);

            GameStateData gameData = new GameStateData()
            {
                AmountOfPausingTurns = 0,
                CurrentlyDemandedRank = CardRanks.None,
                CurrentlyDemandedSuit = CardSuits.None,
                CurrentPlayerNumber = 0,
                AmountOfCardsToTakeIfLostBattle = 0,
                BlockPossibilityOfTakingCardsFromDeck = false,
            };


            PersonalizedForSpecificPlayerStartGameDataRequest data = new PersonalizedForSpecificPlayerStartGameDataRequest
            {
                MinimumPlayerNumber = 0,
                MaximumPlayerNumber = 3,
                AmountOfPlayers = 4,
                CurrentGameStatusData = gameData,
                PlayerID = "dwefkwbfwhbfwhbfb",
                NewCardsOnTheTableList = new List<PlayingCard> { new PlayingCard(CardSuits.Heart, CardRanks.Seven, 1) },
                AmountOfCardsInDeck = 58,
                CurrentPlayerNumber = 0,
                DataOfThisPlayer = thisPlayerData,
                DataOfOtherPlayers = otherPlayerData
            };

            Windows.GameWindow gameWindow = new Windows.GameWindow(ThirdPlayerLocation.Left, data, CardGraphicsLibraryHandler.BackColor.Blue)
            {
                Owner = this,
            };
            gameWindow.ShowDialog();
        }

        #endregion
    }

    #region Enum and EventArgs for chaning the app status

    //enumeration of types of this app
    public enum AppType
    {
        Host,
        Client,
        None,
    }

    //receiving data about players readiness to play from host
    public class ChangeAppStatusEventArgs : EventArgs
    {
        public AppType NewStatus;
    }

    #endregion
}
