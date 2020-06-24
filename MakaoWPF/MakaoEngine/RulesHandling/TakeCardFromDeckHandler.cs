using MakaoEngine.CardCorectnessChecking;
using MakaoInterfaces;
using NLog;
using System;
using System.Collections.Generic;

namespace MakaoEngine.RulesHandling
{
    public class TakeCardFromDeckHandler
    {
        private readonly Logger logger;

        public TakeCardFromDeckHandler()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        #region Public main methods

        public (bool, bool) TakeCardsFromDeck(int playerNumber, ref GameStatus status, ref List<PlayingCard> AlreadyUsedCards,
            PlayingCard FirstCardInDeck, ref Dictionary<int, SinglePlayerData> PlayersCurrentData,
            ref int amountOfCardsToTake, PlayingCard firstAlreadyUsedCard, CardRanks demandedRank, CardSuits demandedSuit,
            ref List<PlayingCard> CurrentDeck, int jokersInDeck, int decksInPlay)
        {
            bool keepMoveOfThisPlayer = false;
            bool cardsTaken;

            //reseting battle matching cards
            ResetMatchingCardsInBattleMode(ref PlayersCurrentData);

            if (status == GameStatus.Battle || status == GameStatus.StopsAndBattle)
            {
                if (Engine.ExtendedLogging) logger.Info($"Taking card from deck by the player: {playerNumber.ToString()}. Status: {status.ToString()}.");

                List<PlayingCard> tempList = new List<PlayingCard>() { FirstCardInDeck };
                bool firstCardInDeckCanBePlacedOnTable = CheckCardCorrectness(tempList, firstAlreadyUsedCard,
                demandedRank, demandedSuit, status) || FirstCardInDeck.Rank == CardRanks.Joker;
                //if there is possible move, player takes only one card, if not, he takes all cards
                if (firstCardInDeckCanBePlacedOnTable && PlayersCurrentData[playerNumber].TookFirstCardLostBattle == false)
                {
                    /*
                    PlayersCurrentData[playerNumber].FirstCardInBattleModeTakenMatches = true;
                    PlayersCurrentData[playerNumber].BattleModeMatchingCard = new PlayingCard(CardSuits.None, CardRanks.Joker, 1);
                    PlayersCurrentData[playerNumber].PlayerCards.Add(new PlayingCard(CardSuits.None, CardRanks.Joker, 1));
                    PlayersCurrentData[playerNumber].TookFirstCardLostBattle = true;
                    cardsTaken = true;
                    keepMoveOfThisPlayer = true;
                    */
                    
                    PlayersCurrentData[playerNumber].FirstCardInBattleModeTakenMatches = true;
                    PlayersCurrentData[playerNumber].BattleModeMatchingCard = FirstCardInDeck;
                    keepMoveOfThisPlayer = true;
                    cardsTaken = TakeCardFromDeckBasic(playerNumber, 1, ref AlreadyUsedCards, ref CurrentDeck, jokersInDeck,
                        decksInPlay, ref PlayersCurrentData);
                    PlayersCurrentData[playerNumber].TookFirstCardLostBattle = true;

                    if (Engine.ExtendedLogging) logger.Info($"Player {playerNumber.ToString()} takes one card. The first card in deck had battle power and he didn't take card before.");
                }
                else
                {
                    cardsTaken = TakeCardFromDeckBasic(playerNumber, amountOfCardsToTake, ref AlreadyUsedCards, ref CurrentDeck, jokersInDeck,
                        decksInPlay, ref PlayersCurrentData);
                    ResetAllPlayersLostBattleCardsToTake(ref PlayersCurrentData);
                    ChangeGameStatusAfterBattle(ref status);
                    amountOfCardsToTake = 0;

                    if (Engine.ExtendedLogging) logger.Info($"Player {playerNumber.ToString()} takes all of the cards.");
                }
            }
            else
            {
                if (Engine.ExtendedLogging) logger.Info($"Taking one card from deck, because game status is: {status.ToString()}.");
                List<PlayingCard> tempList = new List<PlayingCard>() { FirstCardInDeck };

                bool firstCardInDeckCanBePlacedOnTable = CheckCardCorrectness(tempList, firstAlreadyUsedCard,
                demandedRank, demandedSuit, status) || FirstCardInDeck.Rank == CardRanks.Joker;

                if (firstCardInDeckCanBePlacedOnTable)
                {
                    PlayersCurrentData[playerNumber].FirstCardInBattleModeTakenMatches = true;
                    PlayersCurrentData[playerNumber].BattleModeMatchingCard = FirstCardInDeck;
                    keepMoveOfThisPlayer = true;
                }

                cardsTaken = TakeCardFromDeckBasic(playerNumber, 1, ref AlreadyUsedCards, ref CurrentDeck, jokersInDeck,
                            decksInPlay, ref PlayersCurrentData);
            }

            return (cardsTaken, keepMoveOfThisPlayer);
        }

