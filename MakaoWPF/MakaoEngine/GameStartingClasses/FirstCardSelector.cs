using MakaoInterfaces;
using System.Collections.Generic;

namespace MakaoEngine.GameStartingClasses
{
    public class FirstCardSelector
    {
        public void PutFirstCardOnTheTable(ref List<PlayingCard> AlreadyUsedCards, ref List<PlayingCard> CurrentDeck, ref List<PlayingCard> CardsLatelyPutedOnTheTable)
        {
            bool cardIsOk = false;

            AlreadyUsedCards.Clear();
            CardsLatelyPutedOnTheTable.Clear();

            while (!cardIsOk)
            {
                //card cannot be put as fisrt card on the table
                PlayingCard firstCard = CurrentDeck[0];
                if (firstCard.Rank == CardRanks.Joker || firstCard.Rank == CardRanks.Ace || firstCard.Rank == CardRanks.King || firstCard.Rank == CardRanks.Queen ||
                    firstCard.Rank == CardRanks.Jack || firstCard.Rank == CardRanks.Four || firstCard.Rank == CardRanks.Three || firstCard.Rank == CardRanks.Two)
                {
                    //get the first card of the list, and put back on the bottom of it
                    CurrentDeck.RemoveAt(0);
                    CurrentDeck.Add(firstCard);
                }
                //card can be putted as first card on the table
                else
                {
                    CurrentDeck.RemoveAt(0);
                    AlreadyUsedCards.Insert(0, firstCard);
                    CardsLatelyPutedOnTheTable.Add(AlreadyUsedCards[0]);
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info("Card choosen as first on the table: " + firstCard.ToString());
                    cardIsOk = true;
                }
            }
        }
    }
}
