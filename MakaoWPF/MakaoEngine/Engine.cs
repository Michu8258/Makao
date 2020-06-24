using MakaoEngine.CardCorectnessChecking;
using MakaoEngine.Constructing;
using MakaoEngine.Disposing;
using MakaoEngine.GameStartingClasses;
using MakaoEngine.JokerDealing;
using MakaoEngine.MatchingCardsFinding;
using MakaoEngine.RulesHandling;
using MakaoInterfaces;
using System;
using System.Collections.Generic;

namespace MakaoEngine
{
    public class Engine : IMakaoCardGame, IDisposable
    {
        #region PrivateFields

        //field for making extended logging
        public static readonly bool ExtendedLogging = true;

        private bool staticTest;
        private int startCardsAmount;
        private GameStatus status;
        private int currentPlayer;  //counts from 0 (first player = 0)
        private int amountOfCardsToTake;
        private CardRanks demandedRank;
        private CardSuits demandedSuit;
        private int jokersInDeck;
        private int decksInPlay;
        private int amountOfPlayers;
        private List<PlayingCard> CurrentDeck;
        private List<PlayingCard> AlreadyUsedCards;
        List<PlayingCard> CardsLatelyPutedOnTheTable;
        private bool blockPossibilityOfTakingCard;
        private int temporaryPauseAmount;
        private Dictionary<int, SinglePlayerData> PlayersCurrentData;

        //finished players Dictionary (key - position, value - playerNumber)
        private Dictionary<int, int> finishedPlayers;

        //in case of lost battle, player does not have to take all the cards at once
        //he can take one, and check if it is brave card. If it is, he can use it.
        //if it is not, he has to take the rest of them. This boolean variable determines
        //if first card was already taken
        //private bool firstCardBattleLostTaken;
        //private int playerStartedRanksDemanding;
        //private int playerStartedSuitDemanding;
        //private List<List<PlayingCard>> GamersCards;

        //four handling
        //ak dprivate int playerNumberWhoStartedFours;

        #endregion

        #region IMakaoCardGame interface implementation - properties

        public int AmountOfJockerInDeck { get { return jokersInDeck; } }
        public int AmountOfDecksInGame { get { return decksInPlay; } }
        public int AmountOfPlayers { get { return amountOfPlayers; } }
        public int CurrentPlayer { get { return currentPlayer; } }
        public int AmountOfCardsToTake { get { return amountOfCardsToTake; } }
        public CardRanks DemandedRank { get { return demandedRank; } }
        public CardSuits DemandedSuit { get { return demandedSuit; } }
        public List<PlayingCard> Deck { get { return CurrentDeck; } }
        public Dictionary<int, List<PlayingCard>> PlayersCards { get { return PlayersCurrentPartialDataReturner.GetGamersCardsDictionary(PlayersCurrentData); } }
        public List<PlayingCard> UsedCards { get { return AlreadyUsedCards; } }
        public Dictionary<int, SinglePlayerData> PlayersData { get { return PlayersCurrentData; } }
        public GameStatus Status { get { return status; } }
        public List<PlayingCard> CardsLatelyPutOnTheTable { get { return CardsLatelyPutedOnTheTable; } }
        public bool BlockTakingCardsFromDeckOption { get { return blockPossibilityOfTakingCard; } }
        public Dictionary<int, int> FinishedPlayers { get { return finishedPlayers; } }

        #endregion

        #region Constructor

        //constructor
        public Engine(int players, int decks, int jokers, bool test, int amountOfCards = 5)
        {
            _ = new ConstructMakaoEngine(players, decks, jokers, test, amountOfCards, ref jokersInDeck, ref decksInPlay, ref startCardsAmount,
                ref amountOfPlayers, ref staticTest, ref CurrentDeck, ref AlreadyUsedCards, ref PlayersCurrentData,
                ref demandedRank, ref demandedSuit, ref amountOfCardsToTake, ref status, ref temporaryPauseAmount, ref finishedPlayers,
                ref CardsLatelyPutedOnTheTable);
        }

        #endregion

        #region IDisposable interface implementation

        //method called from another class - public
        public void Dispose()
        {
            Dispose();
        }

        //interface implementation
        void IDisposable.Dispose()
        {
            _ = new DisposeTheEngine(ref jokersInDeck, ref decksInPlay, ref startCardsAmount, ref amountOfPlayers,
                ref staticTest, ref CurrentDeck, ref AlreadyUsedCards, ref PlayersCurrentData,
                ref demandedRank, ref demandedSuit, ref amountOfCardsToTake, ref status);
        }

        #endregion

        #region Prepare Deck and Players cards

