using MakaoInterfaces;
using NLog;
using System;

namespace MakaoEngine.CardCorectnessChecking
{
    public class CardCorrectnessChecker
    {
        #region Provate fields

        private readonly PlayingCard cardToCheck;
        private readonly PlayingCard topCard;
        private readonly CardRanks demandedRank;
        private readonly CardSuits demandedSuit;
        private readonly GameStatus status;
        private readonly Logger logger;

        #endregion

        #region Constructor

        public CardCorrectnessChecker(PlayingCard cardToCheck, PlayingCard topCard,
            CardRanks demandedRank, CardSuits demandedSuit, GameStatus status)
        {
            this.cardToCheck = cardToCheck;
            this.topCard = topCard;
            this.demandedRank = demandedRank;
            this.demandedSuit = demandedSuit;
            this.status = status;
            logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Checking correctness methods

        //method for checking correctnes depending on game status
        public bool CanTheCardBePlacedOnTheTable()
        {
            bool theCardIsCorrect;

            if (Engine.ExtendedLogging) logger.Info($"Card correctnes method fired up. Current game status is: {status.ToString()}.");

            switch (status)
            {
                case GameStatus.Standard:
                    theCardIsCorrect = CheckCorectnessInStandardMode(cardToCheck, topCard);
                    break;
                case GameStatus.Battle:
                    theCardIsCorrect = CheckCorectnessInBattleMode(cardToCheck, topCard);
                    break;
                case GameStatus.Stops:
                    theCardIsCorrect = CheckCorectnessInStopsMode(cardToCheck, topCard);
                    break;
                case GameStatus.StopsAndBattle:
                    theCardIsCorrect = CheckCorectnessInStopsANdBattleMode(cardToCheck, topCard);
                    break;
                case GameStatus.RankDemanding:
                    theCardIsCorrect = CheckCorectnessInRankDemandingMode(cardToCheck, topCard);
                    break;
                case GameStatus.SuitDemanding:
                    theCardIsCorrect = CheckCorectnessInSuitDemandingMode(cardToCheck, topCard);
                    break;
                default:
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    string text = "Unexpected game mode";
                    logger.Error(text);
                    throw new ArgumentException(text);
            }

            if (Engine.ExtendedLogging)
            {
                switch (theCardIsCorrect)
                {
                    case true: logger.Info($"The card: {cardToCheck.ToString()} can be placed on: {topCard.ToString()}. OK!"); break;
                    case false: logger.Info($"The card: {cardToCheck.ToString()} can NOT be placed on: {topCard.ToString()}. ERROR!");  break;
                }
            }

            return theCardIsCorrect;
        }

        //Stadard mode
        private static bool CheckCorectnessInStandardMode(PlayingCard newCard, PlayingCard topCard)
        {
            if (newCard.Rank == topCard.Rank || newCard.Suit == topCard.Suit) return true;
            else return false;
        }

        //corectness of the card when there is Suit demanding
        private bool CheckCorectnessInSuitDemandingMode(PlayingCard card, PlayingCard topCard)
        {
            if ((topCard.Rank == CardRanks.Ace && card.Rank == CardRanks.Ace) ||
                (card.Suit == demandedSuit)) return true;
            else return false;
        }

        //corectness of the card when there is Rank demanding
        private bool CheckCorectnessInRankDemandingMode(PlayingCard card, PlayingCard topCard)
        {
            if ((topCard.Rank == CardRanks.Jack && card.Rank == CardRanks.Jack) ||
                (card.Rank == demandedRank)) return true;
            else return false;
        }

        //battle mode
        private bool CheckCorectnessInBattleMode(PlayingCard card, PlayingCard topCard)
        {
            bool retVal;

            switch (card.Rank)
            {
                case CardRanks.Two:
                case CardRanks.Three:
                case CardRanks.King:
                    if (topCard.IsBrave == true && (topCard.Rank == card.Rank || topCard.Suit == card.Suit)) retVal = true;
                    else retVal = false;
                    if (card.Rank == CardRanks.King && (card.Suit == CardSuits.Diamond || card.Suit == CardSuits.Club)) retVal = false;
                    break;
                default:
                    retVal = false;
                    break;
            }

            return retVal;
        }

        //four cards
        private bool CheckCorectnessInStopsMode(PlayingCard card, PlayingCard topCard)
        {
            if (topCard.Rank == CardRanks.Four && card.Rank == CardRanks.Four) return true;
            else return false;
        }

        //stops and battle at the same time mode
        private bool CheckCorectnessInStopsANdBattleMode(PlayingCard card, PlayingCard topCard)
        {
            return CheckCorectnessInBattleMode(card, topCard);
        }

        #endregion
    }
}
