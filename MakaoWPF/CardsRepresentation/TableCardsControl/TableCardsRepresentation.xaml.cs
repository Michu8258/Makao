using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CardGraphicsLibraryHandler;
using MakaoInterfaces;

namespace CardsRepresentation
{
    /// <summary>
    /// Interaction logic for TableCardsRepresentation.xaml
    /// </summary>
    public partial class TableCardsRepresentation : UserControl
    {
        //amount of cards in control
        private int currentAmountOfCards = 0;
        private int numberOfFirstCard = 0;

        #region Current cards list

        private List<PlayingCard> currentCards = new List<PlayingCard>();

        public List<PlayingCard> Cards
        {
            get { return currentCards; }
        }

        #endregion

        public TableCardsRepresentation()
        {
            InitializeComponent();
            NLogConfigMethod();
            LastCardShader.Visibility = Visibility.Collapsed;
        }

        #region Adding green card

        //method for adding green highlight of first card
        private void AddGreenCard()
        {
            CardImageSourceObtainer obtainer = new CardImageSourceObtainer();

            MyButton myButton = new MyButton();
            (myButton.Height, myButton.Width) = EstimateCardHeightAndWidth(false);
            myButton.HorizontalAlignment = HorizontalAlignment.Center;
            myButton.VerticalAlignment = VerticalAlignment.Center;
            myButton.CardImage = obtainer.GetGreenHighlight();
            myButton.Margin = new Thickness(0);
            myButton.Visibility = Visibility.Collapsed;

            GreenCard.Children.Add(myButton);
        }

        #endregion

        #region NLog implementation

        //Nlog configuration
        private void NLogConfigMethod()
        {
            //log initialization of the Main User control
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Cards on table control initialized");
        }

        #endregion

        #region Control content handling

        //method for adding cards to control
        public void AddRange(List<PlayingCard> cardsList)
        {
            foreach (PlayingCard item in cardsList)
            {
                currentCards.Insert(0, item);
            }

            //adding grenn card only once
            AddGreenCardInFirsSlot();

            AdjustCardsAmount();
            AssignPictures();
        }

        //if there is no green card, add one
        private void AddGreenCardInFirsSlot()
        {
            //adding grenn card only onc
            if (GreenCard.Children.Count == 0)
            {
                AddGreenCard();
            }
        }

        
        //clearing control from cards
        public void Clear()
        {
            currentCards.Clear();
            AdjustCardsAmount();
            AddGreenCardInFirsSlot();
            AssignPictures();
            GreenCard.Children.Clear();
        }

        #endregion

        #region Adding cards to and removing them from control

        //method for adjusting amount of cards in control
        private void AdjustCardsAmount()
        {           
            if (currentCards.Count != currentAmountOfCards)
            {
                int difference = Math.Abs(currentCards.Count - currentAmountOfCards);
                if (currentCards.Count < currentAmountOfCards)
                {
                    for (int i = 0; i < difference; i++)
                    {
                        if (currentAmountOfCards > 0)
                        RemoveLastCardFromControl(currentAmountOfCards - 1);
                    }
                }
                else
                {
                    for (int i = 0; i < difference; i++)
                    {
                        if (currentAmountOfCards < 8)
                        AddCardToControl(currentAmountOfCards);
                    }
                }
            }
    }

        //method for adding one card to control
        private void AddCardToControl(int place)
        {
            if (place < 0 || place > 7)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "Place number cannot be grather than 7 or less than 0 - adding";
                logger.Error(text);
                throw new ArgumentException(text);
            }

            MyButton myButton = new MyButton();
            (myButton.Height, myButton.Width) = EstimateCardHeightAndWidth(false);
            myButton.HorizontalAlignment = HorizontalAlignment.Center;
            myButton.VerticalAlignment = VerticalAlignment.Center;
            myButton.CardImage = CardBackImageSourceObtainer.GetBackImageSource(BackColor.Gray);
            myButton.Margin = new Thickness(0);

            switch (place)  
            {
                case 0: FirstCard.Children.Add(myButton); currentAmountOfCards++; break;
                case 1: SecondCard.Children.Add(myButton); currentAmountOfCards++; break;
                case 2: ThirdCard.Children.Add(myButton); currentAmountOfCards++; break;
                case 3: FourthCard.Children.Add(myButton); currentAmountOfCards++; break;
                case 4: FifthCard.Children.Add(myButton); currentAmountOfCards++; break;
                case 5: SixthCard.Children.Add(myButton); currentAmountOfCards++; break;
                case 6: SeventhCard.Children.Add(myButton); currentAmountOfCards++; break;
                case 7: EightCard.Children.Add(myButton); currentAmountOfCards++; break;
                default: break;
            }
        }