        //method implementing interface - prepare game
        public void CreateGame()
        {
            //calling game creator from another class
            NewGameCreator Creator = new NewGameCreator(decksInPlay, jokersInDeck, amountOfPlayers, startCardsAmount);
            Creator.CreateGame(ref CurrentDeck, ref PlayersCurrentData, ref AlreadyUsedCards, ref currentPlayer, ref CardsLatelyPutedOnTheTable);
        }

        #endregion

        #region CardCorectness

        //check if the card can be placed on the top of UsedCardsList
        public bool CanTheCardBePlacedOnTheTable(PlayingCard card)
        {
            CardCorrectnessChecker Checker = new CardCorrectnessChecker(card, AlreadyUsedCards[0],
                            demandedRank, demandedSuit, status);
            return Checker.CanTheCardBePlacedOnTheTable();
        }

        #endregion

        #region JockerDealingRegion

        //change one jocker card into any other card
        public bool ChangeJockerIntoAnotherCard(int playerNumber, PlayingCard card, CardRanks newRank, CardSuits newSuit,
            bool calledByEngine)
        {
            JokerChanger jokerChanger = new JokerChanger(playerNumber, card, newRank, newSuit,
                currentPlayer, calledByEngine);
            return jokerChanger.ChangeJockerIntoAnotherCard(ref PlayersCurrentData);
        }

        //method for returning the card to Jocker back
        public bool ChangeCardsIntoJockersBack(int playerNumber, PlayingCard card)
        {
            JokerRetriever retriever = new JokerRetriever(playerNumber, card);
            return retriever.ChangeCardsIntoJockersBack(ref PlayersCurrentData);
        }

        #endregion

        #region searchingFamiliarCardsInPlayerHand

        //public method for searching familliar cards to passed in list of payer cards
        public List<FamiliarCardsData> FindMatchingCardsInPlayerHand(int playerNumber, PlayingCard card)
        {
            MatchingCardsFinder Finder = new MatchingCardsFinder(playerNumber, card, staticTest,
                AlreadyUsedCards[0], PlayersCurrentData[playerNumber].PlayerCards, demandedRank, demandedSuit, status);
            return Finder.FindMatchingCardsInPlayerHand(ref AlreadyUsedCards, ref PlayersCurrentData);
        }

        #endregion

        #region PlayersMoveManagement

        #region FourHandling

        //method for handling cards of rank 4 - which player stops, and for how many rounds
        private (bool, bool) RankFourHandling(List<PlayingCard> cards, int playerNumber)
        {
            FoursHandler Handler = new FoursHandler(cards, playerNumber, amountOfPlayers);
            return Handler.FourHandling(ref PlayersCurrentData, ref status, ref temporaryPauseAmount,
                ref blockPossibilityOfTakingCard);
        }

        //method for accepting pausing of one of players
        private (bool, bool) MakePauseOnPlayersWish(List<PlayingCard> cards, int playerNumber)
        {
            FoursHandler Handler = new FoursHandler(cards, playerNumber, amountOfPlayers);
            return Handler.PlayerWantsToStop(ref PlayersCurrentData, ref status, ref temporaryPauseAmount,
                ref blockPossibilityOfTakingCard);
        }

        //when it is turn of player that need to wait few turns, it need to be count, that he
        //did not make a move for several rows
        private bool DecrementStopTurnsOfPlayer(int playerNumber, bool justFourEnded)
        {
            FoursHandler Handler = new FoursHandler(null, playerNumber, amountOfPlayers);
            return Handler.DecrementStopTurnsOfPlayer(ref PlayersCurrentData, currentPlayer, justFourEnded);
        }

        #endregion

        #region Determination of next player

        private void DetermineTheNextPlayer(PlayingCard card)
        {
            NextPlayerDeterminator Determinator = new NextPlayerDeterminator();
            currentPlayer = Determinator.DetermineNextPlayerNumber(card, currentPlayer, PlayersCurrentData.Count, PlayersCurrentData);

            //after switching player, the fact that player already took the first card is not true
            FoursHandler.ResetFirstCardTookProperties(ref PlayersCurrentData);
        }

        #endregion

        #region Taking cards from the deck handling

        //method for retrieving cards from table to the deck
        private void RestoreCardsFromTableToDeck()
        {
            TakeCardFromDeckHandler Handler = new TakeCardFromDeckHandler();
            Handler.RestoreCardsFromTableToDeck(ref AlreadyUsedCards, ref CurrentDeck);
        }

