using MakaoInterfaces;
using NLog;
using System.Collections.Generic;

namespace MakaoEngine.RulesHandling
{
    public class BattleCardsCounter
    {
        private readonly Logger logger;

        public BattleCardsCounter()
        {
            logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public void AddCardsToTakeByPlayerThatLostBattle(PlayingCard card, ref int amountOfCardsToTake,
             ref GameStatus status, bool lastCard)

        {
            if (Engine.ExtendedLogging) logger.Info($"Current amount of cards to take: {amountOfCardsToTake}.");

            switch (card.Rank)
            {
                case CardRanks.Two:
                    amountOfCardsToTake += 2;
                    break;
                case CardRanks.Three:
                    amountOfCardsToTake += 3;
                    break;
                case CardRanks.King:
                    if (card.Suit == CardSuits.Spade || card.Suit == CardSuits.Heart) amountOfCardsToTake += 5;
                    else if (card.Suit == CardSuits.Club || card.Suit == CardSuits.Diamond) amountOfCardsToTake = 0;
                    break;
            }

            if (Engine.ExtendedLogging) logger.Info($"Amount of cards to take after adding new cards to the table: {amountOfCardsToTake}.");

            //status
            if (lastCard) ChangeGameStatusToBattleStatus(amountOfCardsToTake, ref status);
        }


        //method for changing game status to battle status
        private void ChangeGameStatusToBattleStatus(int amountOfCardsToTake, ref GameStatus status)
        {
            if (Engine.ExtendedLogging) logger.Info($"Current game status: {status.ToString()}.");

            if ((amountOfCardsToTake > 0) && (status != GameStatus.Battle))
            {
                //if (AreStops(PlayersCurrentData)) status = GameStatus.StopsAndBattle;
                //else status = GameStatus.Battle;
                status = GameStatus.Battle;

                if (Engine.ExtendedLogging) logger.Info($"New game status: {status.ToString()}.");
            }
            else if(amountOfCardsToTake == 0 && status != GameStatus.RankDemanding && status != GameStatus.SuitDemanding)
            {
                //if (AreStops(PlayersCurrentData)) status = GameStatus.Stops;
                //else status = GameStatus.Standard;
                status = GameStatus.Standard;
            }
        }

        //method for checking if there are active card of rank 4 obstacles
        //private bool AreStops(Dictionary<int, SinglePlayerData> PlayersCurrentData)
        //{
        //    bool stops = false;
        //    for (int i = 0; i < PlayersCurrentData.Count; i++)
        //    {
        //        if (PlayersCurrentData[i].PauseTurnsAmount > 0)
        //        {
        //            stops = true;
        //            break;
        //        }
        //    }
        //    if (Engine.ExtendedLogging) logger.Info($"Has at least one of players stop to make: {stops}.");
        //    return stops;
        //}
    }
}
