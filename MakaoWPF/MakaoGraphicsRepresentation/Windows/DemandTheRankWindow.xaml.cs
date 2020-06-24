using CardGraphicsLibraryHandler;
using MakaoInterfaces;
using System;
using System.Collections.Generic;
using System.Windows;

namespace MakaoGraphicsRepresentation.Windows
{
    /// <summary>
    /// Interaction logic for DemandTheRankWindow.xaml
    /// </summary>
    public partial class DemandTheRankWindow : Window
    {
        #region Internal data

        private CardRanks newDemandedRank = CardRanks.None;
        private readonly CardSuits jackInputSuit;

        #endregion

        #region Constructor

        public DemandTheRankWindow(CardSuits suit)
        {
            jackInputSuit = suit;

            InitializeComponent();

            PopulateSuitWrapPanel();

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Rank demanding choice window initialized");
        }

        #endregion

        #region Rank wrappanel handling

        private List<CardRanks> GenerateListOfPermittedRanks()
        {
            List<CardRanks> rankList = new List<CardRanks>()
            {
                CardRanks.Five,
                CardRanks.Six,
                CardRanks.Seven,
                CardRanks.Eight,
                CardRanks.Nine,
                CardRanks.Ten,
                CardRanks.Queen,
            };
            return rankList;
        }

        private void PopulateSuitWrapPanel()
        {
            List<CardRanks> rankList = GenerateListOfPermittedRanks();

            foreach (var item in rankList)
            {
                PopulateWrapPanelWithSingleSuit(jackInputSuit, item);
            }
        }

        //method for adding one item to wrap panel
        private void PopulateWrapPanelWithSingleSuit(CardSuits suit, CardRanks rank)
        {
            CardImageSourceObtainer obtainer = new CardImageSourceObtainer();

            WrapPanelSelectableItem button = new WrapPanelSelectableItem()
            {
                Height = 120,
                Width = 100,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0),
                ItemSelected = Visibility.Collapsed,
                ItemHoovered = Visibility.Collapsed,
                ImageSource = obtainer.GetImageSource(rank, suit),
                Tag = rank,
            };

            button.MouseEnter += Button_MouseEnter;
            button.MouseLeave += Button_MouseLeave;
            button.Click += Button_Click;

            DemandedRankWrapPanel.Children.Add(button);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResetSelectionOfSuits();
            newDemandedRank = (CardRanks)(sender as WrapPanelSelectableItem).Tag;
            (sender as WrapPanelSelectableItem).ItemSelected = Visibility.Visible;
        }

        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as WrapPanelSelectableItem).ItemHoovered = Visibility.Collapsed;
        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as WrapPanelSelectableItem).ItemHoovered = Visibility.Visible;

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"New demanded suit: {newDemandedRank.ToString()}.");
        }

        //method for resetting highlightening of selected suits
        private void ResetSelectionOfSuits()
        {
            foreach (var item in DemandedRankWrapPanel.Children)
            {
                if (item is WrapPanelSelectableItem) (item as WrapPanelSelectableItem).ItemSelected = Visibility.Collapsed;
            }
        }

        #endregion

        #region Button clicks

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            newDemandedRank = CardRanks.None;
            Close();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            OnRankDemandingWindowClosing(newDemandedRank);
            Close();
        }

        #endregion

        #region Closing window with event that sends data about new rank

        public delegate void CardRankDemandingWindowCloseEventHandler(object sender, DemandingRankWindowsEventArgs e);
        public event CardRankDemandingWindowCloseEventHandler RankDemandingWindowClosing;
        protected virtual void OnRankDemandingWindowClosing(CardRanks demadedRank)
        {
            RankDemandingWindowClosing?.Invoke(this, new DemandingRankWindowsEventArgs { NewRank = demadedRank });
        }

        #endregion

        #region Window closed event

        private void Window_Closed(object sender, EventArgs e)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Rank demanding choice window closed");
        }

        #endregion
    }
}
