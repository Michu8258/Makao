using CardGraphicsLibraryHandler;
using CardsRepresentation;
using MakaoEngine.MatchingCardsFinding;
using MakaoGameClientService.DataTransferObjects;
using MakaoGameClientService.Messages;
using MakaoGameClientService.ServiceImplementations;
using MakaoGameHostService.Messages;
using MakaoGraphicsRepresentation.GameWIndowClasses;
using MakaoGraphicsRepresentation.MainWindowData;
using MakaoInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Shapes;

namespace MakaoGraphicsRepresentation.Windows
{
    public partial class GameWindow : Window, INotifyPropertyChanged
    {
        #region Fields

        //amount of users that is shown in the window (2-4)
        private readonly int amoutOfPlayersShown;

        //list pf cards choosen by thos player
        private readonly List<PlayingCard> CardsChoosenByThisPlayer;

        //showing proper players cards - adjust amount of diplayed players
        private readonly int minimumPlayerNumber;
        private readonly int maximumPlayerNumber;

        //this player data
        private readonly int thisPlayerNumber;
        private readonly string thisPlayerID;
        private readonly string thisPlayerName;

        //dictionary for controls identification purposes
        private readonly Dictionary<int, OtherPlayer> PlayersControlsMapper;

        //dictionaries for storing active player data
        private Dictionary<int, Rectangle> ActivePlayerRectangles;

        //bool for remembering if window was closed by player or by host
        private bool willBeClosedAsHost;

        //varaible for storing current game state data
        private GameStateData GameStateData;
        private readonly GameStateDataTranslator GameDataTranslator;

        //Synchronization context
        private readonly SynchronizationContext SynchCont;

        //variables for changing joker into another card aand back handling
        private PlayingCard currentCard;

        //variables for requesting demanding
        private CardRanks demandedRankRequest;
        private CardSuits demandedSuitRequest;
        private bool jackOrAceInCardsToPutOnTheTable;

        //click counter
        private int deckRepresentationControlClickCounter = 0;

        private static PlayingCard autoresponseCard;
        private static bool losingBattleCardAutoResponse;

        public static PlayingCard AutoresponseCard { get { return autoresponseCard; } set { autoresponseCard = value; } }
        public static bool LosingBattleCardAutoresponse { get { return losingBattleCardAutoResponse; } set { losingBattleCardAutoResponse = value; } }

        #endregion

        #region Properties

        public int AmoutOfPlayersShown { get { return amoutOfPlayersShown; } }
        public int ThisPlayerNumber { get { return thisPlayerNumber; } }
        public string ThisPlayerName { get { return thisPlayerName; } }
        public string ThisPLayerID { get { return thisPlayerID; } }

