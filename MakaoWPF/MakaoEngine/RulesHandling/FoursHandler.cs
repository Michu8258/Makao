using MakaoInterfaces;
using NLog;
using System.Collections.Generic;
using System.Linq;

namespace MakaoEngine.RulesHandling
{
    class FoursHandler
    {
        #region Private fields

        private readonly List<PlayingCard> cards;
        private readonly int playerNumber;
        private readonly int amountOfPlayers;
        private readonly Logger logger;

        #endregion

        #region Constructor

        //constructor
        public FoursHandler(List<PlayingCard> cards, int playerNumber, int amountOfPlayers)
        {
            this.cards = cards;
            this.playerNumber = playerNumber;
            this.amountOfPlayers = amountOfPlayers;
            logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Public methods - algorithms

        //the main method of handling
        public (bool, bool) FourHandling(ref Dictionary<int, SinglePlayerData> PlayersCurrentData, ref GameStatus gameStatus,
            ref int temporaryPauseAmount, ref bool blockPossibilityOfTakingCard)
        {
            bool justEndedFours = false;

            //passed card correctness checking
            int amountOfFours = CheckIfAtLeastOnePassedCardIsFour();
            if (Engine.ExtendedLogging) logger.Info($"amount of passed FOURs: {amountOfFours.ToString()}");

            //in case that there are fours in the passed cards list
            if (amountOfFours > 0)
            {
                if (!CheckIfAnyPlayerStartedFours(PlayersCurrentData)) PlayersCurrentData[playerNumber].ThisPlayerStartedFours = true;
                blockPossibilityOfTakingCard = true;
                temporaryPauseAmount += amountOfFours;
                ChangeGameStatus(ref gameStatus, true);

                if (Engine.ExtendedLogging) logger.Info($"Player who started fours: {GetNumberOfPlayerThatStartedFours(PlayersCurrentData).ToString()}, temporary pause amount: {temporaryPauseAmount.ToString()}.");

                //get next player number
                NextPlayerDeterminator Determinator = new NextPlayerDeterminator();
                int nextPlayerNumber = Determinator.DetermineNextPlayerNumber(cards[0],
                    playerNumber, amountOfPlayers, PlayersCurrentData);
                if (Engine.ExtendedLogging) logger.Info($"Four handler, next player determination: {nextPlayerNumber.ToString()}.");

                bool hasNextPlayerFourOrJokerInHand = CheckIfPlayerHasFourOrJokerInHand(PlayersCurrentData[nextPlayerNumber].PlayerCards);
                if (Engine.ExtendedLogging) logger.Info($"Four handler, next player has four in hands: {hasNextPlayerFourOrJokerInHand}");

                if (temporaryPauseAmount > 0 && !hasNextPlayerFourOrJokerInHand)
                {
                    justEndedFours = AssigningStopsAlgorithm(ref PlayersCurrentData, ref gameStatus, ref temporaryPauseAmount, ref blockPossibilityOfTakingCard, nextPlayerNumber);
                }
                else if (temporaryPauseAmount > 0 && hasNextPlayerFourOrJokerInHand)
                {
                    SkippingTheMoveHandler(ref PlayersCurrentData, true, nextPlayerNumber);
                }

                if (Engine.ExtendedLogging) logger.Info($"Four handler, temp pause amount: {temporaryPauseAmount}, player, who started 4: {GetNumberOfPlayerThatStartedFours(PlayersCurrentData).ToString()}, block possibility of movement: {blockPossibilityOfTakingCard}");
            }

            return (amountOfFours > 0, justEndedFours);
        }

        //method for not puting four card on the table, when player has a possibility to do that
        public (bool, bool) PlayerWantsToStop(ref Dictionary<int, SinglePlayerData> PlayersCurrentData, ref GameStatus gameStatus,
            ref int temporaryPauseAmount, ref bool blockPossibilityOfTakingCard)
        {
            if (temporaryPauseAmount >= 1) temporaryPauseAmount--;

            bool justEndedFours;
            justEndedFours = AssigningStopsAlgorithm(ref PlayersCurrentData, ref gameStatus, ref temporaryPauseAmount, ref blockPossibilityOfTakingCard, playerNumber);
            return (false, justEndedFours);
        }

        //decrementing amount of stops for particular player
        public bool DecrementStopTurnsOfPlayer(ref Dictionary<int, SinglePlayerData> PlayersCurrentData, int currentPlayer, bool justFourEnded)
        {
            bool output = true;

            if (PlayersCurrentData[currentPlayer].PauseTurnsAmount > 0)
            {
                //if (!justFourEnded) PlayersCurrentData[currentPlayer].PauseTurnsAmount -= 1;
                PlayersCurrentData[currentPlayer].PauseTurnsAmount -= 1;
                output = false;
            }
            if (Engine.ExtendedLogging || justFourEnded) LogCurrentAmountOfStops(PlayersCurrentData);
            return output;
        }

        //public method for resetting first card took property for each player in game
        public static void ResetFirstCardTookProperties(ref Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            foreach (var item in PlayersCurrentData)
            {
                item.Value.TookFirstCardLostBattle = false;
            }
        }

        #endregion

        #region Internal methods

        private bool AssigningStopsAlgorithm(ref Dictionary<int, SinglePlayerData> PlayersCurrentData, ref GameStatus gameStatus,
            ref int temporaryPauseAmount, ref bool blockPossibilityOfTakingCard, int nextPlayerNumber)
        {
            AssignAmountOfStops(ref PlayersCurrentData, temporaryPauseAmount, nextPlayerNumber);
            temporaryPauseAmount = 0;
            ResetPlayerWhoStartedFoursProperties(ref PlayersCurrentData);
            SkippingTheMoveHandler(ref PlayersCurrentData, false, playerNumber);
            blockPossibilityOfTakingCard = false;
            ChangeGameStatus(ref gameStatus, false);

            return true;
        }

        /// <summary>
        /// setOrResetSkipping: set = true, reset = false
        /// </summary>
        /// <param name="PlayersCurrentData"></param>
        /// <param name="setOrResetSkipping"></param>
        /// <param name="playerNumber"></param>
        private void SkippingTheMoveHandler(ref Dictionary<int, SinglePlayerData> PlayersCurrentData, bool setOrResetSkipping, int playerNumber)
        {
            if(!setOrResetSkipping)
            {
                foreach (var item in PlayersCurrentData) item.Value.CanSkipTheMove = false;
            }
            else
            {
                foreach (var item in PlayersCurrentData) 
                {
                    if (item.Key == playerNumber) item.Value.CanSkipTheMove = true;
                    else item.Value.CanSkipTheMove = false;
                }
            }
        }

        private bool CheckIfAnyPlayerStartedFours(Dictionary<int, SinglePlayerData> CurrentPlayerData)
        {
            bool output = false;
            foreach (var item in CurrentPlayerData)
            {
                if (item.Value.ThisPlayerStartedFours)
                {
                    output = true;
                    break;
                }
            }
            return output;
        }

        private int GetNumberOfPlayerThatStartedFours (Dictionary<int,SinglePlayerData> CurrentPlayerData)
        {
            int output = -1000;
            try
            {
                output = CurrentPlayerData.Single(x => x.Value.ThisPlayerStartedFours == true).Key;
            }
            catch
            {
                if (Engine.ExtendedLogging) logger.Info($"No player started fours");
            }
            return output;
        }

        private void ResetPlayerWhoStartedFoursProperties(ref Dictionary<int, SinglePlayerData> CurrentPlayerData)
        {
            foreach (var item in CurrentPlayerData)
            {
                item.Value.ThisPlayerStartedFours = false;
            }
        }

        private void LogCurrentAmountOfStops(Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            logger.Info($"Logging amount of stops for each player - after decrementing method:");
            foreach (var item in PlayersCurrentData)
            {
                logger.Info($"Player with number: {item.Key.ToString()}, has amount of stops equal to: {item.Value.PauseTurnsAmount.ToString()}");
            }
        }

        //if game passed in is not four, throw an exception
        private int CheckIfAtLeastOnePassedCardIsFour()
        {
            int amountOfFours = 0;
            foreach (PlayingCard item in cards)
            {
                if (item.Rank == CardRanks.Four) amountOfFours++;
            }
            logger.Info($"Player {playerNumber} passed {amountOfFours} FOUR cards into the table.");
            return amountOfFours;
        }

        //method for checking if player has at least one four in his hands
        private bool CheckIfPlayerHasFourOrJokerInHand(List<PlayingCard> cardsList)
        {
            bool output = false;
            foreach (PlayingCard item in cardsList)
            {
                if (item.Rank == CardRanks.Four || item.Rank == CardRanks.Joker)
                {
                    output = true;
                    break;
                }
            }
            return output;
        }

        //method for changing the game status, if the card of rank four is
        //placed on the table
        private void ChangeGameStatus(ref GameStatus status, bool startFours)
        {
            if (startFours)
            {
                if (status == GameStatus.Battle) status = GameStatus.StopsAndBattle;
                else status = GameStatus.Stops;
            }
            else
            {
                if (status == GameStatus.StopsAndBattle) status = GameStatus.Battle;
                else status = GameStatus.Standard;
            }
            if(Engine.ExtendedLogging) logger.Info($"New game status: {status.ToString()}.");
        }

        //assigning amount of stops to specific player
        private void AssignAmountOfStops(ref Dictionary<int, SinglePlayerData> PlayersCurrentData, int amountOfTurnsToStop,
            int stopPlayerNumber)
        {
            PlayersCurrentData[stopPlayerNumber].PauseTurnsAmount += amountOfTurnsToStop;
            if (Engine.ExtendedLogging) logger.Info($"New amount of pause for player: {stopPlayerNumber.ToString()} = {PlayersCurrentData[stopPlayerNumber].PauseTurnsAmount.ToString()}.");
        }

        #endregion
    }
}
