using MakaoEngine.CardCorectnessChecking;
using MakaoInterfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MakaoEngine.MatchingCardsFinding
{
    public class MatchingCardsFinder
    {
        #region Private fields

        private readonly int playerNumber;
        private readonly PlayingCard card;
        private readonly PlayingCard topCard;
        private readonly List<PlayingCard> playerCards;
        private readonly bool staticTest;
        private readonly CardRanks demandedRank;
        private readonly CardSuits demandedSuit;
        private readonly GameStatus status;
        private readonly Logger logger;

        #endregion

        #region Constructor

        public MatchingCardsFinder(int playerNumber, PlayingCard card, bool staticTest,
            PlayingCard topCard, List<PlayingCard> playerCards, CardRanks demandedRank,
            CardSuits demandedSuit, GameStatus status)
        {
            this.playerNumber = playerNumber;
            this.card = card;
            this.topCard = topCard;
            this.playerCards = playerCards;
            this.staticTest = staticTest;
            this.demandedRank = demandedRank;
            this.demandedSuit = demandedSuit;
            this.status = status;

            logger = NLog.LogManager.GetCurrentClassLogger();
            if (Engine.ExtendedLogging) logger.Info($"Matching cards finder class constructed. Putted card: {card.ToString()}, top card: {topCard.ToString()}.");
        }

        #endregion

        #region Main method - public (algorithm)

        //method for calling from engine
        public List<FamiliarCardsData> FindMatchingCardsInPlayerHand(ref List<PlayingCard> AlreadyUsedCards,
            ref Dictionary<int, SinglePlayerData> GamersCards)
        {
            if (Engine.ExtendedLogging) logger.Info($"Finder of familiard cards method fired from host");

            //if the caling is for test purposes, add fake cards to list
            HandleStaticTest(ref AlreadyUsedCards, ref GamersCards);

            //check if passed card can be placed on the top card on the table
            CheckPassedCardCorrectness();

            //checking if user trully has such a card in his hand
            int inputCardIndex = CheckIfPLayerOwnsACard(ref GamersCards);

            //now the checking procedure
            List<FamiliarCardsData> FamiliarCardsData = GenerateOutputData(inputCardIndex);

            return (FamiliarCardsData);
        }

        //method for calling from client
        public List<FamiliarCardsData> FindMatchingCardsInPlayerHand()
        {
            if (Engine.ExtendedLogging) logger.Info($"Finder of familiard cards method fired from client");

            //check if passed card can be placed on the top card on the table
            CheckPassedCardCorrectness();

            //checking if user trully has such a card in his hand
            int inputCardIndex = CheckIfPLayerOwnsACard(playerCards);

            //now the checking procedure
            List<FamiliarCardsData> FamiliarCardsData = GenerateOutputData(inputCardIndex);

            return (FamiliarCardsData);
        }

        #endregion

        #region Internal private methods

        //adding fake cards to list
        private void HandleStaticTest(ref List<PlayingCard> AlreadyUsedCards,
            ref Dictionary<int, SinglePlayerData> GamersCards)
        {
            if (staticTest)
            {
                logger.Info($"Static test fired");
                AlreadyUsedCards[0] = new PlayingCard(CardSuits.Spade, CardRanks.Five, 1);
                GamersCards[0].PlayerCards[0] = new PlayingCard(CardSuits.Spade, CardRanks.Seven, 1);
                GamersCards[0].PlayerCards[1] = new PlayingCard(CardSuits.Heart, CardRanks.Seven, 1);
                GamersCards[0].PlayerCards[2] = new PlayingCard(CardSuits.Diamond, CardRanks.Seven, 1);
                GamersCards[0].PlayerCards[3] = new PlayingCard(CardSuits.Club, CardRanks.Seven, 1);
                GamersCards[0].PlayerCards[4] = new PlayingCard(CardSuits.Spade, CardRanks.Seven, 2);
            }
        }

        //check if the card passed in in the constructor parameters
        //can be placed on the table
        private void CheckPassedCardCorrectness()
        {
            CardCorrectnessChecker Checker = new CardCorrectnessChecker(card, topCard, demandedRank, demandedSuit, status);
            bool cardIsCorrect = Checker.CanTheCardBePlacedOnTheTable();

            if (Engine.ExtendedLogging) logger.Info($"Finder check if the passed card can be placed on the table: {cardIsCorrect}.");

            if (!cardIsCorrect)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "The card: " + card.ToString() + " can not be placed on the last card";
                logger.Error(text);
                throw new ArgumentException(text);
            }
        }

        //method for checking if playr has really a card in his hand
        private int CheckIfPLayerOwnsACard(ref Dictionary<int, SinglePlayerData> GamersCards)
        {
            CardPossessingChecker Checker = new CardPossessingChecker();
            int output;
            (_, output) = Checker.CheckIfPlayerHasDefinedCardInHand(card, playerNumber, ref GamersCards);
            if (output < 0) throw new ArgumentException("Player has no such a card in his cards collection");
            return output;
        }

        //method for checking if playr has really a card in his hand
        private int CheckIfPLayerOwnsACard(List<PlayingCard> PlayerCards)
        {
            try
            {
                PlayingCard tempCard = PlayerCards.Single(x => x.CompareTo(card) == 0);
                int output = PlayerCards.IndexOf(tempCard);

                if (Engine.ExtendedLogging) logger.Info($"The player {playerNumber} has {tempCard.ToString()} in his hands.");

                return output;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Player has no such a card in his cards collection: {ex.Message}.");
            }
        }

        //method for creating an output data list
        private List<FamiliarCardsData> GenerateOutputData(int inputCardIndex)
        {
            List<FamiliarCardsData> FamiliarCardsData = new List<FamiliarCardsData>();
            List<PlayingCard> LogList = new List<PlayingCard>();

            foreach (PlayingCard item in playerCards)
            {
                if (item.Rank == card.Rank && playerCards.IndexOf(item) != inputCardIndex)
                {
                    CardCorrectnessChecker Checker = new CardCorrectnessChecker(item, topCard, demandedRank, demandedSuit, status);
                    bool canBeFirst = Checker.CanTheCardBePlacedOnTheTable();

                    FamiliarCardsData oneCardData = new FamiliarCardsData()
                    {
                        Card = item,
                        CanBePlacedAsFirstCard = canBeFirst,
                    };

                    FamiliarCardsData.Add(oneCardData);
                    LogList.Add(item);
                }
            }

            if (Engine.ExtendedLogging) CardListLogger.LogCardsList(LogList, "Founded familiar card in player's hands");

            return FamiliarCardsData;
        }

        #endregion
    }
}
