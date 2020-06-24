using CardGraphicsLibraryHandler;
using MakaoInterfaces;
using System;
using System.Collections.Generic;
using System.Windows;

namespace MakaoGraphicsRepresentation.Windows
{
    /// <summary>
    /// Interaction logic for FirstCardJokerPermittedCardsSelection.xaml
    /// </summary>
    public partial class FirstCardJokerPermittedCardsSelection : Window
    {
        #region Fields and constructor

        private readonly List<PlayingCard> permittedCards;
        private PlayingCard jokerChangeCatdTo;

        public FirstCardJokerPermittedCardsSelection(List<PlayingCard> permittedCards)
        {
            this.permittedCards = permittedCards;
            jokerChangeCatdTo = null;
            InitializeComponent();
            PopulateWrapPanel();
        }

        #endregion

        #region Populating Wrap panel with items of permitted cards

        private void PopulateWrapPanel()
        {
            foreach (var item in permittedCards)
            {
                AddSingleItemToWrapPanel(item);
            }
        }

        private void AddSingleItemToWrapPanel(PlayingCard card)
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
                ImageSource = (new CardImageSourceObtainer()).GetImageSource(card),
                Tag = card,
            };

            button.MouseEnter += Button_MouseEnter;
            button.MouseLeave += Button_MouseLeave;
            button.Click += Button_Click;

            PermittedCardsWrapPanel.Children.Add(button);
        }

        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as WrapPanelSelectableItem).ItemHoovered = Visibility.Collapsed;
        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as WrapPanelSelectableItem).ItemHoovered = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResetWrapPanelSelection();
            jokerChangeCatdTo = (PlayingCard)(sender as WrapPanelSelectableItem).Tag;
            (sender as WrapPanelSelectableItem).ItemSelected = Visibility.Visible;
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"Card, that joker will be changed to {jokerChangeCatdTo.ToString()}.");
        }

        //method for resetting highlightening of selected suits
        private void ResetWrapPanelSelection()
        {
            foreach (var item in PermittedCardsWrapPanel.Children)
            {
                if (item is WrapPanelSelectableItem) (item as WrapPanelSelectableItem).ItemSelected = Visibility.Collapsed;
            }
        }

        #endregion

        #region Sending new card event

        public delegate void NewJokerCardEventHandler(object sender, JokerChangeToCardEventArgs e);
        public event NewJokerCardEventHandler NewJokerCard;
        protected virtual void OnNewJokerCard(PlayingCard card)
        {
            NewJokerCard?.Invoke(this, new JokerChangeToCardEventArgs() { NewCard = card });
        }

        #endregion

        #region closing the window

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            OnNewJokerCard(jokerChangeCatdTo);
            Close();
        }

        #endregion
    }

    public class JokerChangeToCardEventArgs : EventArgs
    {
        public PlayingCard NewCard { get; set; }
    }
}