        //game status properties
        public bool ThisPlayerPauses { get { return GameDataTranslator.ThisPlayerPauses; } set { GameDataTranslator.ThisPlayerPauses = value;  OnPropertyChanged("ThisPlayerPauses"); } }
        public int AmountOfPauseTurns { get { return GameDataTranslator.AmountOfPauseTurns; } set { GameDataTranslator.AmountOfPauseTurns = value; OnPropertyChanged("AmountOfPauseTurns"); } }
        public bool CardRankIsDemanded { get { return GameDataTranslator.CardRankIsDemanded; } set { GameDataTranslator.CardRankIsDemanded = value;  OnPropertyChanged("CardRankIsDemanded"); } }
        public string DemandedRank { get { return GameDataTranslator.DemandedRank; } set { GameDataTranslator.DemandedRank = value;  OnPropertyChanged("DemandedRank"); } }
        public bool CardSuitIsDemanded { get { return GameDataTranslator.CardSuitIsDemanded; } set { GameDataTranslator.CardSuitIsDemanded = value; OnPropertyChanged("CardSuitIsDemanded"); } }
        public string DemandedSuit { get { return GameDataTranslator.DemandedSuit; } set { GameDataTranslator.DemandedSuit = value; OnPropertyChanged("DemandedSuit"); } }
        public int CurrentPlayerNumber { get { return GameDataTranslator.CurrentPlayerNumber; } set { GameDataTranslator.CurrentPlayerNumber = value; OnPropertyChanged("CurrentPlayerNumber"); } }
        public bool AmountOfCardsToTakeVisibility { get { return GameDataTranslator.AmountOfCardsToTakeVisibility; } set { GameDataTranslator.AmountOfCardsToTakeVisibility = value; OnPropertyChanged("AmountOfCardsToTakeVisibility"); } }
        public int AmountOfCardsToTakeLostBattle { get { return GameDataTranslator.AmountOfCardsToTakeLostBattle; } set { GameDataTranslator.AmountOfCardsToTakeLostBattle = value; OnPropertyChanged("AmountOfCardsToTakeLostBattle"); } }
        public bool BlockPossibilityOfTakingCardFromDeck { get { return GameDataTranslator.BlockPossibilityOfTakingCardsFromDeck; } set { GameDataTranslator.BlockPossibilityOfTakingCardsFromDeck = value;  OnPropertyChanged("BlockPossibilityOfTakingCardFromDeck"); } }
        public string GameStatus { get { return GameDataTranslator.CurrentStatusOfTheGame; } set { GameDataTranslator.CurrentStatusOfTheGame = value; OnPropertyChanged("GameStatus"); } }
        public bool MoveSkippingEnabled { get { return GameDataTranslator.CanPlayerSkipTheMove; } set { GameDataTranslator.CanPlayerSkipTheMove = value; OnPropertyChanged("MoveSkippingEnabled"); } }
        public bool CardTakenInBattleModeMatches { get { return GameDataTranslator.CardTakenInBattleModeMatches; } set { GameDataTranslator.CardTakenInBattleModeMatches = value; } }
        public PlayingCard MatchingCardInBattleMode { get { return GameDataTranslator.MatchingCardInBattleMode; } set { GameDataTranslator.MatchingCardInBattleMode = value; } }


        //player movement demanding options displaying
        public bool IsPlayerDemandingRank { get { return demandedRankRequest != CardRanks.None; } set { OnPropertyChanged("IsPlayerDemandingRank"); } }
        public string RankDemandedByThisPlayer { get { return RankAndSuitNameProvider.GetRankName(demandedRankRequest, "pl"); } set { OnPropertyChanged("RankDemandedByThisPlayer"); } }
        public bool IsPlayerDemandingSuit { get { return demandedSuitRequest != CardSuits.None; } set { OnPropertyChanged("IsPlayerDemandingSuit"); } }
        public string SuitDemandedByThisPlayer { get { return RankAndSuitNameProvider.GetSuitName(demandedSuitRequest, "pl"); } set { OnPropertyChanged("SuitDemandedByThisPlayer"); } }
        //make a move button
        public bool MakeMoveButtonVisible { get { return CardsChoosenByThisPlayer.Count > 0; } set { OnPropertyChanged("MakeMoveButtonVisible"); } }

        #endregion

        #region IPropertyChanged implementation

        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Constructor

        //proper constructor
        public GameWindow(ThirdPlayerLocation locationOfThirdPlayer, PersonalizedForSpecificPlayerStartGameDataRequest inputData,
            BackColor backCardsColor)
        {
            InitializeComponent();

            //synchronization context assignment
            SynchCont = SynchronizationContext.Current;

            //construct this window by using another class
            _ = new GameWindowConstructor(this, inputData, ref willBeClosedAsHost,
                ref amoutOfPlayersShown, ref thisPlayerNumber, ref thisPlayerID, ref thisPlayerName,
                ref minimumPlayerNumber, ref maximumPlayerNumber, ref PlayersControlsMapper,
                locationOfThirdPlayer, backCardsColor, ref ActivePlayerRectangles, ref GameStateData,
                ref CardsChoosenByThisPlayer, ref demandedRankRequest, ref demandedSuitRequest,
                ref GameDataTranslator);

            //assign INotifyPropertyChange properties values
            AssignProperties(inputData.CurrentGameStatusData);

            //player moves posibilities
            AssignPlayersMovePosibilities(inputData.DataOfThisPlayer);

            //assigning cards to controls
            AssignCardsOnGameStart(inputData);

            //subscript click event of deck representation control
            DeckRepresentationControl.TakeCardClick += DeckRepresentationControl_TakeCardClick;

            //Subscribing reciving the window updating data from host
            DataPlaceholder.UpdteTheGame += DataPlaceholder_UpdteTheGame;

            //Subscripting an event with info about game ending
            DataPlaceholder.GameEnded += DataPlaceholder_GameEnded;
        }

