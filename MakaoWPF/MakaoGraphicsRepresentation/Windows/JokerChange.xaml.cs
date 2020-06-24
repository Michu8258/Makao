using MakaoInterfaces;
using System;
using System.Windows;
using CardGraphicsLibraryHandler;

namespace MakaoGraphicsRepresentation.Windows
{
    /// <summary>
    /// Interaction logic for JokerChange.xaml
    /// </summary>
    public partial class JokerChange : Window
    {
        #region Internal data

        //internal data
        private CardRanks thisRank;
        private CardSuits thisSuit;

        #endregion

        //constructor
        public JokerChange()
        {
            InitializeComponent();

            thisRank = CardRanks.None;
            thisSuit = CardSuits.None;

            PopulateSuitsWrapPanel();

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Joker changing window initialized");
        }

        #region Scroll viewer with suits handling

        //method for adding all suit graphics to suit wrap panel control
        private void PopulateSuitsWrapPanel()
        {
            foreach (CardSuits suit in (CardSuits[])Enum.GetValues(typeof(CardSuits)))
            {
                if (suit != CardSuits.None)
                {
                    AddSingleItemToSuitWrapPanel(suit);
                }
            }
        }

        //method for adding one item to suit wrap panel
        private void AddSingleItemToSuitWrapPanel(CardSuits suit)
        {
            WrapPanelSelectableItem button = new WrapPanelSelectableItem
            {
                Height = 120,
                Width = 100,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0),
                ItemSelected = Visibility.Collapsed,
                ItemHoovered = Visibility.Collapsed,
                ImageSource = CardSuitsImageSourceObtainer.GetSuitImageSource(suit),
                Tag = suit,
            };

            //mouse enter and leave event
            button.MouseEnter += SuitButton_MouseEnter;
            button.MouseLeave += SuitButton_MouseLeave;
            button.Click += SuitButton_Click;

            SuitWrapPanel.Children.Add(button);
        }

        //method for hoovering - hide hoover on mouse leave
        private void SuitButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as WrapPanelSelectableItem).ItemHoovered = Visibility.Collapsed;
        }

        //method for hoovering - show hoover on mouse enter
        private void SuitButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as WrapPanelSelectableItem).ItemHoovered = Visibility.Visible;
        }

        //clicking proper game suit - selection of card suit
        private void SuitButton_Click(object sender, RoutedEventArgs e)
        {
            thisSuit = (CardSuits)(sender as WrapPanelSelectableItem).Tag;
            thisRank = CardRanks.None;
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"New choosen Card suit: {thisSuit.ToString()}.");
            ResetSelectionOfSuits();
            (sender as WrapPanelSelectableItem).ItemSelected = Visibility.Visible;
            RemoveAllChildrenFromRankScrllView();
            PopulateRankWrapPanel();
        }

        //method for resetting highlightening of selected suits
        private void ResetSelectionOfSuits()
        {
            foreach (var item in SuitWrapPanel.Children)
            {
                if (item is WrapPanelSelectableItem) (item as WrapPanelSelectableItem).ItemSelected = Visibility.Collapsed;
            }
        }

        #endregion

        #region Scroll viewer with ranks handling

        //method for deleting all items from rank scroll view
        private void RemoveAllChildrenFromRankScrllView()
        {
            RankWrapPanel.Children.Clear();
        }

        //method for adding all suit graphics to suit wrap panel control
        private void PopulateRankWrapPanel()
        {
            foreach (CardRanks rank in (CardRanks[])Enum.GetValues(typeof(CardRanks)))
            {
                if (rank != CardRanks.None && rank != CardRanks.Joker)
                {
                    AddSingleItemToRankWrapPanel(rank);
                }
            }
        }

        //method for adding one item to suit wrap panel
        private void AddSingleItemToRankWrapPanel(CardRanks rank)
        {
            CardImageSourceObtainer obtainer = new CardImageSourceObtainer();

            WrapPanelSelectableItem button = new WrapPanelSelectableItem
            {
                Height = 120,
                Width = 100,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0),
                ItemSelected = Visibility.Collapsed,
                ItemHoovered = Visibility.Collapsed,
                ImageSource = obtainer.GetImageSource(rank, thisSuit),
                Tag = rank,
            };

            //mouse enter and leave event
            button.MouseEnter += RankButton_MouseEnter;
            button.MouseLeave += RankButton_MouseLeave;
            button.Click += RankButton_Click;

            RankWrapPanel.Children.Add(button);
        }


        //method for hoovering - hide hoover on mouse leave
        private void RankButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as WrapPanelSelectableItem).ItemHoovered = Visibility.Collapsed;
        }

        //method for hoovering - show hoover on mouse enter
        private void RankButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as WrapPanelSelectableItem).ItemHoovered = Visibility.Visible;
        }

        //clicking proper game suit - selection of card suit
        private void RankButton_Click(object sender, RoutedEventArgs e)
        {
            thisRank = (CardRanks)(sender as WrapPanelSelectableItem).Tag;
            ResetSelectionOfRanks();
            (sender as WrapPanelSelectableItem).ItemSelected = Visibility.Visible;
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"New choosen Card rank: {thisRank.ToString()}.");
        }

        //method for resetting highlightening of selected suits
        private void ResetSelectionOfRanks()
        {
            foreach (var item in RankWrapPanel.Children)
            {
                if (item is WrapPanelSelectableItem) (item as WrapPanelSelectableItem).ItemSelected = Visibility.Collapsed;
            }
        }

        #endregion

        #region Button Clicks

        //approvement of choice
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (thisRank != CardRanks.None && thisSuit != CardSuits.None)
            {
                OnJokerWindowClosing(thisRank, thisSuit);
                Close();
            }
            else
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Warn("Either new rank or new suit is set to .None");
                MessageBox.Show("Zarówno figura, jak i kolor muszą być wybrane.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        //canceling the choice
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            thisSuit = CardSuits.None;
            thisRank = CardRanks.None;
            Close();
        }

        #endregion

        #region Closing Window With New Event

        public delegate void JokerWindowEventHandlerWindowClose(object sender, JokerWindowEventArgs e);
        public event JokerWindowEventHandlerWindowClose JokerWindowClosing;
        protected virtual void OnJokerWindowClosing(CardRanks cardRank, CardSuits cardSuit)
        {
            JokerWindowClosing?.Invoke(this, new JokerWindowEventArgs { CardRank = cardRank, CardSuit = cardSuit });
        }

        #endregion

        #region Window Closed event

        private void Window_Closed(object sender, System.EventArgs e)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Joker changing window closed");
        }

        #endregion
    }
}
