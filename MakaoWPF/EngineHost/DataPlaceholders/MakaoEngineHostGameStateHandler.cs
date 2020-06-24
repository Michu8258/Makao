using EngineHost.GameStateUpdatesSending;
using EngineHost.PersonalizedGameDataMakerClasses;
using MakaoGameClientService.DataTransferObjects;
using MakaoGameClientService.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EngineHost.DataPlaceholders
{
    static class MakaoEngineHostGameStateHandler
    {
        #region Start new Game algorithm

        //method for obtaining data to send them to specific player
        private static PersonalizedForSpecificPlayerStartGameDataRequest GetDataForParticularPlayerAtTheGameStart
            (string playerID, int minPlayerNumber, int maxPlayerNumber, int totalAmountOfPlayers)
        {
            if (GameStateHolder.EngineConstructed)
            {
                GameDataCollector DataCollector = new GameDataCollector
                    (playerID, minPlayerNumber, maxPlayerNumber, totalAmountOfPlayers);

                var data = DataCollector.GetDataForPlayer();

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info($"Data from StartupDataCollector class returned with ID of this player: {data.DataOfThisPlayer.ThisPlayerID}.");

                return data;
            }
            else return null;
        }

        //method for adding start game data for players to lists
        private static void AddOnePlayerDataForStartGameLists(ref List<object> requestsOutput, ref List<string> listOfIDoutput,
            string playerID, int minNumber, int maxNumber, int totalPLayersAmount)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            PersonalizedForSpecificPlayerStartGameDataRequest data = GetDataForParticularPlayerAtTheGameStart( playerID, minNumber, maxNumber, totalPLayersAmount);
            logger.Info($"Personalized data for player with number: {data.DataOfThisPlayer.ThisPlayerNumber}.");

            requestsOutput.Add((object)data);
            listOfIDoutput.Add(playerID);

            logger.Info($"Obtained personalized data for player with name and ID: {data.DataOfThisPlayer.ThisPlayerName}, {playerID}.");
        }

        #endregion

        #region  Update the game algorithm

        //method for obtaining update data to send them to specific player
        private static PersonalizedPlayerDataRequest GetUpdateGameStateDataForSpecificPlayer
            (string playerID, int minPlayerNumber, int maxPlayerNumber)
        {
            if (GameStateHolder.EngineConstructed)
            {
                UpdateDataCollector DataCollector = new UpdateDataCollector
                    (playerID, minPlayerNumber, maxPlayerNumber, false);

                var data = DataCollector.GetDataForPlayer();

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info($"Data from StartupDataCollector class returned with ID of this player: {data.DataOfThisPlayer.ThisPlayerID}.");

                return data;
            }
            else return null;
        }

        //method for adding ipdate game data for players to lists
        private static void AddOnePlayerDataForUpdateLists(ref List<object> requestsOutput, ref List<string> listOfIDoutput,
            string playerID, int minNumber, int maxNumber)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            PersonalizedPlayerDataRequest data = GetUpdateGameStateDataForSpecificPlayer(playerID, minNumber, maxNumber);
            logger.Info($"Personalized updating data for player with number: {data.DataOfThisPlayer.ThisPlayerNumber},.");

            requestsOutput.Add((object)data);
            listOfIDoutput.Add(playerID);

            logger.Info($"Obtained personalized updating data for player with name and ID: {data.DataOfThisPlayer.ThisPlayerName}, {playerID}.");
        }

        #endregion

        #region Common region

        public static bool ExecutePlayersDataSendingAlgorithm(int amountOfPlayers, int amountOfDecks, int amountOfJokers, int amountOfCards, DataSenderType dataType)
        {
            //list of data received from clients
            bool output = false;
            bool gameCreated;

            //start the engine with proper amount of players, decks etc.
            switch (dataType)
            {
                case DataSenderType.EngineInstanceCreatedData:
                    gameCreated = GameStateHolder.CreateNewGame(amountOfPlayers, amountOfDecks, amountOfJokers, amountOfCards);
                    GameStateHolder.StartGameTimer();
                    break;
                case DataSenderType.EngineInstanceUpdateData:
                case DataSenderType.GameFinished:
                default:
                    gameCreated = GameStateHolder.EngineConstructed; break;
            }

            if (gameCreated)
            {
                //collecting personalized data for each user - starting new game data
                List<object> requestsList = null; List<string> listOfID = null;
                switch (dataType)
                {
                    case DataSenderType.EngineInstanceCreatedData: (requestsList, listOfID) = CollectPlayersPersonalizedDataGameCreation(typeof(PersonalizedForSpecificPlayerStartGameDataRequest)); break;
                    case DataSenderType.EngineInstanceUpdateData: (requestsList, listOfID) = CollectPlayersPersonalizedDataGameCreation(typeof(PersonalizedPlayerDataRequest)); break;
                }

                //send this data to each client
                if ((requestsList != null && listOfID != null) || dataType == DataSenderType.GameFinished)
                {
                    switch (dataType)
                    {
                        case DataSenderType.EngineInstanceCreatedData:
                            GameStateDataSender sender = new GameStateDataSender();
                            output = ClientResponseAnalyzer(sender.SendDataAboutCreationOfNewRoom(requestsList, listOfID));
                            break;
                        case DataSenderType.EngineInstanceUpdateData:
                            GameStateDataSender semder2 = new GameStateDataSender();
                            output = ClientResponseAnalyzer(semder2.SendUpdatedDataToPlayers(requestsList, listOfID));
                            break;
                        case DataSenderType.GameFinished:
                            GameStateDataSender sender3 = new GameStateDataSender();
                            output = ClientResponseAnalyzer(sender3.SendGameFinishedDataToPlayers(CollectEndGameData()));
                            break;
                    }
                }
                else
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Error($"Didn't collect players personalized data, method returned at least one null. Type: {dataType.ToString()}.");
                }
            }
            else
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Game didn't start.");
            }

            return output;
        }

        private static (List<object>, List<string>) CollectPlayersPersonalizedDataGameCreation(Type dataType)
        {
            //establish common data for each player
            int minNumber = MakaoEngineHostDataPlaceholders.PlayersData.Min(x => x.PlayerNumber);
            int maxNumber = MakaoEngineHostDataPlaceholders.PlayersData.Max(x => x.PlayerNumber);
            int totalPLayersAmount = MakaoEngineHostDataPlaceholders.PlayersData.Count;

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"Collecting players personalized data: minimum number = {minNumber}, maximum number = {maxNumber}, amount of players = {totalPLayersAmount}.");

            //return data
            List<object> requestsOutput = new List<object>();
            List<string> listOfIDoutput = new List<string>();

            try
            {
                //for every one of players, get personaized data
                foreach (PlayerData item in MakaoEngineHostDataPlaceholders.PlayersData)
                {
                    if (dataType == typeof(PersonalizedPlayerDataRequest))
                    {
                        AddOnePlayerDataForUpdateLists(ref requestsOutput, ref listOfIDoutput, item.PlayerID, minNumber, maxNumber);
                    }
                    else if (dataType == typeof(PersonalizedForSpecificPlayerStartGameDataRequest))
                    {
                        AddOnePlayerDataForStartGameLists(ref requestsOutput, ref listOfIDoutput, item.PlayerID, minNumber, maxNumber, totalPLayersAmount);
                    }
                }
            }
            catch (Exception ex)
            {
                //if something went wrong, make return data null, and log an exception
                requestsOutput = null;
                listOfIDoutput = null;

                logger.Error($"Couldn't get players personalized data at the start of game: {ex.Message}; {ex.StackTrace}.");
            }

            return (requestsOutput, listOfIDoutput);
        }

        //Received data from client analization - check if all clients send response
        private static bool ClientResponseAnalyzer(List<ReturnData> responses)
        {
            bool output = true;

            try
            {
                foreach (ReturnData item in responses)
                {
                    if ((bool)item.Response == false) { output = false; break; }
                }
            }
            catch (Exception ex)
            {
                output = false;
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Couldn't analyze data responses from clients - StartGameResponses: {ex.Message}.");
            }

            return output;
        }

        #endregion

        #region End game data collecting

        //method for collecting data or game finishing
        private static GameFinishedDataRequest CollectEndGameData()
        {
            GameFinishedDataRequest output = new GameFinishedDataRequest
            {
                WinnerPlayerNumber = DataPlaceholders.GameStateHolder.EngineInstance.FinishedPlayers[1],
                GamersList = new List<PlayerPositionDetails>(),
                GameTimerTimeSpan = GameStateHolder.GameTimerTimeSpan,
                GameTimerTimeSpanMiliseconds = GameStateHolder.GameTimerTimeSpanMiliseconds,
            };

            foreach (var item in DataPlaceholders.MakaoEngineHostDataPlaceholders.PlayersData)
            {
                try
                {
                    PlayerPositionDetails onePlayerData = new PlayerPositionDetails()
                    {
                        PlayerPosition = GetPlayerPosition(item.PlayerNumber, DataPlaceholders.GameStateHolder.EngineInstance.FinishedPlayers),
                        PlayerID = item.PlayerID,
                        PlayedAndWonGames = CheckIfThisPLayerWon(item.PlayerNumber, output.WinnerPlayerNumber, item.PlayedAndWonGames),
                        PlayedGames = item.PlayedGames + 1,
                        PlayerNumber = item.PlayerNumber,
                        PlayerName = item.PlayerName,
                    };
                    output.GamersList.Add(onePlayerData);
                }
                catch (Exception ex)
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Error($"Error while obtaining data for finished game window (in host): {ex.Message}.");
                }
            }

            return output;
        }

        private static int GetPlayerPosition (int playerNumber, Dictionary<int, int> finishedPlayers)
        {
            int output = -5;
            foreach (var item in finishedPlayers)
            {
                if (item.Value == playerNumber)
                {
                    output = item.Key;
                }
            }
            return output;
        }

        private static int CheckIfThisPLayerWon(int playerNumber, int winnerNumber, int wonGamesAmount)
        {
            if (playerNumber == winnerNumber) return wonGamesAmount + 1;
            else return wonGamesAmount;
        }

        #endregion
    }
}