        //method for handling taking cards from deck
        private (bool, bool) TakeCardsFromDeck(int playerNumber)
        {
            TakeCardFromDeckHandler Handler = new TakeCardFromDeckHandler();
            return Handler.TakeCardsFromDeck(playerNumber, ref status, ref AlreadyUsedCards,
                Deck[0], ref PlayersCurrentData, ref amountOfCardsToTake, AlreadyUsedCards[0], demandedRank,
                demandedSuit, ref CurrentDeck, jokersInDeck, decksInPlay);
        }

        #endregion

        #region Counting cards to take for player who losts battle

        //method for checking if just puted card on the table is brave - count cards to take
        //by player that lost the battle
        private void AddCardsToTakeByPlayerThatLostBattle(PlayingCard card, bool lastCard)
        {
            BattleCardsCounter Counter = new BattleCardsCounter();
            Counter.AddCardsToTakeByPlayerThatLostBattle(card, ref amountOfCardsToTake,
                ref status, lastCard);
        }

        #endregion

        #region Checking if player has potentiality of move

        //if user has no matching card or there is no card to take, skip next player
        public bool CheckPotentialiTyOfMove(int playerNumber)
        {
            PossibilityOfMoveChecker Checker = new PossibilityOfMoveChecker();
            return Checker.CheckPotentialiTyOfMove(playerNumber, PlayersCurrentData,
                currentPlayer, AlreadyUsedCards[0], demandedRank, demandedSuit, status);
        }

        #endregion

        #region Handling of card demanding

        //control demanding options
        private bool CheckDemandPossibilities(CardRanks rank, CardSuits suit, int playerNumber)
        {
            CardsDemandingHandler Handler = new CardsDemandingHandler();
            return Handler.CheckDemandPossibilities(rank, suit, playerNumber, ref demandedRank,
                ref demandedSuit, ref status, ref PlayersCurrentData);
        }

        //method for checking if suit demanding can be stopped
        private void CheckStopOfSuitDemanding(PlayingCard firsCardPutedByPlayerOnTheTable)
        {
            CardsDemandingHandler Handler = new CardsDemandingHandler();
            Handler.ManageSuitEndDemanding(ref status, ref demandedSuit, firsCardPutedByPlayerOnTheTable,
                ref PlayersCurrentData);
        }

        #endregion

        #region Reseting move holding

        private void ResetoMoveHoldingForPlayers()
        {
            HoldingMovesReseter Reseter = new HoldingMovesReseter();
            Reseter.ResetHoldingMovesForAllPlayers(ref PlayersCurrentData);
        }

        #endregion

        #region Putting cards on the table

        //main method
        /// <summary>
        /// TakeCardsOrPutCards - true is puting cards and false when taking cards
        /// </summary>
        public bool PutCardsOnTheTable(bool TakeCardsOrPutCards, List<PlayingCard> CardsList, int playerNumber, bool isDemanding,
            CardRanks demandedRank, CardSuits demandedSuit, bool skipTheMove)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            if (ExtendedLogging) logger.Info("Makao engine main method - making a move fired");

            bool thisPlayerStartedRankDemanding = false;
            bool cardsPuted = false;
            bool cardstaken = false;
            bool endgame = false;
            bool justEndedFours = false;
            bool fourOrJokerCard;
            bool holdMoveOfThisPlayer = false;

            /*ALGORITHM
             * 1. Check if player that put the card is current player
             * 2. Check if this player heeds to stop
             * 2a. Check if player wants to take or to lay cards
             * 3. Check potentiality of the move
             * 4. Check card corectness
             * 5. Check if player which turn it is, started demanding
             * 6. check if it is 4
             * 7. check if it is demanding card, and set demanding options
             * 8. Actually put the card or cards on the table
             * 9. taking cards from deck by player who lost battle
             * 10. Determination of next player
             * 11. End the demanding
             * 12. Check end game conditions
             * 13. End game if there are conditions
             * 14. Send data actualization to players
             * */

