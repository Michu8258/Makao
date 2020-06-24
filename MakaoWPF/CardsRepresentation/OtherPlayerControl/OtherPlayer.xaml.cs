using CardGraphicsLibraryHandler;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CardsRepresentation
{
    /// <summary>
    /// Interaction logic for HorizontalPlayer.xaml
    /// </summary>
    public partial class OtherPlayer : UserControl
    {
        //private color of cards back
        private BackColor cardBack = BackColor.Blue;

        private bool horVer;
        /// <summary>
        /// Horizontal - false, Vertical - true
        /// </summary>
        public bool HorizontalVertical
        {
            get { return horVer; }
            set { horVer = value; }
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

        public OtherPlayer()
        {
            InitializeComponent();
            NLogConfigMethod();
        }

        #region NLog implementation

        //Nlog configuration
        private void NLogConfigMethod()
        {
            //log initialization of the other User control
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Horizontal user control initialized");
        }

        #endregion

        #region Adding and removing Cards

        //public method for establishing amount of cards in control
        public void AdjustCardsAmount(int amountOfCards)
        {
            if (amountOfCards != ControlGrid.Children.Count)
            {
                int difference = Math.Abs(amountOfCards - ControlGrid.Children.Count);

                if (amountOfCards > ControlGrid.Children.Count)
                {
                    //adding buttons in loop
                    for (int i = 0; i < difference; i++)
                    {
                        AddSingleButtonToControl();
                    }
                }
                else
                {
                    //removing buttons in loop
                    for (int i = 0; i < difference; i++)
                    {
                        RemoveSingleCardFromControl();
                    }
                }
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Amount of cards in horizontal player control " + numberofPlayer.ToString() +
                    " changed to: " + amountOfCards.ToString());
            }
            AdjustCardsProperties();
        }

        //method for adding new Card (button) to Contorl
        private void AddSingleButtonToControl()
        {
            MyButton myButton = new MyButton();
            (myButton.Height, myButton.Width) = EstimateCardHeightAndWidth(false);
            myButton.VerticalAlignment = VerticalAlignment.Top;
            myButton.HorizontalAlignment = HorizontalAlignment.Left;
            myButton.Margin = new Thickness(0);
            myButton.BackCardImage = CardBackImageSourceObtainer.GetBackImageSource(cardBack);

            ControlGrid.Children.Add(myButton);
        }

        //method for changing back colors
        private void ChangeBackColorInAllCards()
        {
            for (int i = 0; i < ControlGrid.Children.Count; i++)
            {
                (ControlGrid.Children[i] as MyButton).BackCardImage = CardBackImageSourceObtainer.GetBackImageSource(cardBack);
            }
        }

        //method for removing single button from Control
        private void RemoveSingleCardFromControl()
        {
            int amountOfCards = ControlGrid.Children.Count;
            ControlGrid.Children.RemoveAt(amountOfCards - 1);
        }

        //method that adjusts position and dimensions of cards
        private void AdjustCardsProperties()
        {
            for (int i = 0; i < ControlGrid.Children.Count; i++)
            {
                double width;
                double height;
                (height, width) = EstimateCardHeightAndWidth(false);

                //assigning dimensions
                (ControlGrid.Children[i] as MyButton).Height = height;
                (ControlGrid.Children[i] as MyButton).Width = width;

                //assigning distance from top
                if (!horVer) // 0 - horizontal
                {
                    double leftDistance = Distance(i, ControlGrid.Children.Count, false);
                    (ControlGrid.Children[i] as MyButton).Margin = new Thickness(leftDistance, 0, 0, 0);
                }
                else //1 - vertival
                {
                    double topDistance = Distance(i, ControlGrid.Children.Count, false);
                    (ControlGrid.Children[i] as MyButton).Margin = new Thickness(0, topDistance, 0, 0);
                }
            }
        }

        #endregion

        #region Handling of cards size and position

        //method that counts dimensions of a Button
        private (double, double) EstimateCardHeightAndWidth(bool fromEvent, double heigt = 200, double width = 100)
        {
            double controlHeight;
            double controlWidth;

            double widthOutput;
            double heightOutput;

            if (!fromEvent)
            {
                controlHeight = Frame.ActualHeight;
                controlWidth = Frame.ActualWidth;
            }
            else
            {
                controlHeight = heigt;
                controlWidth = width;
            }
            if (!horVer) // 0 = horizontal
            {
                heightOutput = controlHeight;
                widthOutput = (heightOutput * 691) / 1056;
            }
            else //1 - vertical
            {
                widthOutput = controlWidth;
                heightOutput = (widthOutput * 1056) / 691;
            }
            

            //return, first height, then width;
            return (heightOutput, widthOutput);
        }


        //method for counting top distance
        private double Distance(int currentCardNumber, int amountOfCards, bool fromEvent, double width = 100, double height = 160)
        {
            //current card number counts from 0, so first card has nnumber: 0
            double distance = 0;
            double fullWidth = 0;
            double cardWidth = 0;
            double fullHeight = 0;
            double cardHeight = 0;

            if (!horVer) // 0 = horizontal
            {
                return HorizontalDistance(ref distance, ref fullWidth, ref cardWidth, currentCardNumber, amountOfCards, fromEvent, width, height);
            }
            else  //1 - vertical
            {
                return VerticalDistance(ref distance, ref fullHeight, ref cardHeight, currentCardNumber, amountOfCards, fromEvent, width, height);
            }
        }

        //counting distance for the horizontal control
        private double HorizontalDistance(ref double distance, ref double fullWidth, ref double cardWidth,
            int currentCardNumber, int amountOfCards, bool fromEvent, double width = 100, double height = 160)
        {
            //check if method is called from resize event, and get size of cards
            if (!fromEvent)
            {
                fullWidth = Frame.ActualWidth;
                (_, cardWidth) = EstimateCardHeightAndWidth(false);
            }
            else
            {
                fullWidth = width;
                (_, cardWidth) = EstimateCardHeightAndWidth(true, height, width);
            }

            //check if card height summed up is greather than control height
            if (cardWidth * amountOfCards < fullWidth)
            {
                double gapeBeetweenCards = (fullWidth - (cardWidth * amountOfCards)) / (amountOfCards + 1);
                distance = currentCardNumber * cardWidth + ((currentCardNumber + 1) * gapeBeetweenCards);
            }
            else
            {
                double gapeBeetweenCards = ((cardWidth * amountOfCards) - fullWidth) / (amountOfCards - 1);
                distance = (currentCardNumber * cardWidth) - (gapeBeetweenCards * currentCardNumber);
            }

            return distance;
        }

        private double VerticalDistance(ref double distance, ref double fullHeight, ref double cardHeight,
            int currentCardNumber, int amountOfCards, bool fromEvent, double width = 100, double height = 160)
        {
            //check if method is called from resize event, and get size of cards
            if (!fromEvent)
            {
                fullHeight = Frame.ActualHeight;
                (cardHeight, _) = EstimateCardHeightAndWidth(false);
            }
            else
            {
                fullHeight = height;
                (cardHeight, _) = EstimateCardHeightAndWidth(true, height, width);
            }

            //check if card height summed up is greather than control height
            if (cardHeight * amountOfCards < fullHeight)
            {
                double gapeBeetweenCards = (fullHeight - (cardHeight * amountOfCards)) / (amountOfCards + 1);
                distance = currentCardNumber * cardHeight + ((currentCardNumber + 1) * gapeBeetweenCards);
            }
            else
            {
                double gapeBeetweenCards = ((cardHeight * amountOfCards) - fullHeight) / (amountOfCards - 1);
                distance = (currentCardNumber * cardHeight) - (gapeBeetweenCards * currentCardNumber);
            }

            return distance;
        }

            #endregion

            #region Handling resizing of control

            //method for resizing cards
        private void ControlGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double currentHeight = (sender as Grid).ActualHeight;
            double currentWidth = (sender as Grid).ActualWidth;

            for (int i = 0; i < ControlGrid.Children.Count; i++)
            {
                double cardHeight;
                double cardWidth;
                (cardHeight, cardWidth) = EstimateCardHeightAndWidth(true, ActualHeight, ActualWidth);

                //size
                (ControlGrid.Children[i] as MyButton).Height = cardHeight;
                (ControlGrid.Children[i] as MyButton).Width = cardWidth;

                //margin
                if (!horVer)
                {
                    double leftDistance = Distance(i, ControlGrid.Children.Count, true, currentWidth, currentHeight);
                    (ControlGrid.Children[i] as MyButton).Margin = new Thickness(leftDistance, 0, 0, 0);
                }
                else
                {
                    double topDistance = Distance(i, ControlGrid.Children.Count, true, currentWidth, currentHeight);
                    (ControlGrid.Children[i] as MyButton).Margin = new Thickness(0, topDistance, 0, 0);
                }
            }
        }

        #endregion

        #region Changing Back Color

        public void ChangeBackColor(BackColor color)
        {
            cardBack = color;
            ChangeBackColorInAllCards();
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Cards in contol for player " + numberofPlayer.ToString() +
                " Changed to: " + color);
        }

        #endregion
    }
}