        //method for retrieving cards from table to the deck
        public void RestoreCardsFromTableToDeck(ref List<PlayingCard> AlreadyUsedCards,
            ref List<PlayingCard> CurrentDeck)
        {
            List<PlayingCard> tempList = new List<PlayingCard>();
            foreach (PlayingCard item in AlreadyUsedCards)
            {
                tempList.Add(item);
            }

            //changing joker cards turned into another, back to jokers
            foreach (PlayingCard item in tempList)
            {
                if (item.CreatedByJocker) item.ChangeCardBackToJocker();
                if(Engine.ExtendedLogging) logger.Info($"Found card created by joker in already used cards list. No created by joker property is: {item.CreatedByJocker}.");
            }
            tempList.RemoveAt(0);
            tempList.Shuffle();
            CurrentDeck.AddRange(tempList);
            AlreadyUsedCards.RemoveRange(1, AlreadyUsedCards.Count - 1);

            if (Engine.ExtendedLogging) logger.Info($"Used cardst restored and added to deck.");
        }

        #endregion

        #region Internal methods

        //if player needs to take more cards than there are available, there will be thrown
        //an exception, but after allocating as much cards as it is possible to the player
        //the exception is rather for information and will need to be handled in application
        //I mean in the grafics representation - information for user or something

        private bool TakeCardFromDeckBasic(int playerNumber, int amountOfCards, ref List<PlayingCard> AlreadyUsedCards,
            ref List<PlayingCard> CurrentDeck, int jokersInDeck, int decksInPlay, ref Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            bool cardsTaken = false;
            int dontCountCards = 0;

            //check input data - if it is correct
            CheckCorrectnessOfPassedData(amountOfCards, AlreadyUsedCards, CurrentDeck, jokersInDeck, decksInPlay);

            //if there is not enough cards in deck, restore cards from the table
            if (amountOfCards > CurrentDeck.Count) RestoreCardsFromTableToDeck(ref AlreadyUsedCards, ref CurrentDeck);

            if (PlayersCurrentData[playerNumber].TookFirstCardLostBattle) dontCountCards = 1;

            //proper functionality
            for (int i = 0; i < amountOfCards - dontCountCards; i++)
            {
                if (CurrentDeck.Count > 0)
                {
                    if (PlayersCurrentData[playerNumber].FirstCardInBattleModeTakenMatches) PlayersCurrentData[playerNumber].BattleModeMatchingCard = CurrentDeck[0];
                    PlayersCurrentData[playerNumber].PlayerCards.Add(CurrentDeck[0]);
                    CurrentDeck.RemoveAt(0);
                    cardsTaken = true;
                    if (Engine.ExtendedLogging) logger.Info($"One card taken from deck and added to player {playerNumber.ToString()} new cards amount: {PlayersCurrentData[playerNumber].PlayerCards.Count - 1}");
                }
                else
                {
                    break;
                }
            }

            return cardsTaken;
        }

        //after one of players took all cards from lost battle - delete
        //bolean variable about taking one card
        private void ResetAllPlayersLostBattleCardsToTake(ref Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            foreach (var item in PlayersCurrentData)
            {
                item.Value.TookFirstCardLostBattle = false;
                if (Engine.ExtendedLogging) logger.Info($"Reseting player took first card in battle property for player {item.Key.ToString()} : {item.Value.TookFirstCardLostBattle}.");
            }
        }

        //after battle over (someone took all cards), restore the game status
        private void ChangeGameStatusAfterBattle(ref GameStatus status)
        {
            //if (status == GameStatus.StopsAndBattle) status = GameStatus.Stops;
            //else status = GameStatus.Standard;
            status = GameStatus.Standard;
            if (Engine.ExtendedLogging) logger.Info($"Game status changed to: {status.ToString()}.");
        }

        //check if at least one of cards can be placed on the table
        private bool CheckCardCorrectness(List<PlayingCard> Cards, PlayingCard firstAlreadyUsedCard,
            CardRanks demandedRank, CardSuits demandedSuit, GameStatus status)
        {
            bool canMakeMove = false;
            foreach (PlayingCard item in Cards)
            {
                CardCorrectnessChecker Checker = new CardCorrectnessChecker(item, firstAlreadyUsedCard,
                    demandedRank, demandedSuit, status);
                canMakeMove = Checker.CanTheCardBePlacedOnTheTable();
                if (canMakeMove) break;
            }
            return canMakeMove;
        }

        //checking if passed data is correct.
        private void CheckCorrectnessOfPassedData(int amountOfCards, List<PlayingCard> AlreadyUsedCards,
            List<PlayingCard> CurrentDeck, int jokersInDeck, int decksInPlay)
        {
            if (amountOfCards < 1)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "Player needs to take at least one card";
                logger.Error(text);
                throw new ArgumentException(text);
            }
            if (amountOfCards > CountAvailablecards(AlreadyUsedCards, CurrentDeck))
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "There is not enough cards to take";
                logger.Warn(text);
            }
            if (amountOfCards > ((13 * 4 + jokersInDeck) * decksInPlay))
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Warn("Player can not take more cards than all the decks count");
            }

            if (Engine.ExtendedLogging) logger.Info($"Taking cards from deck input parameters test passed.");
        }

        //method for counting available cards
        private int CountAvailablecards(List<PlayingCard> AlreadyUsedCards,
            List<PlayingCard> CurrentDeck)
        {
            int output = 0;
            if (AlreadyUsedCards.Count > 0) output = AlreadyUsedCards.Count - 1;
            output += CurrentDeck.Count;
            return output;
        }

        //reset matching card for all players
        private void ResetMatchingCardsInBattleMode(ref Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            foreach (var item in PlayersCurrentData)
            {
                item.Value.BattleModeMatchingCard = null;
                item.Value.FirstCardInBattleModeTakenMatches = false;
            }
        }

        #endregion
    }
}
