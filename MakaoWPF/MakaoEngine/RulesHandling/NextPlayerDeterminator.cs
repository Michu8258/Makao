using MakaoInterfaces;
using NLog;
using System.Collections.Generic;

namespace MakaoEngine.RulesHandling
{
    public class NextPlayerDeterminator
    {
        private readonly Logger logger;

        public NextPlayerDeterminator()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        public int DetermineNextPlayerNumber(PlayingCard card, int currentPlayerNumber, int totalAmountOfPLayers,
            Dictionary <int, SinglePlayerData> PlayersCurrentData)
        {
            //0 - forward; 1 - backward
            int direction = ChangeDirectionOfMove(card);
            if (Engine.ExtendedLogging) logger.Info($"Next player determination direction: {direction.ToString()}.");

            int output;
            if (direction == 0) output = NextPlayerForward(currentPlayerNumber, totalAmountOfPLayers, PlayersCurrentData);
            else output = NextPlayerBackward(currentPlayerNumber, totalAmountOfPLayers, PlayersCurrentData);
            if (Engine.ExtendedLogging) logger.Info($"Player number passed in: {currentPlayerNumber}, new number: {output}.");

            return output;
        }

        private int NextPlayerForward(int currentPlayerNumber, int totalAmountOfPLayers, Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            int output = currentPlayerNumber;
            bool nextPlayerNumberOK = false;
            int internalCurrentPlayerNumber = currentPlayerNumber;

            while (!nextPlayerNumberOK)
            {
                //last player
                if (internalCurrentPlayerNumber == totalAmountOfPLayers - 1) output = 0;
                //not last player
                else output = internalCurrentPlayerNumber + 1;

                //check if next player did finish the game
                if (PlayersCurrentData[output].PlayerCards.Count == 0) nextPlayerNumberOK = false;
                else nextPlayerNumberOK = true;
            }

            return output;
        }

        private int NextPlayerBackward(int currentPlayerNumber, int totalAmountOfPLayers, Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            int output = currentPlayerNumber;
            bool nextPlayerNumberOK = false;
            int internalCurrentPlayerNumber = currentPlayerNumber;

            while(!nextPlayerNumberOK)
            {
                //first player
                if (internalCurrentPlayerNumber == 0) output = totalAmountOfPLayers - 1;
                //not first player
                else output = internalCurrentPlayerNumber - 1;

                //check if next player did finish the game
                if (PlayersCurrentData[output].PlayerCards.Count == 0) nextPlayerNumberOK = false;
                else nextPlayerNumberOK = true;
            }

            return output;
        }

        //check the direction of the move
        private int ChangeDirectionOfMove(PlayingCard card)
        {
            if (card.NextMove == CardMoveDirections.Backward) return 1;
            else return 0;
            //0 - forward, 1 - backward
        }
    }
}