        //assigning cards to the controls in the window at the game startup
        private void AssignCardsOnGameStart(PersonalizedForSpecificPlayerStartGameDataRequest inputData)
        {
            CardsToControlsAssigner CardsAssigner = new CardsToControlsAssigner(this);
            CardsAssigner.AssignPlayersCards(PlayersControlsMapper, inputData.DataOfThisPlayer.ThisPlayerCards,
                inputData.DataOfOtherPlayers);
            CardsAssigner.AssignAmountOfCardsInDeck(inputData.AmountOfCardsInDeck);
            CardsAssigner.AddOneCardToUsedCardsControl(inputData.NewCardsOnTheTableList[0]);
        }

        #endregion

        #region Assigning game status data (INotifyPropertyChange)

        public void AssignProperties(GameStateData inputData)
        {
            GameStateData = inputData;

            ThisPlayerPauses = inputData.AmountOfPausingTurns > 0;
            AmountOfPauseTurns = inputData.AmountOfPausingTurns;
            CardRankIsDemanded = inputData.CurrentlyDemandedRank != CardRanks.None;
            DemandedRank = RankAndSuitNameProvider.GetRankName(inputData.CurrentlyDemandedRank, "pl");
            CardSuitIsDemanded = inputData.CurrentlyDemandedSuit != CardSuits.None;
            DemandedSuit = RankAndSuitNameProvider.GetSuitName(inputData.CurrentlyDemandedSuit, "pl");
            CurrentPlayerNumber = inputData.CurrentPlayerNumber;
            AmountOfCardsToTakeVisibility = inputData.AmountOfCardsToTakeIfLostBattle > 0;
            AmountOfCardsToTakeLostBattle = inputData.AmountOfCardsToTakeIfLostBattle;
            BlockPossibilityOfTakingCardFromDeck = inputData.BlockPossibilityOfTakingCardsFromDeck;
            GameStatus = GameStatusNameProvider.GetStatusName(inputData.CurrentStatusOfTheGame, "pl");
        }

        public void AssignPlayersMovePosibilities(ThisPlayerData data)
        {

            MoveSkippingEnabled = data.CanSkipTheMove;
            CardTakenInBattleModeMatches = data.TakenInBattleCardMatching;
            MatchingCardInBattleMode = data.MatchingCard;
        }

        #endregion

        #region Event for closing the window

