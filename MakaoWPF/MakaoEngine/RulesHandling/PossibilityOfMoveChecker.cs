using MakaoEngine.CardCorectnessChecking;
using MakaoInterfaces;
using System.Collections.Generic;

namespace MakaoEngine.RulesHandling
{
    public class PossibilityOfMoveChecker
    {
        //if user has no matching card or there is no card to take, skip next player
        public bool CheckPotentialiTyOfMove(int playerNumber, Dictionary<int, SinglePlayerData> PlayersCurrentData,
            int currentPlayer, PlayingCard topCard,
            CardRanks demandedRank, CardSuits demandedSuit, GameStatus status)
        {
            bool canMakeMove = false;
            var logger = NLog.LogManager.GetCurrentClassLogger();

            if (currentPlayer == playerNumber)
            {
                if(Engine.ExtendedLogging) logger.Info($"Potentiality checker: player number ok.");

                //first - check player cards
                foreach (PlayingCard item in PlayersCurrentData[playerNumber].PlayerCards)
                {
                    CardCorrectnessChecker Checker = new CardCorrectnessChecker(item, topCard,
                        demandedRank, demandedSuit, status);
                    canMakeMove = Checker.CanTheCardBePlacedOnTheTable();
                    if (canMakeMove) break;
                }

                if (Engine.ExtendedLogging) logger.Info($"Potentiality checker: can make move before pause amount checking: {canMakeMove}.");

                //check if player do not have to pause
                if (PlayersCurrentData[playerNumber].PauseTurnsAmount > 0)
                {
                    canMakeMove = false;
                }

                if (Engine.ExtendedLogging) logger.Info($"Potentiality checker: cfinal possibility: {canMakeMove}.");
            }

            return canMakeMove;
        }
    }
}