            //1.
            bool moveMadeByCurrentPlayer = CheckIfItIsCurrentPlayer(playerNumber);
            if (moveMadeByCurrentPlayer)
            {
                if (ExtendedLogging) logger.Info("Correct player number condition passed");

                //2.
                if (ExtendedLogging) logger.Info($"STATUS BEFORE DECREMENTATION OF STOPS: {status.ToString().ToUpper()}.");
                if (DecrementStopTurnsOfPlayer(playerNumber, false)) //if this player do not need to pause
                {
                    if (ExtendedLogging) logger.Info("amount of stops condition passed");

                    if (!skipTheMove)
                    {
                        //2a.
                        if (TakeCardsOrPutCards) //placing cards on the table
                        {
                            if (ExtendedLogging) logger.Info("Puting cards on the table condition passed");

                            //3.
                            if (ExtendedLogging) logger.Info($"STATUS BEFORE CHECKING POTENTIALITY OF MOVE: {status.ToString().ToUpper()}.");
                            if (CheckPotentialiTyOfMove(playerNumber)) //if this player can make move
                            {
                                if (ExtendedLogging) logger.Info("Potentiality of the move condition passed");

                                //4.
                                if (CorectnessOfPutedCard(CardsList[0]))
                                {
                                    if (ExtendedLogging) logger.Info("First card correctness condition passed");

                                    //5.
                                    if (ExtendedLogging) logger.Info($"STATUS BEFORE GETTING PLAYER NAMBER WHO STARTE DEMANDING: {status.ToString().ToUpper()}.");
                                    if (currentPlayer == CardsDemandingHandler.GetPlayerNumberWhoStartedDemanding(PlayersCurrentData, false)) thisPlayerStartedRankDemanding = true;
                                    else thisPlayerStartedRankDemanding = false;
                                    if (ExtendedLogging) logger.Info($"This player started rank demanding: {thisPlayerStartedRankDemanding}.");

                                    //8.
                                    if (ExtendedLogging) logger.Info($"STATUS BEFORE PUTING CARDS ON THE TABLE: {status.ToString().ToUpper()}.");
                                    cardsPuted = PutAllCardsOnTheTable(CardsList, playerNumber);
                                    logger.Info($"Cards puted on the table: {cardsPuted}.");

                                    //6.
                                    if (ExtendedLogging) logger.Info($"STATUS BEFORE FOUR HANDLING: {status.ToString().ToUpper()}.");
                                    (fourOrJokerCard, justEndedFours) = RankFourHandling(CardsList, playerNumber);
                                    if (ExtendedLogging) logger.Info($"STATUS AFTER FOUR HANDLING: {status.ToString().ToUpper()}.");
                                    logger.Info($"Fours in player hand: {fourOrJokerCard}.");

                                    //7.
                                    if (isDemanding && (CardsList[0].Rank == CardRanks.Ace || CardsList[0].Rank == CardRanks.Jack))
                                    {
                                        CheckDemandPossibilities(demandedRank, demandedSuit, playerNumber);
                                    }

                                    CheckStopOfSuitDemanding(CardsList[0]);
                                }
                            }
                            ResetoMoveHoldingForPlayers();
                        }
                        else
                        {
                            (cardstaken, holdMoveOfThisPlayer) = TakeCardsFromDeck(playerNumber);
                            if (currentPlayer == CardsDemandingHandler.GetPlayerNumberWhoStartedDemanding(PlayersCurrentData, false)) thisPlayerStartedRankDemanding = true;
                            else thisPlayerStartedRankDemanding = false;
                            EndDemanding(thisPlayerStartedRankDemanding, playerNumber);
                        }
                    }
                    else
                    {
                        if (!PlayersCurrentData[playerNumber].FirstCardInBattleModeTakenMatches)
                        (_, justEndedFours) = MakePauseOnPlayersWish(new List<PlayingCard>(), playerNumber);

                        ResetoMoveHoldingForPlayers();
                    }
                }

                //if either new cards are on the table or some cards are taken from deck
                if (cardsPuted || cardstaken || skipTheMove)
                {
                    if (ExtendedLogging) logger.Info($"Tha card was taken: {cardstaken} / cards was puted: {cardsPuted} / Move skipped: {skipTheMove}.");
                    //9. - choosing next player as long, as he do not need to pause
                    //caused by four rank cards
                    if (!holdMoveOfThisPlayer)
                    {
                        bool nextPlayer = false;
                        while (!nextPlayer)
                        {
                            DetermineTheNextPlayer(AlreadyUsedCards[0]);
                            nextPlayer = DecrementStopTurnsOfPlayer(currentPlayer, justEndedFours);
                            if (ExtendedLogging) logger.Info($"Next player number - ASSIGNED: {nextPlayer}");
                        }
                    }

                    //10.
                    EndDemanding(thisPlayerStartedRankDemanding, playerNumber);

                    //11.
                    if (CurrentDeck.Count <= 5) RestoreCardsFromTableToDeck();

                    //12.
                    endgame = CheckGameEndingConditions();
                }

                if (!cardsPuted)
                {
                    CardsLatelyPutedOnTheTable.Clear();
                    if (ExtendedLogging) logger.Info($"Cards lately puted on the table cleared.");
                }

                //here start sending info to player with data actualization
                OnStartUpdatingTheGame(cardsPuted || cardstaken || skipTheMove);//if (!endgame) 
                if (endgame)
                {
                    if (ExtendedLogging) logger.Info("GAME ENDED - STARTING SENDING INFO ABOUT THIS TO ALL PLAYERS");

                    //adding last player to finished players list
                    EndGameConditionsChecker Checker = new EndGameConditionsChecker();
                    Checker.AddLastPlayerToFinishedList(ref finishedPlayers, PlayersCurrentData);

                    //sending data with game ended info
                    OnEndTheGame();
                }
            }

