using MakaoInterfaces;
using System;
using System.Collections.Generic;

namespace MakaoEngine.JokerDealing
{
    public class JokerRetriever
    {
        private readonly int playerNumber;
        private readonly PlayingCard card;

        public JokerRetriever(int playerNumber, PlayingCard card)
        {
            this.playerNumber = playerNumber;
            this.card = card;
        }

        //method for returning the card to Jocker back
        public bool ChangeCardsIntoJockersBack(ref Dictionary<int, SinglePlayerData> GamersCards)
        {
            bool cardChanged;
            try
            {
                ChangeJokerBack(card, ref GamersCards);
                cardChanged = true;
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error(ex.Message);
                cardChanged = false;
            }

            return cardChanged;
        }

        //method for actually changing some card back to joker
        private void ChangeJokerBack(PlayingCard newCard, ref Dictionary<int, SinglePlayerData> GamersCards)
        {
            for (int i = 0; i < GamersCards[playerNumber].PlayerCards.Count; i++)
            {
                if (GamersCards[playerNumber].PlayerCards[i].CompareTo(newCard) == 0)
                {
                    PlayingCard item = GamersCards[playerNumber].PlayerCards[i];
                    item.ChangeCardBackToJocker();
                    GamersCards[playerNumber].PlayerCards[i] = item;
                    break;
                }
            }
        }
    }
}
