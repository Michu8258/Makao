using EngineHost.GameStateUpdatesSending;
using EngineHost.ServiceImplementation;
using MakaoGameHostService.Messages;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace EngineHost.DataPlaceholders
{
    static class MakaoEngineHostDataPlaceholders
    {
        #region Stored Data

        //List of data of players in the room
        private static readonly List<PlayerData> CurrentPlayersData;
        public static List<PlayerData> PlayersData { get { return CurrentPlayersData; } }

        //storage directory of players avatars
        private static readonly string avatarsDirectory;
        public static string AvatarsLocation { get { return avatarsDirectory; } }

        //storage directory for logs
        private static readonly string logDirectory;
        public static String LogsDirectory { get { return logDirectory; } }

        //strign that indicates current log text file
        private static readonly string currentLogFile;
        public static string CurrentLogFile { get { return currentLogFile; } }

        //game setup
        private static GameSetup currentGameSetup;
        public static int TotalAmountOfPLayers { get { return currentGameSetup.AmountOfPlayers; } }

        //timers for timeouts
        private static System.Timers.Timer JoiningTimeoutTimer;
        private static System.Timers.Timer ReadinessTimeoutTimer;

        //Synchronization context
        private static SynchronizationContext synchronizationContext;

        #endregion

        #region Constructor

        //constructor
        static MakaoEngineHostDataPlaceholders()
        {
            //assign max amount of players to one - when starting app
            //currentGameSetup.AmountOfPlayers = 1;

            //avatars storage directory assignment
            avatarsDirectory = System.Reflection.Assembly.GetExecutingAssembly().
            Location.Substring(0, System.Reflection.Assembly.GetExecutingAssembly().Location.Length - 14) + @"Avatars\";

            //logs storage directory assignment
            logDirectory = System.Reflection.Assembly.GetExecutingAssembly().
            Location.Substring(0, System.Reflection.Assembly.GetExecutingAssembly().Location.Length - 14) + @"Logs\";

            //delete old log files
            DeleteOldLogFiles(5);

            //configure the NLog
            string directory = logDirectory + @"WindowsSericeHost" + DateTime.Now.ToString() + ".txt";
            NLogConfigMethod(directory);
            currentLogFile = directory;

            CurrentPlayersData = new List<PlayerData>();
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("New Current players List.");

            //make timeout timers null at the start of app
            JoiningTimeoutTimer = null;
            ReadinessTimeoutTimer = null;
        }

        #endregion

        #region NLog configuration

        //Nlog configuration
        private static void NLogConfigMethod(string source)
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = source };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;

            //log construction of the engine
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Makao Engine Data Placeholder constructed.");
        }

        #endregion

        #region Delete old Log files

        private static void DeleteOldLogFiles(int amountOfdays)
        {
            //List<DirectoryInfo> filesToDelete = new List<DirectoryInfo>();
            DirectoryInfo info = new DirectoryInfo(logDirectory);
            FileInfo[] files = info.GetFiles("*.txt");
            //deleting files 3 days older than the day at which the app starts
            DateTime deletionDate = DateTime.Today.AddDays(-amountOfdays);
            for (int i = 0; i < files.Length; i++)
            {
                if ((DateTime.Compare(files[i].CreationTime, deletionDate) < 0))
                {
                    File.Delete(files[i].FullName);
                }
            }
        }

        #endregion

        #region Game setup data handling

        //assign game setup data
        public static bool AssignGameData(AssignGameSetupDataRequest request)
        {
            currentGameSetup = new GameSetup()
            {
                AmountOfPlayers = request.AmountOfPlayers,
                AmountOfDecks = request.AmountOfDecks,
                AmountOfJokers = request.AmountOfJokers,
                AmountOfStartCards = request.AmountOfStartCards,
                RoomPassword = request.RoomPassword,
                HostID = request.HostID,
                HostName = request.HostName,
                JoiningTimeoutEnabled = request.JoiningTimeoutEnabled,
                JoiningTimeoutTimeInMinutes = request.JoiningTimeoutInMinutes,
                ReadinessTimeoutEnabled = request.ReadinessTimeoutEnabled,
                ReadinessTimeoutInMinutes = request.ReadinessTimeoutInMinutes,
            };

            if (currentGameSetup.RoomPassword == request.RoomPassword && currentGameSetup.HostID == request.HostID) return true;
            else return false;
        }

        //read host information
        public static GetRoomDetailsWhenJoiningRoomResponse GetHostRoomDetails()
        {
            GetRoomDetailsWhenJoiningRoomResponse retVal = new GetRoomDetailsWhenJoiningRoomResponse();
            try
            {
                retVal.HostName = currentGameSetup.HostName;
                retVal.AmountOfPlayersInRoom = CurrentPlayersData.Count;
                retVal.RoomCapacity = currentGameSetup.AmountOfPlayers;
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Couldn not get room detailed data when user was searching for active hosts. Exception: " + ex.Message);

                retVal.HostName = "Not assigned";
                retVal.AmountOfPlayersInRoom = 0;
                retVal.RoomCapacity = 0;
            };

            return retVal;
        }

        #endregion

        #region Assigning new user Data

        //method for adding host player to the list
        public static (bool, int) AddNewPlayerToTheRoom(AddNewPlayerRequest userRequestData, bool hostPlayer)
        {
            if (userRequestData.Password == currentGameSetup.RoomPassword)
            {
                //clear the list if it is host player
                if (hostPlayer) CurrentPlayersData.Clear();

                //minimuma mount of plaers while joining the room
                int minimumAmountOfPlayers = 0;
                if (!hostPlayer) minimumAmountOfPlayers = 1;

                //if there is free space in the room
                if (CurrentPlayersData.Count < currentGameSetup.AmountOfPlayers && CurrentPlayersData.Count >= minimumAmountOfPlayers)
                {
                    //add first player - the host
                    PlayerData playerData = new PlayerData()
                    {
                        PlayerName = userRequestData.PlayerName,
                        PlayedGames = userRequestData.PlayedGames,
                        PlayedAndWonGames = userRequestData.PlayedAndWonGames,
                        PlayerID = GetUniqueID(),
                        PlayerEndpoint = userRequestData.PlayerEndpoint,
                        PlayerNumber = GetFirstNotAssignedPlayerNumber(),
                        IsHostPlayer = hostPlayer,
                        ReadyToPlay = false,
                    };
                    CurrentPlayersData.Add(playerData);

                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info("Current amount of players in the room is: " + CurrentPlayersData.Count.ToString());

                    //control joining timeout
                    ControlTimerForJoiningTimeout();

                    //control readiness timeout
                    ControlTimerForReadinessTimeout();

                    return (true, playerData.PlayerNumber);
                }
                else
                {
                    return (false, -1);
                }
            }
            else
            {
                return (false, -1);
            }
        }

        //method that returns player number of newly added player
        private static int GetFirstNotAssignedPlayerNumber()
        {
            List<PlayerData> players = new List<PlayerData>();
            int newPlayerNumber = -10;
            for (int i = 0; i < 4; i++)
            {
                players = CurrentPlayersData.FindAll(p => p.PlayerNumber == i);
                if (players.Count == 0)
                {
                    newPlayerNumber = i;
                    break;
                }
            }
            return newPlayerNumber;
        }

        #endregion

        #region Joining and Readiness timeouts handling

        //method for starting the timer - joining timeout
        private static void ControlTimerForJoiningTimeout()
        {
            //start the timer only if enabled and first player joined the room
            if (currentGameSetup.JoiningTimeoutEnabled == true && CurrentPlayersData.Count == 1)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Start counting time for joining the room timeout.");

                //starting the timer
                JoiningTimeoutTimer = new System.Timers.Timer()
                {
                    Interval = currentGameSetup.JoiningTimeoutTimeInMinutes * 60 * 1000,
                    AutoReset = false,
                    Enabled = true,
                };
                //elapsed event subscription
                JoiningTimeoutTimer.Elapsed += JoiningTimeoutTimer_Elapsed;
                JoiningTimeoutTimer.Start();
            }

            //check if current amount of players in the room is equal to amount of players in the room
            //from game setup data
            if (CurrentPlayersData.Count == currentGameSetup.AmountOfPlayers)
            {
                DispodeJoiningTimer();
            }
        }

        //Joining to the room timeout time passed
        private static void JoiningTimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Time for joining the room just passed. Start sending info to clients.");

            //Send info to clients
            InfoSenderClass Sender = new InfoSenderClass();
            Sender.DeleteTheRoom(ClientInfoType.JoiningTimeout);

            DispodeJoiningTimer();
        }

        //disposing timer for joining timeout
        private static void DispodeJoiningTimer()
        {
            if (JoiningTimeoutTimer != null)
            {
                //stop the timer
                JoiningTimeoutTimer.Stop();
                JoiningTimeoutTimer.Dispose();
                JoiningTimeoutTimer = null;
            }
        }

        //method for starting the timer - joining timeout
        private static void ControlTimerForReadinessTimeout()
        {
            //start timer only if enabled and the room is full
            if (currentGameSetup.ReadinessTimeoutEnabled == true && CurrentPlayersData.Count == currentGameSetup.AmountOfPlayers)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Start counting time for readiness to play timeout.");

                //stop timer for joining to the room timeout
                DispodeJoiningTimer();

                //starting the timer
                ReadinessTimeoutTimer = new System.Timers.Timer()
                {
                    Interval = currentGameSetup.ReadinessTimeoutInMinutes * 60 * 1000,
                    AutoReset = false,
                    Enabled = true,
                };

                //elapsed event subscription
                ReadinessTimeoutTimer.Elapsed += ReadinessTimeoutTimer_Elapsed;
                ReadinessTimeoutTimer.Start();
            }

            //dispose timer if all players confirmed readiness
            if (ReadinessTimeoutTimer != null)
            {
                (_, bool allPlayersReady) = CheckIfPlayersAreReady();
                if (allPlayersReady) DispodeReadinessTimer();
            }
        }

        //Jreadiness to sart the game timeout time passed
        private static void ReadinessTimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Time for confirming readiness to play just passed. Start sending info to clients.");

            //Send info to clients
            InfoSenderClass Sender = new InfoSenderClass();
            Sender.DeleteTheRoom(ClientInfoType.ReadinessTimeout);

            //stop the timer
            DispodeReadinessTimer();
        }

        //disposing timer for readiness timeout
        private static void DispodeReadinessTimer()
        {
            if (ReadinessTimeoutTimer != null)
            {
                //stop the timer
                ReadinessTimeoutTimer.Stop();
                ReadinessTimeoutTimer.Dispose();
                ReadinessTimeoutTimer = null;
            }
        }

        //stopBothtimers
        private static void StopAllTimeoutTimers()
        {
            DispodeJoiningTimer();
            DispodeReadinessTimer();
        }
        #endregion

        #region Delete player from room

        //deleting player from roo if exists in the room
        public static (bool, bool) DeletePlayerFromRoom(LeaveTheRoomRequest request)
        {
            bool deleted = false;
            bool hostPlayer = false;
            int indexToRemove = -5;

            //check if player with such ID and number exists in room
            List<PlayerData> playersList = CurrentPlayersData.FindAll(p => p.PlayerID == request.PlayerID);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Deleting player from room. Amunt of players with matching ID is: " + playersList.Count.ToString());

            //if yes, delete it
            if (playersList.Count == 1)
            {
                if (playersList[0].PlayerID == request.PlayerID && playersList[0].PlayerNumber == request.PlayerNumber)
                {
                    for (int i = 0; i < CurrentPlayersData.Count; i++)
                    {
                        if (CurrentPlayersData[i].PlayerID == request.PlayerID)
                        {
                            var innerLogger = NLog.LogManager.GetCurrentClassLogger();
                            innerLogger.Info("Deleting player from room. Index in the list of matching player: " + i.ToString());

                            //remember index to remove
                            indexToRemove = i;

                            //if player was host, close the room
                            if (CurrentPlayersData[i].IsHostPlayer) hostPlayer = true;
                            break;
                        }
                    }

                    //delete the player from list
                    if (indexToRemove >= 0)
                    {
                        CurrentPlayersData.RemoveAt(indexToRemove);
                        deleted = true;

                        var innerLogger = NLog.LogManager.GetCurrentClassLogger();
                        innerLogger.Info("Deleting player from room. Playr deleted");
                    }
                }
            }

            return (deleted, hostPlayer);
        }

        #endregion

        #region Players readiness to play

        //method, which assign if player is ready to start the game
        public static bool ChangeReadinessToPlay(PlayerIsReadyToPlayGameRequest request)
        {
            bool assigned = false;
            try
            {
                //check if player with received ID is in the room
                PlayerData playerData = CurrentPlayersData.Single(p => p.PlayerID == request.PlayerID);
                if (playerData.PlayerID == request.PlayerID)
                {
                    //assign readiness status change to proper player
                    foreach (PlayerData item in CurrentPlayersData)
                    {
                        if (item.PlayerID == request.PlayerID)
                        {
                            item.ReadyToPlay = request.IsReadyToPlay;
                            assigned = true; //assignation successful
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Changing players readiness to start game. Could'n obtain player with matching number." + ex.Message);
            }
            return assigned;
        }

        #endregion

        #region Starting new game

        //method called when some user changes readiness to play status.
        public static void FireUpStartingNewGameAlgorithm(SynchronizationContext synchCont)
        {
            //assign synchronization context
            synchronizationContext = synchCont;

            //controrlling timeout measuring options
            (_, bool allPlayersReady) = CheckIfPlayersAreReady();
            if (allPlayersReady) StopAllTimeoutTimers();

            //check if there is as many players as in current game setup data
            bool allPlayersAreAlive = false;
            if (CheckIfRoomIsFull())
            {
                InfoSenderClass Sender = new InfoSenderClass();
                allPlayersAreAlive = Sender.CheckAllivenessOfCurrentPlayers();
            }

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info(string.Format("While starting new game. Readiness of players: {0}, aliveness pf all players: {1}.",
                allPlayersReady.ToString(), allPlayersAreAlive.ToString()));

            //now really start the game
            if (allPlayersReady && allPlayersAreAlive)
            {
                Task.Run(() => StartTheGame(synchCont));
            }
            //or send info to client about error of no connection to some client
            else if (allPlayersReady && !allPlayersAreAlive)
            {
                InfoSenderClass Sender = new InfoSenderClass();
                Sender.DeleteTheRoom(ClientInfoType.LostConnectionToClient);
            }
        }

        private static void StartTheGame(SynchronizationContext synchCont)
        {
            //start algorithm
            bool allPlayersReceivedData = MakaoEngineHostGameStateHandler.ExecutePlayersDataSendingAlgorithm
                (CurrentPlayersData.Count, currentGameSetup.AmountOfDecks, currentGameSetup.AmountOfJokers,
                currentGameSetup.AmountOfStartCards, DataSenderType.EngineInstanceCreatedData);

            //event from engine - start updating data sending to players
            GameStateHolder.EngineInstance.StartUpdatingTheGame += EngineInstance_StartUpdatingTheGame;

            //event from engine - end the game
            GameStateHolder.EngineInstance.EndTheGame += EngineInstance_EndTheGame;

            //if at least on player did not response, end the game
            if (!allPlayersReceivedData)
            {
                synchCont.Post(_ => SendLostConnectionRoomDeletedInfo(), null);
            }
        }

        //if game did not start properly, send info to players about deletion
        private static void SendLostConnectionRoomDeletedInfo()
        {
            InfoSenderClass Sender = new InfoSenderClass();
            Sender.DeleteTheRoom(ClientInfoType.LostConnectionToClient);
        }

        //method for checking if amount of players in the room is correct
        public static bool CheckIfRoomIsFull()
        {
            if (currentGameSetup.AmountOfPlayers == CurrentPlayersData.Count) return true;
            else return false;
        }

        //method for checking if all players confirmed that they are ready to
        //start the game and if at least one player is ready
        private static (bool, bool) CheckIfPlayersAreReady()
        {
            bool allPlayersAreReady = true;
            bool onePlayerIsReady = false;
            foreach (PlayerData item in CurrentPlayersData)
            {
                if (!item.ReadyToPlay)
                {
                    allPlayersAreReady = false;
                    break;
                }
                else
                {
                    onePlayerIsReady = true;
                }
            }
            return (onePlayerIsReady, allPlayersAreReady);
        }

        #endregion

        #region Sending data updates to players

        //event from engine host - triggers sending current game data to players
        private static void EngineInstance_StartUpdatingTheGame(object sender, EventArgs e)
        {
            //start algorithm
            bool allPlayersReceivedData = MakaoEngineHostGameStateHandler.ExecutePlayersDataSendingAlgorithm
                (CurrentPlayersData.Count, currentGameSetup.AmountOfDecks, currentGameSetup.AmountOfJokers,
                currentGameSetup.AmountOfStartCards, DataSenderType.EngineInstanceUpdateData);

            //if at least on player did not response, end the game
            if (!allPlayersReceivedData)
            {
                synchronizationContext.Post(_ => SendLostConnectionRoomDeletedInfo(), null);
            }
        }

        #endregion

        #region Ending the game procedure

        //method for sending data about game ended fact to the players
        private static void EngineInstance_EndTheGame(object sender, EventArgs e)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"Procedure of sending info to players about game ended started.");

            //stop game timer
            GameStateHolder.StopGameTimer();

            //calling the method from game state handler class
            MakaoEngineHostGameStateHandler.ExecutePlayersDataSendingAlgorithm(CurrentPlayersData.Count, currentGameSetup.AmountOfDecks,
                currentGameSetup.AmountOfJokers, currentGameSetup.AmountOfStartCards, DataSenderType.GameFinished);
        }

        #endregion

        #region Generating new unique ID for new player

        //method for obtaining new unique ID
        private static string GetUniqueID()
        {
            bool notUnique = true;
            string ID = "";
            while (notUnique)
            {
                ID = GenerateNewID();
                bool unique = CheckUniquenessOfID(ID);
                if (unique)
                {
                    notUnique = false;
                }
            }
            return ID;
        }

        //Method for generatig new ID
        private static string GenerateNewID()
        {
            //generating playerID
            IDgeneratorClass generator = new IDgeneratorClass();
            string newPlayerID = generator.GenerateID(25);
            return newPlayerID;
        }

        //check if ID passed by client is unique
        private static bool CheckUniquenessOfID(string ID)
        {
            bool unique = true;

            foreach (PlayerData item in CurrentPlayersData)
            {
                if (item.PlayerID == ID)
                {
                    unique = false;
                    break;
                }
            }

            return unique;
        }

        #endregion
    }
}