            if (ExtendedLogging) logger.Info("Makao engine main method - making a move ended");
            if (ExtendedLogging) logger.Info($"GAME STATE AFTER EXECUION OF ENGINE MAIN METHOD: {status.ToString().ToUpper()}");
            return (cardsPuted || cardstaken || skipTheMove);
        }

        #region Card putting methods

        private bool CheckGameEndingConditions()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            EndGameConditionsChecker Checker = new EndGameConditionsChecker();
            bool returnConditions =  Checker.CheckGameEndingConditions(ref finishedPlayers, PlayersCurrentData);
            if (ExtendedLogging) logger.Info($"Return Conditions: {returnConditions}.");
            return returnConditions;
        }

        //1. Check if player that sends the method has his turn at the moment
        private bool CheckIfItIsCurrentPlayer(int playerNumber)
        {
            if (currentPlayer != playerNumber)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "It is not the turn of the player that tries to make a move";
                logger.Error(text);
                throw new ArgumentException(text);
            }
            else return true;
        }

        //4. Check corectness
        private bool CorectnessOfPutedCard(PlayingCard card)
        {
            bool OK = CanTheCardBePlacedOnTheTable(card);
            if (!OK)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "The cars passed as first to put on the table cannot be placed on the table in this circumstantions";
                logger.Error(text);
                throw new ArgumentException(text);
            }
            return OK;
        }

        //9. actually add cards to the table
        private bool PutAllCardsOnTheTable(List<PlayingCard> list, int playerNumber)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            if (ExtendedLogging) logger.Info($"Put all cards on the table method fired.");

            bool output = false; //true when at leas one card puten on the table
            CardsLatelyPutedOnTheTable.Clear();

            if (ExtendedLogging)
            {
                logger.Info($"Current cards of player {playerNumber}:");
                foreach (PlayingCard item in PlayersCurrentData[playerNumber].PlayerCards)
                {
                    logger.Info($"      {item.ToString()};");
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                bool cardExistsInPlayerHand;
                int cardIndex;

                CardPossessingChecker Checker = new CardPossessingChecker();
                (cardExistsInPlayerHand, cardIndex) = Checker.CheckIfPlayerHasDefinedCardInHand(list[i], playerNumber, ref PlayersCurrentData);
                //bool corectness = CanTheCardBePlacedOnTheTable(list[i]);

                //if (ExtendedLogging) logger.Info($"Put all cards on the table is card correct: {corectness}");

                //add cards to be taken by player that lost battle
                AddCardsToTakeByPlayerThatLostBattle(list[i], i == list.Count - 1);

                if (cardExistsInPlayerHand) // && corectness
                {
                    AlreadyUsedCards.Insert(0, list[i]);
                    CardsLatelyPutedOnTheTable.Add(list[i]);
                    PlayersCurrentData[playerNumber].PlayerCards.RemoveAt(cardIndex);
                    output = true;
                    logger.Info($"Card added to the table: {list[i].ToString()}");
                }
            }
            return output;
        }

        //12 End of demanding
        private void EndDemanding(bool thisPlayerStartedDemanding, int playerNumber)
        {
            //reset demanding player
            if (thisPlayerStartedDemanding == true)
            {
                CheckDemandPossibilities(CardRanks.None, CardSuits.None, playerNumber);
            }
        }

        #endregion

        #endregion

        #endregion

        #region Event for fire updatiing the plaers data

        public delegate void StartUpdatingGameDataEventHandler(object sender, UpdateGameStatusEventArgs e);
        public event StartUpdatingGameDataEventHandler StartUpdatingTheGame;
        public  void OnStartUpdatingTheGame(bool args)
        {
            StartUpdatingTheGame?.Invoke(null, new UpdateGameStatusEventArgs { MoveAccepted = args});
        }

        #endregion

        #region Event for ending the game

        public delegate void EndingTheGameEventHandler(object sender, EventArgs e);
        public event EndingTheGameEventHandler EndTheGame;
        public void OnEndTheGame()
        {
            EndTheGame?.Invoke(null, new EventArgs {});
        }

        #endregion
    }
}
