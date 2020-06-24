using MakaoInterfaces;
using System;
using System.Collections.Generic;

namespace MakaoEngine.GameStartingClasses
{
    public class CardsDistributor
    {
        #region Private fields

        private readonly int playersAmount;
        private readonly int cardsAmount;

        #endregion

        #region Constructor

        public CardsDistributor(int playersAmount, int cardsAmount = 5)
        {
            this.playersAmount = playersAmount;
            this.cardsAmount = cardsAmount;
        }

        #endregion

        #region Distribution of cards methods

        public void DistributeCardsToPlayer(ref Dictionary<int, SinglePlayerData> PlayersCurrentData,
            ref List<PlayingCard> CurrentDeck)
        {
            //check for parameters passed in constructor errors
            CheckForCardsAmountCorrectness();
            CheckForPlayersAmountCorrectness();

            //create lists for playersCards
            CreateNewPlayersCardsLists(ref PlayersCurrentData);

            //distribute cards to players
            DistributeCards(ref PlayersCurrentData, ref CurrentDeck);
        }

        //error with amount of cards
        private void CheckForCardsAmountCorrectness()
        {
            if (cardsAmount <= 0)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "Amount of cards is less than 1 - distributing cards";
                logger.Error(text);
                throw new ArgumentException(text);
            }
        }

        //error with amount of players
        private void CheckForPlayersAmountCorrectness()
        {
            if (playersAmount <= 1)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "Amount of players is less than 2 - distributing cards";
                logger.Error(text);
                throw new ArgumentException(text);
            }
        }

        //delete all existing players and add ampty list of cards for every
        //player to GamersCards list
        private void CreateNewPlayersCardsLists(ref Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            //creation of players
            for (int i = 0; i < PlayersCurrentData.Count; i++)
            {
                PlayersCurrentData[i].PlayerCards.Clear();
            }
        }

        //distribute cards to players by adding cards to lista of players cards
        //and removing from the deck list
        private void DistributeCards(ref Dictionary<int, SinglePlayerData> PlayersCurrentData, 
            ref List<PlayingCard> CurrentDeck)
        {
            for (int i = 0; i < cardsAmount; i++)
            {
                for (int j = 0; j < playersAmount; j++)
                {
                    PlayersCurrentData[j].PlayerCards.Add(CurrentDeck[0]);
                    CurrentDeck.RemoveAt(0);
                }

                //log new amount of cards (after distributing cards to one player
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Each player got one new card from Deck. Cadrs in deck: " + CurrentDeck.Count.ToString());
            }
        }

        #endregion
    }
}
