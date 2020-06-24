using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CardGraphicsLibraryHandler;
using MakaoInterfaces;

#region Algorithms
/*Adding cards to control
 * 1. set the internal cards list equal to passed list
 * 2. Adjust amount of buttons in control to amount of cards
 * 3. assign card parameters to the buttons
 * 
 * Removing cards from control
 * 1. After all - the same as in adding carda
 * 2. One diffrence is to check if the card is really in the control
 * 3. if yes, delete the proper card
 * 
 * Removing all cards from control
 * 1. Just delete all cards from list
 * 2. up to date amount of cards
 * */

#endregion

namespace CardsRepresentation
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainUser : UserControl
    {
        //private fields
        private List<PlayingCard> currentCards = new List<PlayingCard>();
        private List<PlayingCard> temporaryCardList = null;

        //public properties
        public List<PlayingCard> ButtonCards
        {
            get { return currentCards; }
        }

        #region Player number property

        //private field to hold number of player
        private int numberofPlayer = -10;

        public int PlayerNumber
        {
            get { return numberofPlayer; }
        }

        //method for assigning number
        public void AssignPlayerNumber(int number)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            string text;
            if (number < 0)
            {
                text = "Player number passed in is less than 0";
                logger.Error(text);
                throw new ArgumentException(text);
            }
            if (number > 3)
            {
                text = "Player number passed in is more than 3";
                logger.Error(text);
                throw new ArgumentException(text);
            }
            if (numberofPlayer != number && numberofPlayer > 0)
            {
                text = "You can only assign player number once";
                logger.Error(text);
                throw new ArgumentException(text);
            }
            numberofPlayer = number;
            text = "Player number changed to: " + number.ToString();
            logger.Info(text);
        }

        #endregion

        //constructor - basicaly does nothing
        public MainUser()
        {
            InitializeComponent();
            AddMouseButtonDownEventToLabel();
            NLogConfigMethod();
        }

        #region NLog implementation

        //Nlog configuration
        private void NLogConfigMethod()
        {
            //log initialization of the Main User control
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Main User control initialized");
        }

        #endregion

        #region Add click event to frame

        //method for adding click event to label - reset cards selection
        private void AddMouseButtonDownEventToLabel()
        {            
            ResetSelectionText.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(
                (s, e) => { ResetLabelClick(); });
        }

        #endregion

        #region Adding and Removing Cards from control

        //method for modifying joker into another card or backwards
        public void ChangeToOrFromJocker (int index, PlayingCard beforeChange, PlayingCard afterChange)
        {
            if (currentCards[index].CompareTo(beforeChange) == 0)
            {                
                currentCards[index] = afterChange;
                SortTheCards();
                AdjustButtonsProperties();
                DisplayJokerTextOnButtons();
            }
        }

        //method for displaying the joker text
        private void DisplayJokerTextOnButtons()
        {
            foreach (MyButton item in ControlGrid.Children)
            {
                if (item.Card.CreatedByJocker == true)
                {
                    item.FromJoker = Visibility.Visible;
                }
                else
                {
                    item.FromJoker = Visibility.Collapsed;
                }
            }
        }

        //method for adding cards to the control
        public void AddCardsToCOntrol(List<PlayingCard> cards)
        {
            foreach (PlayingCard item in cards)
            {
                currentCards.Add(item);
            }

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Cards added to control: " + cards.Count);

            SortTheCards();
            AdjustButtonAmount();
            AdjustButtonsProperties();
            DisplayJokerTextOnButtons();
        }

        //remove cards from the control
        public void RemoveCardsFromControl(List<PlayingCard> cards)
        {
            if (cards != null)
            {
                if (cards.Count > 0)
                {
                    foreach (PlayingCard item in cards)
                    {
                        int indexToRemove = GetCardIndex(item);
                        if (indexToRemove > -1)
                        {
                            currentCards.RemoveAt(indexToRemove);
                        }
                    }
                }

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Cards removed from control: " + cards.Count);
            }

            SortTheCards();
            AdjustButtonAmount();
            AdjustButtonsProperties();
        }

        //remova all cards from control - will be blank
        public void RemoveAllCardsFromControl()
        {
            currentCards.Clear();
            AdjustButtonAmount();
            AdjustButtonsProperties();

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("All cards removed from control");
        }

        #endregion

        #region Adjusting buttons amount

        //checking, if card is in the control
        private int GetCardIndex(PlayingCard thisCard)
        {
            int index = -10;

            for (int i = 0; i < currentCards.Count; i++)
            {
                if (currentCards[i].CompareTo(thisCard) == 0)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        //method for setting correct amount of buttons in control
        private void AdjustButtonAmount()
        {
            int difference = Math.Abs(currentCards.Count - ControlGrid.Children.Count);

            //log amount of cards before change
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Adjusting button amount - old amount: " + ControlGrid.Children.Count);

            if (currentCards.Count < ControlGrid.Children.Count)
            {
                for (int i = 0; i < difference; i++)
                {
                    RemoveSingleButton();
                }
            }
            else if (currentCards.Count > ControlGrid.Children.Count)
            {
                for (int i = 0; i < difference; i++)
                {
                    AddSingleButton(ControlGrid.Children.Count);
                }
            }

            //log amount of cards after change
            logger.Info("Adjusting button amount - new amount: " + ControlGrid.Children.Count);
        }

        //adding one button
        private void AddSingleButton(int index)
        {
            MyButton button = new MyButton();
            (button.Height, button.Width) = EstimateCardHeightAndWidth(false);
            button.VerticalAlignment = VerticalAlignment.Bottom;
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.Margin = new Thickness(0);
            button.FromJoker = Visibility.Collapsed;

            //left mouse button click event            
            button.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(
                (s, e) =>
                {
                    MyButton clickedButton = s as MyButton;
                    PlayingCard playingCard = clickedButton.Card;
                    Visibility notPermitter = clickedButton.NotPermitted;
                    Visibility alreadySelected = clickedButton.AlreadySelected;
                    OnCardLeftClick(playingCard, notPermitter, alreadySelected);
                });

            //right mouse button click event
            button.PreviewMouseRightButtonDown += new MouseButtonEventHandler(
                (s, e) =>
                {
                    MyButton clickedButton = s as MyButton;
                    PlayingCard playingCard = clickedButton.Card;
                    bool wasMadeByJoker = clickedButton.Card.CreatedByJocker;
                    int childrenNumber = index;
                    OnCardRightClick(playingCard, wasMadeByJoker, childrenNumber);
                });

            //actually add button to ControlGrid
            ControlGrid.Children.Add(button);
        }

        //removing single button
        private void RemoveSingleButton()
        {
            ControlGrid.Children.RemoveAt(ControlGrid.Children.Count - 1);
        }

        //private void adjust buttons size and position
        private void AdjustButtonsProperties()
        {
            CardImageSourceObtainer obtainer = new CardImageSourceObtainer();

            for (int i = 0; i < currentCards.Count; i++)
            {
                double height;
                double width;
                (height, width) = EstimateCardHeightAndWidth(false);

                (ControlGrid.Children[i] as MyButton).Height = height;
                (ControlGrid.Children[i] as MyButton).Width = width;
                //(ControlGrid.Children[i] as MyButton).Picture = GetCardsPNGNameAsString(currentCards[i]);
                (ControlGrid.Children[i] as MyButton).CardImage = obtainer.GetImageSource(currentCards[i]);
                (ControlGrid.Children[i] as MyButton).Card = currentCards[i];
                (ControlGrid.Children[i] as MyButton).StringRepresentation = currentCards[i].ToString();

                //margins and position
                if (i < currentCards.Count - 1)
                {
                    (ControlGrid.Children[i] as MyButton).HorizontalAlignment = HorizontalAlignment.Left;
                    double leftDstance = Distance(i, ControlGrid.Children.Count, false);
                    (ControlGrid.Children[i] as MyButton).Margin = new Thickness(leftDstance, 0, 0, 0);
                }
                else
                {
                    (ControlGrid.Children[i] as MyButton).HorizontalAlignment = HorizontalAlignment.Right;
                    double rightDistance = Distance(i, ControlGrid.Children.Count, false);
                    (ControlGrid.Children[i] as MyButton).Margin = new Thickness(0, 0, rightDistance, 0);
                }
            }
        }

        #endregion

        #region Make not matching cards gray or green

        //method for reseting the highlights
        public void ResetAllHighlights()
        {
            foreach (MyButton item in ControlGrid.Children)
            {
                item.NotPermitted = Visibility.Collapsed;
                item.AlreadySelected = Visibility.Collapsed;
            }
            ResetSelectionVisibility();
        }

        //method for highlightening those cards, wchich was choosen to put on the table
        public void MarkCardsAsAlreadyChoosen(List<PlayingCard> cards)
        {
            //fot iteration loop by all buttons in control
            for (int i = 0; i < ControlGrid.Children.Count; i++)
            {
                //if card showe by this button is in the list, return true
                bool matching = HighlightOneCard((ControlGrid.Children[i] as MyButton).Card, cards);
                if (matching)
                {
                    //if true, hide gray rectangle
                    (ControlGrid.Children[i] as MyButton).AlreadySelected = Visibility.Visible;
                }
                else
                {
                    //if false, show the rectangle
                    (ControlGrid.Children[i] as MyButton).AlreadySelected = Visibility.Collapsed;
                }
            }
            ResetSelectionVisibility();
        }

        //method for making gray not matching cards
        public void HighlightNotMatchingCards(List<PlayingCard> cards)
        {
            //as input, there should be passed cards that match to the one selected,
            //if the cards is not in the list, will be set to gray

            //fot iteration loop by all buttons in control
            for (int i = 0; i < ControlGrid.Children.Count; i++)
            {
                //if card showe by this button is in the list, return true
                bool matching = HighlightOneCard((ControlGrid.Children[i] as MyButton).Card, cards);
                if (matching)
                {
                    //if true, hide gray rectangle
                    (ControlGrid.Children[i] as MyButton).NotPermitted = Visibility.Collapsed;
                }
                else
                {
                    //if false, show the rectangle
                    (ControlGrid.Children[i] as MyButton).NotPermitted = Visibility.Visible;
                }
            }
            ResetSelectionVisibility();
        }

        //find if one card is in list passed it
        private bool HighlightOneCard(PlayingCard button, List<PlayingCard> cardsList)
        {
            bool matching = false;

            foreach (PlayingCard item in cardsList)
            {
                if(item.CompareTo(button) == 0)
                {
                    matching = true;
                    break;
                }
            }
            ResetSelectionVisibility(); 
            return matching;
        }

        //method for checking if clicked card is first
        private bool FirstSelectionCard()
        {
            bool firstCard = true;

            foreach (MyButton item in ControlGrid.Children)
            {
                if (item.AlreadySelected == Visibility.Visible)
                {
                    firstCard = false;
                    break;
                }
            }

            return firstCard;
        }

        //visibility of label for reseting cards selection
        private void ResetSelectionVisibility()
        {
            bool firstCard = FirstSelectionCard();
            if (!firstCard)
            {
                ResetSelestionLabel.Visibility = Visibility.Visible;
            }
            else
            {
                ResetSelestionLabel.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Size adjustnment

        //counting cards height and width
        private (double, double) EstimateCardHeightAndWidth(bool fromEvent, double height = 100)
        {
            double controlHeight;

            if (fromEvent == false)
            {
                controlHeight = Frame.ActualHeight;
            }
            else
            {
                controlHeight = height;
            }
            double heightOutput = controlHeight * 0.8;
            double widthOutput = (heightOutput * 500) / 726;
            return (heightOutput, widthOutput);
        }

        //counting distance from left side of grid
        private double Distance(int currentNumber, int amountOfCards, bool fromEvent, double width = 300, double height = 100)
        {
            //current number counter from 0; for first card pass in value 0
            double distance;
            double fullWidth;
            double cardWidth;

            //checking if method is called from event and depending on this, counting card dimensions
            if (fromEvent == false)
            {
                fullWidth = Frame.ActualWidth;
                (_, cardWidth) = EstimateCardHeightAndWidth(false);
            }
            else
            {
                fullWidth = width;
                (_, cardWidth) = EstimateCardHeightAndWidth(true, height);
            }

            //not last card (on the right)
            if (currentNumber < amountOfCards - 1)
            {
                //total width of cards sumed up is less than whole control width
                if (cardWidth * amountOfCards < fullWidth)
                {
                    double gapBetweenCards = (fullWidth - (cardWidth * amountOfCards)) / (amountOfCards + 1);
                    distance = currentNumber * cardWidth + ((currentNumber + 1) * gapBetweenCards);
                }
                //total cards width bigger than control width
                else
                {                    
                    if (currentNumber == amountOfCards - 1)
                    {
                        distance = fullWidth - cardWidth;
                    }
                    else
                    {
                        double overlapDistance = (((amountOfCards * cardWidth) - fullWidth) / (amountOfCards - 1));
                        distance = (cardWidth - overlapDistance) * currentNumber;
                    }
                }
            }
            //last card
            else
            {
                //total width of cards sumed up is less than whole control width
                if (cardWidth * amountOfCards < fullWidth)
                {
                    double gapBetweenCards = (fullWidth - (cardWidth * amountOfCards)) / (amountOfCards + 1);
                    distance = gapBetweenCards;
                }
                //total cards width bigger than control width
                else
                {
                    distance = 0;
                }
            }           

            return distance;
        }

        #endregion

        #region Resizing cards to fit into control dimensions

        //method for resizing cards
        private void ControlGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double currentHeight = (sender as Grid).ActualHeight;
            double currentWidth = (sender as Grid).ActualWidth;

            for (int i = 0; i < currentCards.Count; i++)
            {
                double cardHeight;
                double cardWidth;
                (cardHeight, cardWidth) = EstimateCardHeightAndWidth(true, currentHeight);

                //size
                (ControlGrid.Children[i] as MyButton).Height = cardHeight;
                (ControlGrid.Children[i] as MyButton).Width = cardWidth;

                //margin
                if (i < ControlGrid.Children.Count - 1)
                {
                    double leftDistance = Distance(i, currentCards.Count, true, currentWidth, currentHeight);
                    (ControlGrid.Children[i] as MyButton).Margin = new Thickness(leftDistance, 0, 0, 0);
                }
                else
                {
                    double rightDistance = Distance(i, currentCards.Count, true, currentWidth, currentHeight);
                    (ControlGrid.Children[i] as MyButton).Margin = new Thickness(0, 0, rightDistance, 0);
                }
            }
        }

        #endregion

        #region Sorting

        //sorting method
        /* There it is how the sortin works:
         * first (from left) Joker, Ace, King, Queen, Jack, 10, 9, 8, 7, 6, 5, 4, 3, 2
         * second, by color: spades, headts, Clubs, Diamonds
         */

        //main method
        public void SortTheCards()
        {
            temporaryCardList = new List<PlayingCard>();
            temporaryCardList.Clear();

            temporaryCardList.AddRange(SortOneRank(CardRanks.Joker));
            temporaryCardList.AddRange(SortOneRank(CardRanks.Ace));
            temporaryCardList.AddRange(SortOneRank(CardRanks.King));
            temporaryCardList.AddRange(SortOneRank(CardRanks.Queen));
            temporaryCardList.AddRange(SortOneRank(CardRanks.Jack));
            temporaryCardList.AddRange(SortOneRank(CardRanks.Ten));
            temporaryCardList.AddRange(SortOneRank(CardRanks.Nine));
            temporaryCardList.AddRange(SortOneRank(CardRanks.Eight));
            temporaryCardList.AddRange(SortOneRank(CardRanks.Seven));
            temporaryCardList.AddRange(SortOneRank(CardRanks.Six));
            temporaryCardList.AddRange(SortOneRank(CardRanks.Five));
            temporaryCardList.AddRange(SortOneRank(CardRanks.Four));
            temporaryCardList.AddRange(SortOneRank(CardRanks.Three));
            temporaryCardList.AddRange(SortOneRank(CardRanks.Two));

            bool correct = CheckSortingCorectness();
            if (!correct)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Sorting error - list of sorted cards does not contain the same cards as before sorting");
                throw new InvalidProgramException("Cards after sort do not match cards before sort");
            }
            else
            {
                currentCards = temporaryCardList;
                temporaryCardList = null;
            }
        }

        //checkif if all the cards was before and are after sorting
        private bool CheckSortingCorectness()
        {
            bool correct = true;
            foreach (PlayingCard item in temporaryCardList)
            {
                if(!currentCards.Contains(item))
                {
                    correct = false;
                }
            }

            return correct;
        }

        //sorting cards with the same rank
        private List<PlayingCard> SortOneRank(CardRanks rank)
        {
            List<PlayingCard> sameRankList = GetCardsWithSameRank(rank);
            List<PlayingCard> sordetList = SortByColors(sameRankList);
            return sordetList;
        }

        //method for getting cards with the same rank from all cards in control
        private List<PlayingCard> GetCardsWithSameRank(CardRanks rank)
        {
            List<PlayingCard> cards = new List<PlayingCard>();
            foreach (PlayingCard item in currentCards)
            {
                if (item.Rank == rank)
                {
                    cards.Add(item);
                }
            }
            return cards;
        }

        //sort all the cards in one rank by card suit
        private List<PlayingCard> SortByColors(List<PlayingCard> sameRankList)
        {
            List<PlayingCard> sortedList = new List<PlayingCard>();
            sortedList.AddRange(GetCardsWithSameSuit(CardSuits.None, sameRankList));
            sortedList.AddRange(GetCardsWithSameSuit(CardSuits.Spade, sameRankList));
            sortedList.AddRange(GetCardsWithSameSuit(CardSuits.Heart, sameRankList));
            sortedList.AddRange(GetCardsWithSameSuit(CardSuits.Club, sameRankList));
            sortedList.AddRange(GetCardsWithSameSuit(CardSuits.Diamond, sameRankList));

            return sortedList;
        }

        //method for getting cards with specified suit in one rank list
        private List<PlayingCard> GetCardsWithSameSuit(CardSuits suit, List<PlayingCard> sameRankList)
        {
            List<PlayingCard> cards = new List<PlayingCard>();
            foreach (PlayingCard item in sameRankList)
            {
                if (item.Suit == suit)
                {
                    cards.Add(item);
                }
            }
            return cards;
        }

        #endregion

        #region LeftClickEvent

        public delegate void MainUserEventHandlerFirstSelectedCardClisk(object sender, MainUserEventArgs e);
        public delegate void MainUserEventHandlerAnotherSelectedCardClick(object sender, MainUserEventArgs e);

        public event MainUserEventHandlerFirstSelectedCardClisk FirstSelectedCardClick;
        public event MainUserEventHandlerAnotherSelectedCardClick AnotherSelectedCardClick;

        protected virtual void OnCardLeftClick(PlayingCard playingCard, Visibility notPermitted, Visibility alreadySelected)
        {       
            if (!FirstSelectionCard())
            {
                AnotherSelectedCardClick?.Invoke(this, new MainUserEventArgs { PlayingCard = playingCard, NotPermitted = notPermitted, AlreadySelected = alreadySelected });
            }
            else
            {
                FirstSelectedCardClick?.Invoke(this, new MainUserEventArgs { PlayingCard = playingCard });
            }
        }

        #endregion

        #region RightClickEvent

        public delegate void MainUserEventHandlerJokerClick(object sender, MainUserEventArgs e);

        public event MainUserEventHandlerJokerClick CardJokerClick;

        protected virtual void OnCardRightClick(PlayingCard playingCard, bool wasMadeByoker, int childrerNumber)
        {
            if (ResetSelestionLabel.Visibility == Visibility.Collapsed)
            {
                CardJokerClick?.Invoke(this, new MainUserEventArgs { PlayingCard = playingCard, WasMadeByJoker = wasMadeByoker, ChildrenNumber = childrerNumber });
            }
        }

        #endregion

        #region ResetLabelClick

        public delegate void MainUserEventHandlerFrameClick(object sender, MainUserEventArgs e);

        public event MainUserEventHandlerJokerClick ResetLabel;

        protected virtual void ResetLabelClick()
        {
            ResetLabel?.Invoke(this, new MainUserEventArgs());
        }

        #endregion
    }
}
