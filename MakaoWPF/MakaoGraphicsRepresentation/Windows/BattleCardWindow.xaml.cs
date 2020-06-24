using CardGraphicsLibraryHandler;
using MakaoEngine.CardCorectnessChecking;
using MakaoInterfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MakaoGraphicsRepresentation.Windows
{
    /// <summary>
    /// Interaction logic for BattleCardWindow.xaml
    /// </summary>
    public partial class BattleCardWindow : Window
    {
        #region Fields and properties

        private bool canBeClosed;
        private readonly PlayingCard newCard;
        private readonly PlayingCard topCard;
        private List<PlayingCard> permittedCardsList;
        private PlayingCard cardToChangeFromJoker;

        private readonly GameStatus status;

        //card demanding options
        private CardRanks demandedRank;
        private CardSuits demandedSuit;
        private readonly CardRanks alreadyDemandedRank;
        private readonly CardSuits alreadyDemandedSuit;

        #endregion

        #region Constructor

        public BattleCardWindow(PlayingCard card, PlayingCard topCard, GameStatus status, CardRanks alreadyDemandedRank, CardSuits alreadyDemandedSuit)
        {
            canBeClosed = false;
            newCard = card;
            this.topCard = topCard;
            cardToChangeFromJoker = null;
            demandedRank = CardRanks.None;
            demandedSuit = CardSuits.None;
            this.status = status;
            this.alreadyDemandedRank = alreadyDemandedRank;
            this.alreadyDemandedSuit = alreadyDemandedSuit;

            InitializeComponent();

            AssignPictureToTheCard(card);

            GameStatusHandling(card);
        }

        private void GameStatusHandling(PlayingCard card)
        {
            //joker card handling
            bool isJoker = ChackIfPassedCardHasSpecificRank(card, CardRanks.Joker);
            ChangeJokerChangeButtonVisibility(isJoker);
            if (isJoker)
            {
                //list of cards to change
                GenerateListOfCorrectCards(status);
            }

            //jack handling
            bool isJack = ChackIfPassedCardHasSpecificRank(card, CardRanks.Jack);
            ChangeJackButtonVisibility(isJack);

            //ace handling
            bool isAce = ChackIfPassedCardHasSpecificRank(card, CardRanks.Ace);
            ChangeAceButtonVisibility(isAce);
        }

        #endregion

        #region Window layout options setting

        private void AssignPictureToTheCard(PlayingCard card)
        {
            CardImage.Source = (new CardImageSourceObtainer()).GetImageSource(card);
        }

        private bool ChackIfPassedCardHasSpecificRank(PlayingCard card, CardRanks rank)
        {
            if (card.Rank == rank)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ChangeJokerChangeButtonVisibility(bool visible)
        {
            if (visible) ChangeJokerButton.Visibility = Visibility.Visible;
            else ChangeJokerButton.Visibility = Visibility.Collapsed;
        }

        private void ChangeJackButtonVisibility(bool visible)
        {
            if (visible) RankDemandingButton.Visibility = Visibility.Visible;
            else RankDemandingButton.Visibility = Visibility.Collapsed;
        }

        private void ChangeAceButtonVisibility(bool visible)
        {
            if (visible) SuitDemandingButton.Visibility = Visibility.Visible;
            else SuitDemandingButton.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Generating the list of permitted cards to change the joker

        private void GenerateListOfCorrectCards(GameStatus status)
        {
            permittedCardsList = new List<PlayingCard>();
            if (status == GameStatus.Battle || status == GameStatus.StopsAndBattle)
            {
                AddCardsBattle(ref permittedCardsList, CardRanks.Two);
                AddCardsBattle(ref permittedCardsList, CardRanks.Three);
                AddCardsBattle(ref permittedCardsList, CardRanks.King);
            }
            else if (status == GameStatus.RankDemanding)
            {
                AddCardsRankDemanding(ref permittedCardsList);
            }
            else if (status == GameStatus.SuitDemanding)
            {
                AddCardsSuitDemanding(ref permittedCardsList);
            }
            else //can't be stops, so here is standard mode
            {
                AddCardsStandardMode(ref permittedCardsList);
            }
        }

        private void AddCardsBattle(ref List<PlayingCard> cardsList, CardRanks rank)
        {
            foreach (CardSuits item in (CardSuits[])Enum.GetValues(typeof(CardSuits)))
            {
                if (rank != CardRanks.King || (rank == CardRanks.King && (item == CardSuits.Heart || item == CardSuits.Spade)))
                {
                    if (item != CardSuits.None)
                    {
                        bool correct = false;
                        bool exists = CheckIfCardAlreadyInTheList(cardsList, new PlayingCard(item, rank, newCard.DeckNumber));
                        if (!exists) correct = CheckCardCorrectness(new PlayingCard(item, rank, newCard.DeckNumber));
                        if (correct) cardsList.Add(new PlayingCard(item, rank, newCard.DeckNumber));
                    }
                }
            }
        }

        private void AddCardsRankDemanding(ref List<PlayingCard> cardsList)
        {
            foreach (CardRanks item in (CardRanks[])Enum.GetValues(typeof(CardRanks)))
            {
                if (item == alreadyDemandedRank || item == CardRanks.Jack)
                {
                    if (item != CardRanks.None)
                    {
                        foreach (CardSuits suitItem in (CardSuits[])Enum.GetValues(typeof(CardSuits)))
                        {
                            bool correct = false;
                            bool exists = CheckIfCardAlreadyInTheList(cardsList, new PlayingCard(suitItem, item, newCard.DeckNumber));
                            if (!exists) correct = CheckCardCorrectness(new PlayingCard(suitItem, item, newCard.DeckNumber));
                            if (correct) cardsList.Add(new PlayingCard(suitItem, item, newCard.DeckNumber));
                        }
                    }
                }
            }
        }

        private void AddCardsSuitDemanding(ref List<PlayingCard> cardsList)
        {
            foreach (CardRanks item in (CardRanks[])Enum.GetValues(typeof(CardRanks)))
            {
                if (item != CardRanks.None && item != CardRanks.Joker)
                {
                    foreach (CardSuits suitItem in (CardSuits[])Enum.GetValues(typeof(CardSuits)))
                    {
                        if (suitItem == alreadyDemandedSuit || (alreadyDemandedSuit == CardSuits.None && suitItem != CardSuits.None)
                            || (item == CardRanks.Ace && suitItem != CardSuits.None))
                        {
                            bool correct = false;
                            bool exists = CheckIfCardAlreadyInTheList(cardsList, new PlayingCard(suitItem, item, newCard.DeckNumber));
                            if (!exists) correct = CheckCardCorrectness(new PlayingCard(suitItem, item, newCard.DeckNumber));
                            if (correct) cardsList.Add(new PlayingCard(suitItem, item, newCard.DeckNumber));
                        }
                    }
                }
            }
        }

        private void AddCardsStandardMode(ref List<PlayingCard> cardsList)
        {
            foreach (CardRanks item in (CardRanks[])Enum.GetValues(typeof(CardRanks)))
            {
                if (item != CardRanks.None && item != CardRanks.Joker)
                {
                    foreach (CardSuits suitItem in (CardSuits[])Enum.GetValues(typeof(CardSuits)))
                    {
                        if (suitItem != CardSuits.None)
                        {
                            bool correct = false;
                            bool exists = CheckIfCardAlreadyInTheList(cardsList, new PlayingCard(suitItem, item, newCard.DeckNumber));
                            if (!exists) correct = CheckCardCorrectness(new PlayingCard(suitItem, item, newCard.DeckNumber));
                            if (correct) cardsList.Add(new PlayingCard(suitItem, item, newCard.DeckNumber));
                        }
                    }
                }
            }
        }

        private bool CheckCardCorrectness(PlayingCard card)
        {
            CardCorrectnessChecker Checker = new CardCorrectnessChecker(card, topCard, alreadyDemandedRank,
                alreadyDemandedSuit, status);
            return Checker.CanTheCardBePlacedOnTheTable();
        }

        private bool CheckIfCardAlreadyInTheList(List<PlayingCard> cardsList, PlayingCard newCard)
        {
            bool exists = false;

            foreach (var item in cardsList)
            {
                if (item.CompareTo(newCard) == 0)
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }

        #endregion

        #region Joker Button

        private void ChangeJokerButton_Click(object sender, RoutedEventArgs e)
        {
            FirstCardJokerPermittedCardsSelection newJokerWindow
                = new FirstCardJokerPermittedCardsSelection(permittedCardsList)
                {
                    Owner = this,
                };
            newJokerWindow.NewJokerCard += NewJokerWindow_NewJokerCard;
            newJokerWindow.ShowDialog();
            MemoryManagement.FlushMemory();
        }

        private void NewJokerWindow_NewJokerCard(object sender, JokerChangeToCardEventArgs e)
        {
            cardToChangeFromJoker = e.NewCard;
            AssignPictureToTheCard(cardToChangeFromJoker);
            ChangeVisibilityOfJackAndAceButtons();
        }

        private void ChangeVisibilityOfJackAndAceButtons()
        {

            //checking if choosen card is jack
            if (cardToChangeFromJoker.Rank == CardRanks.Jack)
            {
                ChangeJackButtonVisibility(true);
            }
            else
            {
                ChangeJackButtonVisibility(false);
                demandedRank = CardRanks.None;
            }

            //checking if choosen card is ace
            if (cardToChangeFromJoker.Rank == CardRanks.Ace)
            {
                ChangeAceButtonVisibility(true);
            }
            else
            {
                ChangeAceButtonVisibility(false);
                demandedSuit = CardSuits.None;
            }
        }

        #endregion

        #region Jack button

        private void RankDemandingButton_Click(object sender, RoutedEventArgs e)
        {
            DemandTheRankWindow rankDemandingWindow = new DemandTheRankWindow(topCard.Suit)
            {
                Owner = this,
            };
            rankDemandingWindow.RankDemandingWindowClosing += RankDemandingWindow_RankDemandingWindowClosing;
            rankDemandingWindow.ShowDialog();
            MemoryManagement.FlushMemory();
        }

        private void RankDemandingWindow_RankDemandingWindowClosing(object sender, DemandingRankWindowsEventArgs e)
        {
            demandedRank = e.NewRank;
        }

        #endregion

        #region Ace Button

        private void SuitDemandingButton_Click(object sender, RoutedEventArgs e)
        {
            DemandTheSuitWindow suitDemandingWIndow = new DemandTheSuitWindow()
            {
                Owner = this,
            };
            suitDemandingWIndow.SuitDemandingWindowClosing += SuitDemandingWIndow_SuitDemandingWindowClosing;
            suitDemandingWIndow.ShowDialog();
            MemoryManagement.FlushMemory();
        }

        private void SuitDemandingWIndow_SuitDemandingWindowClosing(object sender, DemandingSuitWindowsEventArgs e)
        {
            demandedSuit = e.NewSuit;
        }

        #endregion

        #region Closing the window

        //method - event handler for disabling closing window with x button
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!canBeClosed)
            {
                e.Cancel = true;
            }
        }

        //canceling puting new battle card on the table
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            canBeClosed = true;
            OnCancelButtonClick();
            Close();
        }

        //confirming puting card on the table
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (cardToChangeFromJoker != null || newCard.Rank != CardRanks.Joker)
            {
                canBeClosed = true;
                if (newCard.Rank != CardRanks.Joker) OnConfirmButtonClick(newCard.Suit, newCard.Rank, demandedRank, demandedSuit);
                else OnConfirmButtonClick(cardToChangeFromJoker.Suit, cardToChangeFromJoker.Rank, demandedRank, demandedSuit);
                Close();
            }
        }

        #endregion

        #region Clicking buttons events

        public delegate void CancelButtonClickEventHandler(object sender, EventArgs e);
        public event CancelButtonClickEventHandler CancelButtonClick;
        protected virtual void OnCancelButtonClick()
        {
            CancelButtonClick?.Invoke(this, new EventArgs());
        }

        public delegate void ConfirmButtonClickEventHandler(object sender, ConfirmPutBattleCardOnTheTableEventArgs e);
        public event ConfirmButtonClickEventHandler ConfirmButtonClick;
        protected virtual void OnConfirmButtonClick(CardSuits suit, CardRanks rank, CardRanks demRank, CardSuits demDuit)
        {
            ConfirmButtonClick?.Invoke(this, new ConfirmPutBattleCardOnTheTableEventArgs()
            { NewRank = rank, NewSuit = suit, DemandedRank = demRank, DemandedSuit = demandedSuit });
        }

        #endregion
    }

    #region Class with eventArgs for closing the window event data

    public class ConfirmPutBattleCardOnTheTableEventArgs : EventArgs
    {
        public CardRanks NewRank { get; set; }
        public CardSuits NewSuit { get; set; }
        public CardRanks DemandedRank { get; set; }
        public CardSuits DemandedSuit { get; set; }
    }

    #endregion
}
