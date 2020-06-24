using MakaoInterfaces;
using System;
using System.Collections.Generic;

namespace MakaoEngine.JokerDealing
{
    public class JokerChanger
    {
        private readonly int playerNumber;
        private readonly int curremtPlayerNumber;
        private readonly PlayingCard card;
        private readonly CardRanks newRank;
        private readonly CardSuits newSuit;
        private readonly bool calledByEngine;

        public JokerChanger(int playerNumber, PlayingCard card, CardRanks newRank, CardSuits newSuit,
            int curremtPlayerNumber, bool calledByEngine)
        {
            this.playerNumber = playerNumber;
            this.curremtPlayerNumber = curremtPlayerNumber;
            this.card = card;
            this.newRank = newRank;
            this.newSuit = newSuit;
            this.calledByEngine = calledByEngine;
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"Joker changed class - constructor. Passed card created by joker: {card.CreatedByJocker}.");
        }

        //change one jocker card into any other card
        public bool ChangeJockerIntoAnotherCard(ref Dictionary<int, SinglePlayerData> GamersCards)
        {
            bool cardChanged;
            var logger = NLog.LogManager.GetCurrentClassLogger();

            //protection of change of joker if requesting player is not the current player
            //and ezecute
            if ((playerNumber == curremtPlayerNumber) || calledByEngine) //protection of change of joker if requesting player
                //is not the current player
            {
                try
                {
                    logger.Info($"Old card: {card.ToString()}. + {card.CreatedByJocker}.");
                    ChangeJoker(playerNumber, newRank, newSuit, ref GamersCards);
                    cardChanged = true;
                    logger.Info("Joker changing into another card method try block executed.");
                }
                catch (Exception ex)
                {
                    logger.Error($"Error occured why trying to change joker into another card: {ex.Message}.");
                    cardChanged = false;
                }

                return cardChanged;
            }
            else return false;
        }

        //method for actually changing card in engines
        //list with cards from joker to aonther card
        private void ChangeJoker(int playerNumber,CardRanks newRank, CardSuits newSuit,
            ref Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            for (int i = 0; i < PlayersCurrentData[playerNumber].PlayerCards.Count; i++)
            {
                if (PlayersCurrentData[playerNumber].PlayerCards[i].CompareTo(card) == 0)
                {
                    //PlayingCard item = GamersCards[playerNumber][i];
                    //item.ChangeCardFromJocker(newSuit, newRank);
                    PlayersCurrentData[playerNumber].PlayerCards[i].ChangeCardFromJocker(newSuit, newRank);
                    logger.Info($"Found matching card in player: {playerNumber} cards. New card is {PlayersCurrentData[playerNumber].PlayerCards[i].ToString()}.");
                    break;
                }
            }
        }
    }
}
