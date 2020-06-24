using MakaoInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MakaoEngine.MatchingCardsFinding
{
    public class CardPossessingChecker
    {
        //finding if player has defined card in his hand
        public  (bool, int) CheckIfPlayerHasDefinedCardInHand(PlayingCard card, int playerNumber,
            ref Dictionary<int, SinglePlayerData> GamersCards)
        {
            bool cardInPlayersHand = false;

            //index of card in the list
            int inputCardNumber = -5;

            try
            {
                //checking if player have such a card in his hands
                PlayingCard tempCard = GamersCards[playerNumber].PlayerCards.Single(x => x.CompareTo(card) == 0);
                cardInPlayersHand = true;
                inputCardNumber = GamersCards[playerNumber].PlayerCards.IndexOf(tempCard);

                if (Engine.ExtendedLogging) LogCardPosessing(playerNumber, cardInPlayersHand, tempCard);
            }
            catch (Exception ex)
            {
                if (Engine.ExtendedLogging) LogCardPosessing(playerNumber, false, card);

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Player { playerNumber.ToString()} does not have { card.ToString()} in his hand: {ex.Message}.");
                throw new ArgumentException("PlayerNumber doeas not have such a card in his hands");
            }

            return (cardInPlayersHand, inputCardNumber);
        }

        //method for extended logging
        private void LogCardPosessing(int playerNumber, bool doesHave, PlayingCard card)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            if (doesHave) logger.Info($"Player {playerNumber} does have the card: {card.ToString()}, in hos hands.");
            else logger.Info($"Player {playerNumber} does not have card: {card.ToString()}, in his hands.");
        }
    }
}