        //method for removing last card
        private void RemoveLastCardFromControl(int place)
        {
            if (place < 0 || place > 7)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "Place number cannot be grather than 7 or less than 0 - deleting";
                logger.Error(text);
                throw new ArgumentException(text);
            }
            if (currentCards.Count >= 8)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "Cannot delete cards if there are at least 8 cards in the list";
                logger.Error(text);
                throw new ArgumentException(text);
            }

            switch (place)
            {
                case 0: FirstCard.Children.Clear(); currentAmountOfCards--; break;
                case 1: SecondCard.Children.Clear(); currentAmountOfCards--; break;
                case 2: ThirdCard.Children.Clear(); currentAmountOfCards--; break;
                case 3: FourthCard.Children.Clear(); currentAmountOfCards--; break;
                case 4: FifthCard.Children.Clear(); currentAmountOfCards--; break;
                case 5: SixthCard.Children.Clear(); currentAmountOfCards--; break;
                case 6: SeventhCard.Children.Clear(); currentAmountOfCards--; break;
                case 7: EightCard.Children.Clear(); currentAmountOfCards--; break;
                default: break;
            }
        }

        #endregion

        #region Cards Size adjustnment

        //method for counting width and height of cards
        private (double, double) EstimateCardHeightAndWidth(bool fromEvent, double height = 400, double width = 800)
        {
            double controlHeight;
            double controlWidth;
            double heightOutput;
            double widthOutput;

            if(!fromEvent)
            {
                controlHeight = Frame.ActualHeight;
                controlWidth = Frame.ActualWidth;
            }
            else
            {
                controlHeight = height;
                controlWidth = width;
            }

            double gridHeight = (controlHeight * 38) / 80;
            double gridWidth = (controlWidth * 38) / 160;

            //estimating mode - either width is obstacle or height is obstacle
            double PNGfactor = 0.688705234;
            double gridFactor = gridWidth / gridHeight;

            //width is obstacle
            if (gridFactor < PNGfactor)
            {
                widthOutput = gridWidth;
                heightOutput = (widthOutput * 726) / 500;
            }
            //height is obstacle
            else
            {
                heightOutput = gridHeight;
                widthOutput = (heightOutput * 500) / 726;
            }

            return (heightOutput, widthOutput);
        }

        #endregion

        #region Assigning pictures to cards

        //method for assigning Cards PNGs
        private void AssignPictures(int firstCardNumberInList = 0)
        {
            numberOfFirstCard = firstCardNumberInList;
            CardImageSourceObtainer obtainer = new CardImageSourceObtainer();

            //showing and hiding green highlight - first card
            if (numberOfFirstCard == 0)
            {
                GreenCard.Children[0].Visibility = Visibility.Visible;
            }
            else
            {
                GreenCard.Children[0].Visibility = Visibility.Collapsed;
            }

            //first card
            if (FirstCard.Children.Count > 0)
            {
                (FirstCard.Children[0] as MyButton).CardImage = obtainer.GetImageSource(currentCards[firstCardNumberInList]); //GetOnePNGString(currentCards[firstCardNumberInList]);
            }
            //second card
            if (SecondCard.Children.Count > 0)
            {
                (SecondCard.Children[0] as MyButton).CardImage = obtainer.GetImageSource(currentCards[firstCardNumberInList + 1]);
            }
            //third card
            if (ThirdCard.Children.Count > 0)
            {
                (ThirdCard.Children[0] as MyButton).CardImage = obtainer.GetImageSource(currentCards[firstCardNumberInList + 2]);
            }
            //fourth card
            if (FourthCard.Children.Count > 0)
            {
                (FourthCard.Children[0] as MyButton).CardImage = obtainer.GetImageSource(currentCards[firstCardNumberInList + 3]);
            }
            //fifth card
            if (FifthCard.Children.Count > 0)
            {
                (FifthCard.Children[0] as MyButton).CardImage = obtainer.GetImageSource(currentCards[firstCardNumberInList + 4]);
            }
            //sixth card
            if (SixthCard.Children.Count > 0)
            {
                (SixthCard.Children[0] as MyButton).CardImage = obtainer.GetImageSource(currentCards[firstCardNumberInList + 5]);
            }
            //seventh card
            if (SeventhCard.Children.Count > 0)
            {
                (SeventhCard.Children[0] as MyButton).CardImage = obtainer.GetImageSource(currentCards[firstCardNumberInList + 6]);
            }
            //eight card
            if (EightCard.Children.Count > 0)
            {
                (EightCard.Children[0] as MyButton).CardImage = obtainer.GetImageSource(currentCards[firstCardNumberInList + 7]);

                if (currentCards.Count == 8 || currentCards.Count == firstCardNumberInList + 8) LastCardShader.Visibility = Visibility.Collapsed;
                else LastCardShader.Visibility = Visibility.Visible;
            }
            else
            {
                LastCardShader.Visibility = Visibility.Collapsed;
            }
        }

        //method for getting one string describing png path
        private string GetOnePNGString(PlayingCard card)
        {
            string result = @"CardGraphics/Cards/";
            result += GetRankPNGString(card);
            result += GetSuitPNGString(card);
            result += ".png";
            return result;
        }

        //mathod for getting rank
        private string GetRankPNGString(PlayingCard card)
        {
            string result;
            switch (card.Rank)
            {
                case CardRanks.Two: result = "2_of_"; break;
                case CardRanks.Three: result = "3_of_"; break;
                case CardRanks.Four: result = "4_of_"; break;
                case CardRanks.Five: result = "5_of_"; break;
                case CardRanks.Six: result = "6_of_"; break;
                case CardRanks.Seven: result = "7_of_"; break;
                case CardRanks.Eight: result = "8_of_"; break;
                case CardRanks.Nine: result = "9_of_"; break;
                case CardRanks.Ten: result = "10_of_"; break;
                case CardRanks.Jack: result = "jack_of_"; break;
                case CardRanks.Queen: result = "queen_of_"; break;
                case CardRanks.King: result = "king_of_"; break;
                case CardRanks.Ace: result = "ace_of_"; break;
                default:
                    {
                        var logger = NLog.LogManager.GetCurrentClassLogger();
                        logger.Error("PNG string determination error (Joker or None rank passed to method) - GetRankPNGString");
                        throw new ArgumentException("Not permitted Card Rank");
                    }

            }
            return result;
        }

        //method for getting suit
        private string GetSuitPNGString(PlayingCard card)
        {
            string result;
            switch (card.Suit)
            {
                case CardSuits.Spade: result = "spades"; break;
                case CardSuits.Club: result = "clubs"; break;
                case CardSuits.Heart: result = "hearts"; break;
                case CardSuits.Diamond: result = "diamonds"; break;
                default:
                    {
                        var logger = NLog.LogManager.GetCurrentClassLogger();
                        logger.Error("PNG string determination error (None Suit passed to method) - GetSuitPNGString");
                        throw new ArgumentException("Not permitted Card Suit");
                    }
            }

            return result;
        }

        #endregion

        #region Handling resizing of control

        //method for resizing cards in control
        private void Frame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double currentHeight = Frame.ActualHeight;
            double currentWidth = Frame.ActualWidth;

            double height, width;
            (height, width) = EstimateCardHeightAndWidth(true, currentHeight, currentWidth);

            //green highlight
            if (GreenCard.Children.Count > 0)
            {
                (GreenCard.Children[0] as MyButton).Height = height;
                (GreenCard.Children[0] as MyButton).Width = width;
            }
            //first card
            if(FirstCard.Children.Count > 0)
            {
                (FirstCard.Children[0] as MyButton).Height = height;
                (FirstCard.Children[0] as MyButton).Width = width;
            }
            //second card
            if (SecondCard.Children.Count > 0)
            {
                (SecondCard.Children[0] as MyButton).Height = height;
                (SecondCard.Children[0] as MyButton).Width = width;
            }
            //third card
            if (ThirdCard.Children.Count > 0)
            {
                (ThirdCard.Children[0] as MyButton).Height = height;
                (ThirdCard.Children[0] as MyButton).Width = width;
            }
            //fourth card
            if (FourthCard.Children.Count > 0)
            {
                (FourthCard.Children[0] as MyButton).Height = height;
                (FourthCard.Children[0] as MyButton).Width = width;
            }
            //fifth card
            if (FifthCard.Children.Count > 0)
            {
                (FifthCard.Children[0] as MyButton).Height = height;
                (FifthCard.Children[0] as MyButton).Width = width;
            }
            //sixth card
            if (SixthCard.Children.Count > 0)
            {
                (SixthCard.Children[0] as MyButton).Height = height;
                (SixthCard.Children[0] as MyButton).Width = width;
            }
            //seventh card
            if (SeventhCard.Children.Count > 0)
            {
                (SeventhCard.Children[0] as MyButton).Height = height;
                (SeventhCard.Children[0] as MyButton).Width = width;
            }
            //eight card
            if (EightCard.Children.Count > 0)
            {
                (EightCard.Children[0] as MyButton).Height = height;
                (EightCard.Children[0] as MyButton).Width = width;
            }
        }

        #endregion

        #region Scrolling Cards

        //method for handling scrolling the content
        private void Frame_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //direction up
            if (e.Delta > 0)
            {
                if (numberOfFirstCard > 0)
                {
                    AssignPictures(numberOfFirstCard - 1);
                }
            }

            //direction down
            if (e.Delta < 0)
            {
                if (currentCards.Count >= 9)
                {
                    if (currentCards.Count - numberOfFirstCard >= 9)
                    {
                        AssignPictures(numberOfFirstCard + 1);
                    }
                }
            }
        }

        #endregion
    }
}
