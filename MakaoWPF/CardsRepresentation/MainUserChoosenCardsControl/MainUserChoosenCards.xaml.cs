using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CardGraphicsLibraryHandler;
using MakaoInterfaces;

namespace CardsRepresentation
{
    /// <summary>
    /// Interaction logic for MainUserChoosenCards.xaml
    /// </summary>
    public partial class MainUserChoosenCards : UserControl
    {
        #region Current cards list

        private List<PlayingCard> currentCards = new List<PlayingCard>();

        public List<PlayingCard> Cards
        {
            get { return currentCards; }
        }

        #endregion

        public MainUserChoosenCards()
        {
            InitializeComponent();
            NLogConfigMethod(); 
        }

        #region NLog implementation

        //Nlog configuration
        private void NLogConfigMethod()
        {
            //log initialization of the Main User control
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Vertical user control initialized");
        }

        #endregion

        #region Control content handling

        //method for assigning lista of cards to cntrol
        public void AssignCardsContent(List<PlayingCard> cardsList)
        {
            currentCards = cardsList;
            AbjustCardAmountInControl();
            AssignImages();
        }

        //method for cleaning control from cards
        public void EraseCrdsContent()
        {
            currentCards.Clear();
            AbjustCardAmountInControl();
            AssignImages();
        }

        #endregion

        #region Adding and removing cards from control

        //method for establoshement of cards amount in this control
        private void AbjustCardAmountInControl()
        {
            if(ControlGrid.Children.Count != currentCards.Count)
            {
                int difference = Math.Abs(ControlGrid.Children.Count - currentCards.Count);

                if(currentCards.Count > ControlGrid.Children.Count)
                {
                    for (int i = 0; i < difference; i++)
                    {
                        AddSingleButtonToControl();
                    }
                }
                else
                {
                    for (int i = 0; i < difference; i++)
                    {
                        RemoveSingleButtonFromControl();
                    }
                }
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Amount of cards in selected cards representation control changed to: " 
                    + ControlGrid.Children.Count.ToString());
            }
            AdjustCardsProperties();
        }

        //method for adding one button from control
        private void AddSingleButtonToControl()
        {
            MyButton myButton = new MyButton();
            (myButton.Height, myButton.Width) = EstimateCardHeightAndWidth(false);
            myButton.VerticalAlignment = VerticalAlignment.Top;
            myButton.HorizontalAlignment = HorizontalAlignment.Left;
            myButton.Margin = new Thickness(0);

            ControlGrid.Children.Add(myButton);
        }

        //method for deleting single button from control
        private void RemoveSingleButtonFromControl()
        {
            int amountOfCards = ControlGrid.Children.Count;
            ControlGrid.Children.RemoveAt(amountOfCards - 1);
        }

        //method for adjusting positions and dimensions of cards
        private void AdjustCardsProperties()
        {
            for (int i = 0; i < currentCards.Count; i++)
            {
                double width;
                double height;
                (height, width) = EstimateCardHeightAndWidth(false);

                //assigning dimension
                (ControlGrid.Children[i] as MyButton).Height = height;
                (ControlGrid.Children[i] as MyButton).Width = width;

                //assigning distance from top
                double topDistance = Distance(i, false);
                (ControlGrid.Children[i] as MyButton).Margin = new Thickness(0, topDistance, 0, 0);
            }
        }

        #endregion

        #region Assigning card graphics to cards

        //assigning image in loop
        private void AssignImages()
        {
            CardImageSourceObtainer obtainer = new CardImageSourceObtainer();

            if (ControlGrid.Children.Count != currentCards.Count)
            {
                throw new InvalidOperationException("Amount of cards in control and in" +
                    " internal list is not equal while asigning images to cards");
            }
            for (int i = 0; i < ControlGrid.Children.Count; i++)
            {
                if (currentCards[i].Rank == CardRanks.Joker)
                {
                    (ControlGrid.Children[i] as MyButton).CardImage = obtainer.GetImageSource(CardRanks.Joker, CardSuits.None);
                }
                else
                {
                    (ControlGrid.Children[i] as MyButton).CardImage = obtainer.GetImageSource(currentCards[i]);
                }
            }
        }

        #endregion

        #region Handling of cards sizes and positions

        //method for counting height and width of the control
        private (double, double) EstimateCardHeightAndWidth(bool fromEvent, double width = 100)
        {
            double controlWidth;

            if(!fromEvent)
            {
                controlWidth = Frame.ActualWidth;
            }
            else
            {
                controlWidth = width;
            }

            double widthOutput = controlWidth;
            double heightOutput = (widthOutput * 726) / 500;
            return (heightOutput, widthOutput);
        }

        //method for counting top distance
        private double Distance(int currentCardNumber, bool fromEvent, double width = 100, double height = 160)
        {
            int amountOfCards = currentCards.Count;

            //current card number counts from 0, so first card has nnumber: 0
            double distance;
            double fullHeight;
            double cardHeight;

            //check if method is called from resize event, and get size of cards
            if (!fromEvent)
            {
                fullHeight = Frame.ActualHeight;
                (cardHeight, _) = EstimateCardHeightAndWidth(false);
            }
            else
            {
                fullHeight = height;
                (cardHeight, _) = EstimateCardHeightAndWidth(true, width);
            }

            //check if card height summed up is greather than control height
            if (cardHeight * amountOfCards < fullHeight)
            {
                double gapeBeetweenCards = (fullHeight - (cardHeight * amountOfCards)) / (amountOfCards + 1);
                distance = currentCardNumber * cardHeight + ((currentCardNumber + 1) * gapeBeetweenCards);
            }
            else
            {
                double gapeBeetweenCards;
                if (amountOfCards > 1)
                {
                    gapeBeetweenCards = ((cardHeight * amountOfCards) - fullHeight) / (amountOfCards - 1);
                }
                else
                {
                    gapeBeetweenCards = 0;
                }
                distance = (currentCardNumber * cardHeight) - (gapeBeetweenCards * currentCardNumber);
            }

            return distance;
        }

        #endregion

        #region Handling resizing of control

        private void ControlGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double currentHeight = (sender as Grid).ActualHeight;
            double currentWidth = (sender as Grid).ActualWidth;

            for (int i = 0; i < ControlGrid.Children.Count; i++)
            {
                double cardHeight;
                double cardWidth;
                (cardHeight, cardWidth) = EstimateCardHeightAndWidth(true, ActualWidth);

                //size
                (ControlGrid.Children[i] as MyButton).Height = cardHeight;
                (ControlGrid.Children[i] as MyButton).Width = cardWidth;

                //margin
                double topDistance = Distance(i, true, currentWidth, currentHeight);
                (ControlGrid.Children[i] as MyButton).Margin = new Thickness(0, topDistance, 0, 0);
            }
        }

        #endregion
    }
}
