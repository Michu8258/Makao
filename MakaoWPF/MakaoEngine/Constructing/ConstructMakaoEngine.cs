using MakaoInterfaces;
using System;
using System.Collections.Generic;

namespace MakaoEngine.Constructing
{
    class ConstructMakaoEngine
    {
        //constructor only
        public ConstructMakaoEngine(int players, int decks, int jokers, bool test, int amountOfCards, ref int jokersInDeck,
            ref int decksInPlay, ref int startCardsAmount, ref int amountOfPlayers, ref bool staticTest, ref List<PlayingCard> CurrentDeck,
                ref List<PlayingCard> AlreadyUsedCards, ref Dictionary<int, SinglePlayerData> PlayersCurrentData,
                ref CardRanks demandedRank, ref CardSuits demandedSuit, ref int amountOfCardsToTake, ref GameStatus status,
                ref int temporaryPauseAmount, ref Dictionary<int, int> FinishedPlayers, ref List<PlayingCard> CardsLatelyPutedOnTheTable)
        {
            try
            {
                //assigning number od player who started puting fours in the table
                temporaryPauseAmount = 0;

                //NlogSourcePath = source;
                jokersInDeck = jokers;
                decksInPlay = decks;
                startCardsAmount = amountOfCards;
                amountOfPlayers = players;
                staticTest = test;

                //inicialize lists
                CurrentDeck = new List<PlayingCard>();
                AlreadyUsedCards = new List<PlayingCard>();
                PlayersCurrentData = new Dictionary<int, SinglePlayerData>();
                FinishedPlayers = new Dictionary<int, int>();
                CardsLatelyPutedOnTheTable = new List<PlayingCard>();

                //For the game start no Suit nor rank is demanded
                demandedRank = CardRanks.None;
                demandedSuit = CardSuits.None;

                //at the start of game, amount of the cards to take as penalty for loosing the battle is 0
                amountOfCardsToTake = 0;

                //NLOG - full method for testing purposes
                LogConstructionOfEngine(amountOfPlayers, decksInPlay, jokersInDeck, startCardsAmount);

                // at the start, set the game status to default - standard
                status = GameStatus.Standard;

                //handling player cannot move while stopped by card with rank: four
                PlayersCurrentData.Clear();
                for (int i = 0; i < players; i++)
                {
                    SinglePlayerData newPlayerData = new SinglePlayerData()
                    {
                        PauseTurnsAmount = 0,
                        TookFirstCardLostBattle = false,
                        ThisPlayerStartedRankDemanding = false,
                        ThisPlayerStartedSuitDemanding = false,
                        PlayerCards = new List<PlayingCard>(),
                        ThisPlayerStartedFours = false,
                        CanSkipTheMove = false,
                        FirstCardInBattleModeTakenMatches = false,
                        BattleModeMatchingCard = null,
                    };
                    PlayersCurrentData.Add(i, newPlayerData);
                }
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Creation of the game engine failed: " + ex.Message);
            }
        }

        //method for configuration of Nloger
        private void LogConstructionOfEngine(int amountOfPlayers, int decksInPlay, int jokersInDeck, int cards)
        {
            //log construction of the engine
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Engine constructed. Players: " + amountOfPlayers + ", decks: " + decksInPlay + ", jockers: " + jokersInDeck
                 + ", cards: " + cards);
        }
    }
}
