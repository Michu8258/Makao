using CardGraphicsLibraryHandler;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CardsRepresentation
{
    /// <summary>
    /// Interaction logic for DeckRepresentation.xaml
    /// </summary>
    public partial class DeckRepresentation : UserControl
    {
        public DeckRepresentation()
        {
            InitializeComponent();
            AdjustCardAtStart();
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Deck representation ocntrol initialized");
        }

        //public property - amount of cards can be read
        public int NumberOfCards
        {
            get
            {
                return Int32.Parse(AmountOfCards.Text);
            }
        }

        #region AdjustCard

        //method for adjusting card in the constructor
        private void AdjustCardAtStart()
        {
            if (ActualFrame.Children.Count == 0)
            {
                MyButton myButton = new MyButton();
                (myButton.Height, myButton.Width) = EstimateCardHeightAndWidth(false);
                myButton.BackCardImage = CardBackImageSourceObtainer.GetBackImageSource(BackColor.Blue);
                myButton.VerticalAlignment = VerticalAlignment.Center;
                myButton.HorizontalAlignment = HorizontalAlignment.Center;
                myButton.Margin = new Thickness(0);
                myButton.ClickMode = ClickMode.Press;

                //left mouse preview left mouse button down event 
                //for creation an event that corresponds to taking
                //cards from deck by player
                myButton.Click += new RoutedEventHandler(
                    (s, e) =>
                    {
                        MyButton button = s as MyButton;
                        OnTakeCardClick();
                    });

                ActualFrame.Children.Add(myButton);
            }
        }

        #endregion

        #region Card size

        //method for counting width and height of cards
        private (double, double) EstimateCardHeightAndWidth(bool fromEvent, double height = 726, double width = 500)
        {
            double controlHeight;
            double controlWidth;
            double heightOutput;
            double widthOutput;

            if (!fromEvent)
            {
                controlHeight = Frame.ActualHeight;
                controlWidth = Frame.ActualWidth;
            }
            else
            {
                controlHeight = height;
                controlWidth = width;
            }

            //estimating mode - either width is obstacle or height is obstacle
            double PNGfactor = 0.654356;
            double gridFactor = controlWidth / controlHeight;

            //width is obstacle
            if (gridFactor < PNGfactor)
            {
                widthOutput = controlWidth;
                heightOutput = (widthOutput * 1056) / 691;
            }
            //height is obstacle
            else
            {
                heightOutput = controlHeight;
                widthOutput = (heightOutput * 691) / 1056;
            }

            return (heightOutput, widthOutput);
        }

        #endregion

        #region Changing Back Color

        //method for changing picture that represents back of the cards
        public void ChangeBackColor(BackColor color)
        {
            if (ActualFrame.Children.Count > 0)
            {
                (ActualFrame.Children[0] as MyButton).BackCardImage = CardBackImageSourceObtainer.GetBackImageSource(color);
            }

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Deck representation card color changed to: " + color);
        }

        #endregion

        #region Hadlling resizing of control

        //method for resizing card
        private void Frame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualFrame.Children.Count > 0)
            {
                double currentHeight = (sender as Grid).ActualHeight;
                double currentWidth = (sender as Grid).ActualWidth;

                double cardHeight;
                double cardWidth;
                (cardHeight, cardWidth) = EstimateCardHeightAndWidth(true, currentHeight, currentWidth);

                //size
                InternalFrame.Height = cardHeight;
                InternalFrame.Width = cardWidth;
                (ActualFrame.Children[0] as MyButton).Height = cardHeight;
                (ActualFrame.Children[0] as MyButton).Width = cardWidth;
            }
        }

        #endregion

        #region Displaying number of cards

        //method for changing displayed number of cards in deck
        public void ModifyCardsNumber(int numberOfCards)
        {
            if (numberOfCards < 0)
            {
                throw new ArgumentException("There cannot me less cards than 0 in deck");
            }

            AmountOfCards.Text = numberOfCards.ToString();
        }

        #endregion

        #region PreviewLeftMouseButtonDown event

        public delegate void DeckRepresentationreviewLeftMouseButtonDown(object sender, MainUserEventArgs e);

        public event DeckRepresentationreviewLeftMouseButtonDown TakeCardClick;

        protected virtual void OnTakeCardClick()
        {
            TakeCardClick?.Invoke(this, new MainUserEventArgs { });
        }

        #endregion
    }
}
