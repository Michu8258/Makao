using MakaoInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MakaoEngine.Disposing
{
    class DisposeTheEngine
    {
        public DisposeTheEngine(ref int jokersInDeck, ref int decksInPlay, ref int startCardsAmount, ref int amountOfPlayers,
            ref bool staticTest, ref List<PlayingCard> CurrentDeck,
            ref List<PlayingCard> AlreadyUsedCards, ref Dictionary<int, SinglePlayerData> PlayersCurrentData, ref CardRanks demandedRank, 
            ref CardSuits demandedSuit, ref int amountOfCardsToTake, ref GameStatus status)
        {
            jokersInDeck = 0;
            decksInPlay = 0;
            startCardsAmount = 0;
            amountOfPlayers = 0;
            staticTest = false;

            //inicialize lists
            CurrentDeck = null;
            AlreadyUsedCards = null;
            PlayersCurrentData = null;

            //For the game start no Suit nor rank is demanded
            demandedRank = CardRanks.None;
            demandedSuit = CardSuits.None;

            //at the start of game, amount of the cards to take as penalty for loosing the battle is 0
            amountOfCardsToTake = 0;

            // at the start, set the game status to default - standard
            status = GameStatus.Standard;
        }
    }
}
