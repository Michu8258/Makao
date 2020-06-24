using MakaoInterfaces;
using System;
using System.Collections.Generic;

namespace MakaoEngine.GameStartingClasses
{
    public class DeckCreator
    {
        #region Private fields

        private readonly int amountOfDecks;
        private readonly int amountOfJokers;

        #endregion

        #region Constructor

        public DeckCreator(int amountOfDecks, int amountOfJokers)
        {
            this.amountOfDecks = amountOfDecks;
            this.amountOfJokers = amountOfJokers;
        }

        #endregion

        #region Deck creation

        //public method for creating the proper deck
        public void CreateDeck(ref List<PlayingCard> CurrentDeck)
        {
            // in case of wrong constructor parameters, log it
            LogWrongInputParameters();

            //clear all cards in the deck
            CurrentDeck.Clear();

            //Generate cards and add them to list
            GenerateAllCards(ref CurrentDeck);
        }

        //if the parameters passed in the constructor are wrong, log it and
        //throw an exception
        private void LogWrongInputParameters()
        {
            if (amountOfDecks <= 0 || amountOfJokers > 3)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "Wrong input parameters while creating new deck";
                logger.Error(text);
                throw new ArgumentException(text);
            }
        }

        //algorithm method for sreating all necessary cards and
        //add them to the deck
        private void GenerateAllCards(ref List<PlayingCard> CurrentDeck)
        {
            for (int i = 0; i < amountOfDecks; i++)
            {
                //suits - foreach element in CardSuits enum
                foreach (CardSuits suit in (CardSuits[])Enum.GetValues(typeof(CardSuits)))
                {
                    GenerateNonJokerCards(suit, i, ref CurrentDeck);
                }

                GenerateJokerCards(i, ref CurrentDeck);
            }
        }

        //adding to deck all cards that are not joker
        private void GenerateNonJokerCards(CardSuits suit, int i, ref List<PlayingCard> CurrentDeck)
        {
            if (suit != CardSuits.None)
            {
                //ranks
                foreach (CardRanks rank in (CardRanks[])Enum.GetValues(typeof(CardRanks)))
                {
                    if (rank != CardRanks.Joker && rank != CardRanks.None)
                    {
                        PlayingCard Card = new PlayingCard(suit, rank, i + 1);
                        CurrentDeck.Add(Card);
                        //log card creation if file
                        var logger = NLog.LogManager.GetCurrentClassLogger();
                        logger.Info("Card created: " + Card.ToString());
                    }
                }
            }
        }

        //adding joker cards to the deck
        private void GenerateJokerCards(int i, ref List<PlayingCard> CurrentDeck)
        {
            //if amount of Jockers is greather than one
            if (amountOfJokers > 0)
            {
                for (int j = 0; j < amountOfJokers; j++)
                {
                    PlayingCard Card = new PlayingCard(CardSuits.None, CardRanks.Joker, i + 1);
                    CurrentDeck.Add(Card);
                    //log card creation if file
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info("Card created: " + Card.ToString());
                }
            }
        }

        #endregion
    }
}
