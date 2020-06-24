using MakaoGameClientService.Messages;
using System;

namespace EngineHost.PersonalizedGameDataMakerClasses
{
    class GameDataCollector : UpdateDataCollector
    {
        #region Fields and properties

        private readonly int totalAmountOfPlayers;

        #endregion

        #region Constructor

        //constructor method
        public GameDataCollector(string playerID, int minPlayerNumber, int maxPlayerNumber, int totalAmountOfPlayers) : 
            base (playerID, minPlayerNumber, maxPlayerNumber, true)
        {
            this.totalAmountOfPlayers = totalAmountOfPlayers;

            //checking data correctness
            CheckConstructorData();

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("GameDataCollector class constructed.");
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

            bool amountOfPlayersOK = false;
            CheckAmountOfPlayersCorrectness(ref amountOfPlayersOK);

            bool playerNumberOK = false;
            CheckPlayerNumberCorrectness(ref playerNumberOK);

            constructorDataCorrect = minNumberOK && maxNumberOK && amountOfPlayersOK && playerNumberOK;

            if (!constructorDataCorrect)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Data passed to constructor of StartupDataCollector doesn't match data in Placeholder class");
            }
        }

        //check amount of players correctness
        private void CheckAmountOfPlayersCorrectness(ref bool valid)
        {
            try { if (DataPlaceholders.MakaoEngineHostDataPlaceholders.PlayersData.Count == totalAmountOfPlayers) valid = true; }
            catch (Exception ex)
            {
                valid = false;
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error when tried to confirm amount of players data: {ex.Message}.");
            }
        }

        #endregion

        #region Obtaining data for response

        //method that starts algorithm
        public new PersonalizedForSpecificPlayerStartGameDataRequest GetDataForPlayer()
        {
            PersonalizedForSpecificPlayerStartGameDataRequest returnData = new PersonalizedForSpecificPlayerStartGameDataRequest();

            if (constructorDataCorrect)
            {
                returnData.MinimumPlayerNumber = minPlayerNumber;
                returnData.MaximumPlayerNumber = maxPlayerNumber;
                returnData.PlayerID = playerID;
                returnData.AmountOfPlayers = DataPlaceholders.MakaoEngineHostDataPlaceholders.PlayersData.Count;
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

        #endregion
    }
}