        //closing event from the window
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!willBeClosedAsHost) OnWindowWasClosedByUser();
        }

        public delegate void ClosingTheWindowWventHandler(object sender, EventArgs e);
        public event ClosingTheWindowWventHandler WindowWasClosedByUser;
        public void OnWindowWasClosedByUser()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Game window is being closed by user");

            WindowWasClosedByUser?.Invoke(null, new EventArgs());
        }

        #endregion

        #region Updating game window with data received from host

        //event handler for updating the window
        private void DataPlaceholder_UpdteTheGame(object sender, UpdateGameWindowEventArgs e)
        {
            if(thisPlayerID == e.ReceivedData.PlayerID)
            {
                SynchCont.Post(_ => ThisUserControlsEnabler.EnableOrDisableThisUserControls(ref ThisPlayerControl,
                    ref DeckRepresentationControl, e.ReceivedData.CurrentPlayerNumber), null);

                //assign cards to controls
                CardsToControlsAssigner CardsAssigner = new CardsToControlsAssigner(this);
                SynchCont.Post(_ => CardsAssigner.AssignPlayersCards(PlayersControlsMapper, e.ReceivedData.DataOfThisPlayer.ThisPlayerCards,
                    e.ReceivedData.DataOfOtherPlayers), null);
                SynchCont.Post(_ => CardsAssigner.AssignAmountOfCardsInDeck(e.ReceivedData.AmountOfCardsInDeck), null);
                SynchCont.Post(_ => CardsAssigner.AddMultipleCardsToUsedCardsControl(e.ReceivedData.NewCardsOnTheTableList), null);

                //assign properties values - those which implements INotifyPropertyChange interface
                SynchCont.Post(_ => AssignProperties(e.ReceivedData.CurrentGameStatusData), null);

                //move posibilities
                SynchCont.Post(_ => AssignPlayersMovePosibilities(e.ReceivedData.DataOfThisPlayer), null);

                //players rectangles colors
                SynchCont.Post(_ => PlayersRectanglesColorAssigner.AssignPlayersRectanglesColors(ref ActivePlayerRectangles, e.ReceivedData.CurrentPlayerNumber), null);

                SynchCont.Post(_ => OpenMatchingCardInBattleWindow(MatchingCardInBattleMode, AlreadyUsedCardsControl.Cards[0]), null);
            }

            SynchCont.Post(_ => MemoryManagement.FlushMemory(), null);
        }

        #endregion

        #region Reseting internal demanding options

        private void ResetInternalDemandOptions()
        {
            jackOrAceInCardsToPutOnTheTable = false;
            demandedRankRequest = CardRanks.None;
            IsPlayerDemandingRank = false;
            RankDemandedByThisPlayer = RankAndSuitNameProvider.GetRankName(CardRanks.None, "pl");
            demandedSuitRequest = CardSuits.None;
            IsPlayerDemandingSuit = false;
            SuitDemandedByThisPlayer = RankAndSuitNameProvider.GetSuitName(CardSuits.None, "pl");
        }

        #endregion

        #region Handling custom events of main user control

        //choicement of first card to put on the table
        private void ThisPlayerControl_FirstSelectedCardClick(object sender, MainUserEventArgs e)
        {
            try
            {
                List<PlayingCard> permittedCardsList = new List<PlayingCard>();
                MatchingCardsFinder Finder = new MatchingCardsFinder(thisPlayerNumber, e.PlayingCard, false,
                    AlreadyUsedCardsControl.Cards[0], ThisPlayerControl.ButtonCards, GameStateData.CurrentlyDemandedRank,
                    GameStateData.CurrentlyDemandedSuit, GameStateData.CurrentStatusOfTheGame);
                List<FamiliarCardsData> FamiliarCardsLIst = Finder.FindMatchingCardsInPlayerHand();
                foreach (FamiliarCardsData item in FamiliarCardsLIst)
                {
                    permittedCardsList.Add(item.Card);
                }
                permittedCardsList.Add(e.PlayingCard);
                //make not matching cards gray
                ThisPlayerControl.HighlightNotMatchingCards(permittedCardsList);
                //reset list of choosen cards and add the one clicked
                CardsChoosenByThisPlayer.Clear();
                CardsChoosenByThisPlayer.Add(e.PlayingCard);
                //make one card green
                ThisPlayerControl.MarkCardsAsAlreadyChoosen(CardsChoosenByThisPlayer);

                //demanding windows
                if (e.PlayingCard.Rank == CardRanks.Jack) RankDemandingWindowHandle(e.PlayingCard.Suit);
                else if (e.PlayingCard.Rank == CardRanks.Ace) SuitDemandingWindowHandle();

                MakeMoveButtonVisible = true;
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info($"Error while trying to choose firs card to put on the table: {ex.Message}.");
            }
            ThisPlayerChoosenCardsControl.AssignCardsContent(CardsChoosenByThisPlayer);
        }

        //choicement or next card to put on the table
        private void ThisPlayerControl_AnotherSelectedCardClick(object sender, MainUserEventArgs e)
        {
            if (e.NotPermitted == Visibility.Collapsed && e.AlreadySelected == Visibility.Collapsed && CardsChoosenByThisPlayer.Count > 0)
            {
                CardsChoosenByThisPlayer.Add(e.PlayingCard);
                ThisPlayerControl.MarkCardsAsAlreadyChoosen(CardsChoosenByThisPlayer);
            }
            ThisPlayerChoosenCardsControl.AssignCardsContent(CardsChoosenByThisPlayer);
        }

        //changing card from joker into another or backwards
        private void ThisPlayerControl_CardJokerClick(object sender, MainUserEventArgs e)
        {
            currentCard = e.PlayingCard;

            if (e.PlayingCard.Rank == CardRanks.Joker && e.WasMadeByJoker == false)
            {
                Windows.JokerChange JokerWindow = new Windows.JokerChange()
                {
                    Owner = this
                };
                JokerWindow.JokerWindowClosing += JokerWindow_OKButtonClick;
                JokerWindow.ShowDialog();
                MemoryManagement.FlushMemory();
            }
            else if (e.WasMadeByJoker == true)
            {
                EngineHostDataOperator Operator = new EngineHostDataOperator();
                Operator.ChangeJokerBackFromAnotherCard(GenerateJokerChangeBackRequest(e.PlayingCard), ThisPlayerControl, SynchCont);
            }
        }

        //cclosing joker window event handler
        private void JokerWindow_OKButtonClick(object sender, MakaoGraphicsRepresentation.JokerWindowEventArgs e)
        {
            if (e.CardSuit != CardSuits.None && e.CardRank != CardRanks.None)
            {
                EngineHostDataOperator Operator = new EngineHostDataOperator();
                Operator.ChangeJokerIntoAnotherCard(GenerateJokerChangeRequest(currentCard, e.CardRank, e.CardSuit), ThisPlayerControl, SynchCont);
            }
        }

        //creating request for changing joker into another card
        private ChangeJokerIntoAnotherCardRequest GenerateJokerChangeRequest(PlayingCard cardToChange, CardRanks newRank, CardSuits newSuit)
        {
            ChangeJokerIntoAnotherCardRequest request = new ChangeJokerIntoAnotherCardRequest()
            {
                PlayerNumber = MainWindow.PlayerNumber,
                PlayerID = MainWindow.PlayerID,
                CardToChange = cardToChange,
                NewRank = newRank,
                NewSuit = newSuit,
            };
            return request;
        }

        //creating rewuest for changing joker card back (as it is now not a joker)
        private ChangeJokerBackRequest GenerateJokerChangeBackRequest(PlayingCard jokerCardToRetrieve)
        {
            ChangeJokerBackRequest request = new ChangeJokerBackRequest()
            {
                PlayerID = MainWindow.PlayerID,
                PlayerNumber = MainWindow.PlayerNumber,
                CardToRetrieveJoker = jokerCardToRetrieve,
            };
            return request;
        }

        //resetting cards selection
        private void ThisPlayerControl_ResetLabel(object sender, MainUserEventArgs e)
        {
            ResetUIAfterMakingMoveOrCancelingMove();
        }

        #endregion

        #region Demanding windows handling

        //method for handling with rank demanding window
        private void RankDemandingWindowHandle(CardSuits suit)
        {
            jackOrAceInCardsToPutOnTheTable = true;
            Windows.DemandTheRankWindow DemandingWindow = new DemandTheRankWindow(suit)
            {
                Owner = this,
            };
            DemandingWindow.RankDemandingWindowClosing += DemandingWindow_RankDemandingWindowClosing;
            DemandingWindow.ShowDialog();
            MemoryManagement.FlushMemory();
        }

        //event for closing the window for wstablishing demanded rank
        private void DemandingWindow_RankDemandingWindowClosing(object sender, DemandingRankWindowsEventArgs e)
        {
            demandedRankRequest = e.NewRank;
            IsPlayerDemandingRank = true;
            RankDemandedByThisPlayer = RankAndSuitNameProvider.GetRankName(e.NewRank, "pl");
        }

        //method for handling with suit demanding window
        private void SuitDemandingWindowHandle()
        {
            jackOrAceInCardsToPutOnTheTable = true;
            Windows.DemandTheSuitWindow DemandingWindow = new DemandTheSuitWindow()
            {
                Owner = this,
            };
            DemandingWindow.SuitDemandingWindowClosing += DemandingWindow_SuitDemandingWindowClosing;
            DemandingWindow.ShowDialog();
            MemoryManagement.FlushMemory();
        }

        private void DemandingWindow_SuitDemandingWindowClosing(object sender, DemandingSuitWindowsEventArgs e)
        {
            demandedSuitRequest = e.NewSuit;
            IsPlayerDemandingSuit = true;
            SuitDemandedByThisPlayer = RankAndSuitNameProvider.GetSuitName(e.NewSuit, "pl");
        }

        #endregion

        #region Making a move handling

        //event handler of taking a card (cards) from deck event
        private void DeckRepresentationControl_TakeCardClick(object sender, MainUserEventArgs e)
        {
            if (!BlockPossibilityOfTakingCardFromDeck)
            {
                deckRepresentationControlClickCounter++;
                if (deckRepresentationControlClickCounter >= 2)
                {
                    deckRepresentationControlClickCounter = 0;
                    SendMovementDataToHost(GeneratePerformingAMoveRequestData(false, new List<PlayingCard>(), false));
                }
            }
        }

        //button for passing the cards to engine (when making move)
        private void MakeAMoveButton_Click(object sender, RoutedEventArgs e)
        {
            SendMovementDataToHost(GeneratePerformingAMoveRequestData(true, CardsChoosenByThisPlayer, false));
        }

        //skipping the move
        private void SkipTheMoveButton_Click(object sender, RoutedEventArgs e)
        {
            SendMovementDataToHost(GeneratePerformingAMoveRequestData(true, new List<PlayingCard>(), true));
        }

        //method for changing game window view in case of making move
        private void ResetUIAfterMakingMoveOrCancelingMove()
        {
            CardsChoosenByThisPlayer.Clear();
            ThisPlayerControl.ResetAllHighlights();
            ThisPlayerChoosenCardsControl.EraseCrdsContent();
            ResetInternalDemandOptions();
            MakeMoveButtonVisible = false;
        }

        //mathod for generating request sended to host to perform a move
        private MakeAMoveRequest GeneratePerformingAMoveRequestData(bool takingCardsOrPutiingCards, List<PlayingCard> CardsList, bool skipMove) //false, new List<PlayingCard>(), false
        {
            List<PlayingCard> cardsToSend = new List<PlayingCard>();
            foreach (PlayingCard item in CardsList) cardsToSend.Add(item);

            MakeAMoveRequest request = new MakeAMoveRequest()
            {
                PlayerID = MainWindow.PlayerID,
                PlayerNumber = MainWindow.PlayerNumber,
                TakingCardsOrPutingCards = takingCardsOrPutiingCards,
                CardsToPutOnTheTable = cardsToSend,
                PlayerIsDemanding = demandedRankRequest != CardRanks.None || demandedSuitRequest != CardSuits.None || jackOrAceInCardsToPutOnTheTable,
                DemandedRank = demandedRankRequest,
                DemandedSuit = demandedSuitRequest,
                SkipTheMove = skipMove,
            };
            return request;
        }

        //method which starts sending the data about move
        private void SendMovementDataToHost(MakeAMoveRequest request)
        {
            EngineHostDataOperator Operator = new EngineHostDataOperator();
            Operator.MakeAMove(request, SynchCont);
            ResetUIAfterMakingMoveOrCancelingMove();
        }

        #endregion

        #region Loosing battle - first card from deck is correct handler

        //method for opening window
        private void OpenMatchingCardInBattleWindow(PlayingCard newCard, PlayingCard topCard)
        {
            if (CardTakenInBattleModeMatches)
            {
                BattleCardWindow BattleWindow = new BattleCardWindow(newCard, topCard, GameStateData.CurrentStatusOfTheGame,
                    GameStateData.CurrentlyDemandedRank, GameStateData.CurrentlyDemandedSuit)
                {
                    Owner = this
                };
                BattleWindow.CancelButtonClick += BattleWindow_CancelButtonClick;
                BattleWindow.ConfirmButtonClick += BattleWindow_ConfirmButtonClick;
                BattleWindow.ShowDialog();
                MemoryManagement.FlushMemory();
            }
        }

        //Confirming of putting card on the table
        private void BattleWindow_ConfirmButtonClick(object sender, ConfirmPutBattleCardOnTheTableEventArgs e)
        {
            //demanding options
            demandedRankRequest = e.DemandedRank;
            demandedSuitRequest = e.DemandedSuit;

            if (MatchingCardInBattleMode.CompareTo(new PlayingCard(e.NewSuit, e.NewRank, MatchingCardInBattleMode.DeckNumber)) == 0)
            SendMovementDataToHost(GeneratePerformingAMoveRequestData(true,
                new List<PlayingCard>() { MatchingCardInBattleMode }, false));
            else
            {
                losingBattleCardAutoResponse = true;
                autoresponseCard = new PlayingCard(MatchingCardInBattleMode.Suit, MatchingCardInBattleMode.Rank, MatchingCardInBattleMode.DeckNumber);
                autoresponseCard.ChangeCardFromJocker(e.NewSuit, e.NewRank);

                EngineHostDataOperator Operator = new EngineHostDataOperator();
                Operator.ResponseArrived += Operator_ResponseArrived;
                Operator.ChangeJokerIntoAnotherCard(GenerateJokerChangeRequest(
                    MatchingCardInBattleMode, e.NewRank, e.NewSuit), ThisPlayerControl, SynchCont);
            }
        }

        private void Operator_ResponseArrived(object sender, EventArgs e)
        {
            if (losingBattleCardAutoResponse)
            {
                losingBattleCardAutoResponse = false;
                SendMovementDataToHost(GeneratePerformingAMoveRequestData(true,
                    new List<PlayingCard>() { autoresponseCard }, false));
            }
        }

        //canceling putting card of the table
        private void BattleWindow_CancelButtonClick(object sender, EventArgs e)
        {
            if (GameStateData.CurrentStatusOfTheGame == MakaoInterfaces.GameStatus.Battle || GameStateData.CurrentStatusOfTheGame == MakaoInterfaces.GameStatus.StopsAndBattle)
            {
                SendMovementDataToHost(GeneratePerformingAMoveRequestData(false, new List<PlayingCard>(), false));
            }
            else
            {
                SendMovementDataToHost(GeneratePerformingAMoveRequestData(false, new List<PlayingCard>(), true));
            }
        }

        #endregion

        #region Game ended handling

        //event subscribed from data placeholder
        private void DataPlaceholder_GameEnded(object sender, GameEndedEventArgs e)
        {
            SynchCont.Post(_ => OpenEngGameResultsWindow(e.GameEndedData), null);
        }

        //method for opening window with game results
        private void OpenEngGameResultsWindow(GameFinishedDataRequest inputData)
        {
            GameFinishedWindow EndingWindow = new GameFinishedWindow(inputData, thisPlayerID)
            {
                //Owner = this,
            };
            EndingWindow.GameResultsWindowClosing += EndingWindow_GameResultsWindowClosing;
            EndingWindow.ShowDialog();
            MemoryManagement.FlushMemory();
        }

        //event for window closing by the button - event
        private void EndingWindow_GameResultsWindowClosing(object sender, GameFinishedEventArgs e)
        {
            willBeClosedAsHost = true;
            OnGameResultsWindowClosing(e.PlayedGames, e.PlayedAndWonGames);
            this.Close();
        }

        //event for passing data of closing window to main window
        public delegate void CloseResultsWindowEventHandler(object sender, GameFinishedEventArgs e);
        public event CloseResultsWindowEventHandler GameWindowClosedByHost;
        public void OnGameResultsWindowClosing(int playedGames, int wonGames)
        {
            GameWindowClosedByHost?.Invoke(null, new GameFinishedEventArgs { PlayedGames = playedGames, PlayedAndWonGames = wonGames });
        }

        #endregion
    }
}
