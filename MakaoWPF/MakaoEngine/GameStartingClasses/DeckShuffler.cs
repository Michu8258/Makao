using MakaoInterfaces;
using System.Collections.Generic;

namespace MakaoEngine.GameStartingClasses
{
    public class DeckShuffler
    {
        public void ShuffleTheDeck(ref List<PlayingCard> CurrentDeck)
        {
            CurrentDeck.Shuffle();
            {
                //log card creation if file
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Current deck shuffeled, total: " + CurrentDeck.Count.ToString() + " cards");
            }
        }
    }
}
