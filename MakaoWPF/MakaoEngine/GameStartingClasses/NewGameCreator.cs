using MakaoInterfaces;
using System;
using System.Collections.Generic;

namespace MakaoEngine.GameStartingClasses
{
    class NewGameCreator
    {
        #region Fields

        private readonly int decksInPlay;
        private readonly int jokersInDeck;
        private readonly int amountOfPlayers;
        private readonly int startCardsAmount;

        #endregion

        #region Constructor

        public NewGameCreator(int decksInPlay, int jokersInDeck, int amountOfPlayers, int startCardsAmount)
        {
            this.decksInPlay = decksInPlay;
            this.jokersInDeck = jokersInDeck;
            this.amountOfPlayers = amountOfPlayers;
            this.startCardsAmount = startCardsAmount;
        }

        #endregion

        #region Creation of new game mathods

        public void CreateGame(ref List<PlayingCard> CurrentDeck, ref Dictionary<int, SinglePlayerData> PlayersCurrentData,
            ref List<PlayingCard> AlreadyUsedCards, ref int currentPlayer, ref List<PlayingCard> CardsLatelyPutedOnTheTable)
        {
            CheckAmountOfCardsCorrectness();

            //Create the deck, shuffle cards, distribute them
            //to players and choose first card on the table

            //create deck
            DeckCreator Creator = new DeckCreator(decksInPlay, jokersInDeck);
            Creator.CreateDeck(ref CurrentDeck);

            //shuffle the deck
            DeckShuffler Shuffler = new DeckShuffler();
            Shuffler.ShuffleTheDeck(ref CurrentDeck);

            //cards to players distribution
            CardsDistributor Distributor = new CardsDistributor(amountOfPlayers, startCardsAmount);
            Distributor.DistributeCardsToPlayer(ref PlayersCurrentData, ref CurrentDeck);

            //put first card on the table
            FirstCardSelector FSelector = new FirstCardSelector();
            FSelector.PutFirstCardOnTheTable(ref AlreadyUsedCards, ref CurrentDeck, ref CardsLatelyPutedOnTheTable);

            //Choose first player (the starting one)
            FirstPlayerSelector Selector = new FirstPlayerSelector();
            Selector.ChooseFirstPlayerRandomly(amountOfPlayers, ref currentPlayer);
        }

        private void CheckAmountOfCardsCorrectness()
        {
            if (decksInPlay * (52 + jokersInDeck) < amountOfPlayers * startCardsAmount)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "There is no enough cards in deck to start the Play. Change creating game criteria.";
                logger.Error(text);
                throw new ArgumentException(text);
            }
        }

        #endregion
    }
}
