using CardGraphicsLibraryHandler;
using MakaoInterfaces;
using System;
using System.Windows;

namespace MakaoGraphicsRepresentation.Windows
{
    /// <summary>
    /// Interaction logic for DemandTheSuitWindow.xaml
    /// </summary>
    public partial class DemandTheSuitWindow : Window
    {
        #region Internal data

        private CardSuits newDemandedSuit = CardSuits.None;

        #endregion

        #region Constructor

        public DemandTheSuitWindow()
        {
            InitializeComponent();
            PopulateSuitWrapPanel();

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Suit demanding choice window initialized");
        }

        #endregion

        #region Suit Wrap panel handling

        private void PopulateSuitWrapPanel()
        {
            foreach (CardSuits suit in (CardSuits[])Enum.GetValues(typeof(CardSuits)))
            {
                if (suit != CardSuits.None)
                {
                    PopulateWrapPanelWithSingleSuit(suit);
                }
            }
        }

        //method for adding one item to wrap panel
        private void PopulateWrapPanelWithSingleSuit(CardSuits suit)
        {
            WrapPanelSelectableItem button = new WrapPanelSelectableItem()
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

            button.MouseEnter += Button_MouseEnter;
            button.MouseLeave += Button_MouseLeave;
            button.Click += Button_Click;

            DemandedSuitWrapPanel.Children.Add(button);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResetSelectionOfSuits();
            newDemandedSuit = (CardSuits)(sender as WrapPanelSelectableItem).Tag;
            (sender as WrapPanelSelectableItem).ItemSelected = Visibility.Visible;
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"New demanded suit: {newDemandedSuit.ToString()}.");
        }

        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as WrapPanelSelectableItem).ItemHoovered = Visibility.Collapsed;
        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as WrapPanelSelectableItem).ItemHoovered = Visibility.Visible;

        }

        //method for resetting highlightening of selected suits
        private void ResetSelectionOfSuits()
        {
            foreach (var item in DemandedSuitWrapPanel.Children)
            {
                if (item is WrapPanelSelectableItem) (item as WrapPanelSelectableItem).ItemSelected = Visibility.Collapsed;
            }
        }

        #endregion

        #region Button clicks

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            newDemandedSuit = CardSuits.None;
            Close();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            OnSuitDemandingWindowClosing(newDemandedSuit);
            Close();
        }

        #endregion

        #region Closing window with event that sends data about new suit

        public delegate void CardSuitDemandingWindowCloseEventHandler(object sender, DemandingSuitWindowsEventArgs e);
        public event CardSuitDemandingWindowCloseEventHandler SuitDemandingWindowClosing;
        protected virtual void OnSuitDemandingWindowClosing(CardSuits demandedSuit)
        {
            SuitDemandingWindowClosing?.Invoke(this, new DemandingSuitWindowsEventArgs { NewSuit = demandedSuit });
        }

        #endregion

        #region Window closed event

        private void Window_Closed(object sender, EventArgs e)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Siut demanding choice window closed");
        }

        #endregion
    }
}
