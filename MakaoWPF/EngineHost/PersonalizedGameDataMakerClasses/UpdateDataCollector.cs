using MakaoGameClientService.DataTransferObjects;
using MakaoGameClientService.Messages;
using MakaoGraphicsRepresentation.MainWindowData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EngineHost.PersonalizedGameDataMakerClasses
{
    class UpdateDataCollector
    {
        #region Fields and properties

        //data from constructing the instance of this class
        protected readonly string playerID;
        protected readonly int minPlayerNumber;
        protected readonly int maxPlayerNumber;
        protected int playerNumber;

        //construction of object is correct
        protected bool constructorDataCorrect;
        public bool ConstructedCorrectly { get { return constructorDataCorrect; } }

        #endregion

        #region Constructor

        //constructor method
        public UpdateDataCollector(string playerID, int minPlayerNumber, int maxPlayerNumber, bool calledFromDerivedClass)
        {
            //assigning data passed to constructor
            this.playerID = playerID;
            this.minPlayerNumber = minPlayerNumber;
            this.maxPlayerNumber = maxPlayerNumber;
            playerNumber = -10;

            if (!calledFromDerivedClass)
            {
                //checking data correctness
                CheckConstructorData();

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("StartupDataCollector class constructed.");
            }
        }

        #endregion

        #region Passed data validation

        //method for checking if data passed to constructor is correct
        private void CheckConstructorData()
        {
            bool minNumberOK = false;
            CheckMinimumNumberCorrectness(ref minNumberOK);

            bool maxNumberOK = false;
            CheckMaximumNumberCorrectness(ref maxNumberOK);

            bool playerNumberOK = false;
            CheckPlayerNumberCorrectness(ref playerNumberOK);

            constructorDataCorrect = minNumberOK && maxNumberOK && playerNumberOK;

            if (!constructorDataCorrect)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Data passed to constructor of StartupDataCollector doesn't match data in Placeholder class");
            }
        }

        //minimum number checking
        protected void CheckMinimumNumberCorrectness(ref bool valid)
        {
            try { if (DataPlaceholders.MakaoEngineHostDataPlaceholders.PlayersData.Min(x => x.PlayerNumber) == minPlayerNumber) valid = true; }
            catch (Exception ex)
            {
                valid = false;
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error when tried to confirm minimum number data: {ex.Message}.");
            }
        }

        //maximum number checking
        protected void CheckMaximumNumberCorrectness(ref bool valid)
        {
            try { if (DataPlaceholders.MakaoEngineHostDataPlaceholders.PlayersData.Max(x => x.PlayerNumber) == maxPlayerNumber) valid = true; }
            catch (Exception ex)
            {
                valid = false;
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error when tried to confirm maximum number data: {ex.Message}.");
            }
        }

        //check player number - based on player ID passed to constructor
        protected void CheckPlayerNumberCorrectness(ref bool valid)
        {
            try
            {
                playerNumber = DataPlaceholders.MakaoEngineHostDataPlaceholders.PlayersData.Single(x => x.PlayerID == playerID).PlayerNumber;
                if (playerNumber >= minPlayerNumber && playerNumber <= maxPlayerNumber) valid = true;
            }
            catch (Exception ex)
            {
                valid = false;
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error when tried to confirm player number data: {ex.Message}.");
            }
        }

        #endregion

        #region Obtaining data for response

        //method that starts algorithm
        public PersonalizedPlayerDataRequest GetDataForPlayer()
        {
            PersonalizedPlayerDataRequest returnData = new PersonalizedPlayerDataRequest();

            if (constructorDataCorrect)
            {
                returnData.PlayerID = playerID;
                returnData.NewCardsOnTheTableList = DataPlaceholders.GameStateHolder.EngineInstance.CardsLatelyPutOnTheTable;
                returnData.CurrentPlayerNumber = DataPlaceholders.GameStateHolder.EngineInstance.CurrentPlayer;
                returnData.AmountOfCardsInDeck = DataPlaceholders.GameStateHolder.EngineInstance.Deck.Count;
                returnData.CurrentGameStatusData = GetCurrentGameStateSata();
                returnData.DataOfThisPlayer = GetInfoAboutCurrentPlayer();
                returnData.DataOfOtherPlayers = GetInfoAboutOtherPlayers();
            }
            else
            {
                returnData = null;
            }

            return returnData;
        }

        //method for getting current game status data
        protected GameStateData GetCurrentGameStateSata()
        {
            GameStateData Data = new GameStateData()
            {
                AmountOfPausingTurns = DataPlaceholders.GameStateHolder.EngineInstance.PlayersData[playerNumber].PauseTurnsAmount,
                CurrentlyDemandedRank = DataPlaceholders.GameStateHolder.EngineInstance.DemandedRank,
                CurrentlyDemandedSuit = DataPlaceholders.GameStateHolder.EngineInstance.DemandedSuit,
                CurrentPlayerNumber = DataPlaceholders.GameStateHolder.EngineInstance.CurrentPlayer,
                CurrentStatusOfTheGame = DataPlaceholders.GameStateHolder.EngineInstance.Status,
                AmountOfCardsToTakeIfLostBattle = DataPlaceholders.GameStateHolder.EngineInstance.AmountOfCardsToTake,
                BlockPossibilityOfTakingCardsFromDeck = DataPlaceholders.GameStateHolder.EngineInstance.BlockTakingCardsFromDeckOption,
            };
            return Data;
        }

        //obtaining data about user with number passed in here to constructor
        protected ThisPlayerData GetInfoAboutCurrentPlayer()
        {
            ThisPlayerData returnData = new ThisPlayerData()
            {
                ThisPlayerNumber = playerNumber,
                ThisPlayerName = GetPlayerName(playerNumber),
                ThisPlayerID = GetPlayerID(playerNumber),
                ThisPlayerCards = DataPlaceholders.GameStateHolder.EngineInstance.PlayersCards[playerNumber],
                CanSkipTheMove = DataPlaceholders.GameStateHolder.EngineInstance.PlayersData[playerNumber].CanSkipTheMove,
                TakenInBattleCardMatching = DataPlaceholders.GameStateHolder.EngineInstance.PlayersData[playerNumber].FirstCardInBattleModeTakenMatches,
                MatchingCard = DataPlaceholders.GameStateHolder.EngineInstance.PlayersData[playerNumber].BattleModeMatchingCard,
            };

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"Gatherd data about this player: {playerNumber}.");

            return returnData;
        }

        //method for obtaining data for all other users
        protected List<OtherPlayerData> GetInfoAboutOtherPlayers()
        {
            List<OtherPlayerData> otherPlayersDataList = new List<OtherPlayerData>();

            for (int i = minPlayerNumber; i < maxPlayerNumber + 1; i++)
            {
                if (i != playerNumber)
                {
                    OtherPlayerData otherPlayerInfo = GetInfoAboutOneOtherPlayer(i);
                    otherPlayersDataList.Add(otherPlayerInfo);
                }
            }

            return otherPlayersDataList;
        }

        //method for obtaining data about one other player
        protected OtherPlayerData GetInfoAboutOneOtherPlayer(int otherPlayerNumber)
        {
            OtherPlayerData returnData = new OtherPlayerData()
            {
                OtherPlayerNumber = otherPlayerNumber,
                OtherPlayerName = GetPlayerName(otherPlayerNumber),
                OtherPlayerID = GetPlayerID(otherPlayerNumber),
                OtherPlayerAmountOfCards = DataPlaceholders.GameStateHolder.EngineInstance.PlayersCards[otherPlayerNumber].Count,
            };

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"Gatherd data about other player: {otherPlayerNumber}.");

            return returnData;
        }

        //get player name
        protected string GetPlayerName(int playerNumber)
        {
            return (DataPlaceholders.MakaoEngineHostDataPlaceholders.PlayersData.Single(x => x.PlayerNumber == playerNumber)).PlayerName;
        }

        //get player id
        protected string GetPlayerID(int playerName)
        {
            return (DataPlaceholders.MakaoEngineHostDataPlaceholders.PlayersData.Single(x => x.PlayerNumber == playerName)).PlayerID;
        }

        #endregion
    }
}
