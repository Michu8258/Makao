using CardsRepresentation;
using MakaoGameClientService.DataTransferObjects;
using MakaoGraphicsRepresentation.Windows;
using MakaoInterfaces;
using System.Collections.Generic;

namespace MakaoGraphicsRepresentation.GameWIndowClasses
{
    class CardsToControlsAssigner
    {
        private readonly GameWindow gameWindow;

        #region Constructor

        public CardsToControlsAssigner(GameWindow gameWindow)
        {
            this.gameWindow = gameWindow;
        }

        #endregion

        #region Public methods

        public void AssignPlayersCards(Dictionary<int, OtherPlayer> PlayersControlsMapper, List<PlayingCard> thisPlayerCards,
            List<OtherPlayerData> otherPlayerCards)
        {
            AssignAllPlayersCardsToControls(PlayersControlsMapper, thisPlayerCards, otherPlayerCards);
        }

        public void AssignAmountOfCardsInDeck(int amount)
        {
            AssignCardsInDeckAmount(amount);
        }

        public void AddOneCardToUsedCardsControl(PlayingCard card)
        {
            AddSingleCardToCardsOnTheTableControl(card);
        }

        public void AddMultipleCardsToUsedCardsControl(List<PlayingCard> cardsList)
        {
            AssignCardsToCardsOnTheTableControl(cardsList);
        }

        public void AssignCardsToCurrentlyChoosenCardsControl(List<PlayingCard> cardsList)
        {
            gameWindow.ThisPlayerChoosenCardsControl.AssignCardsContent(cardsList);
        }

        public void DeleteAllCardsFromCurrentlyChoosenCardsControl()
        {
            gameWindow.ThisPlayerChoosenCardsControl.EraseCrdsContent();
        }

        #endregion

        #region Private methods

        //assigning cards to all user controls, specific cards for this user
        //and only amount of cards for other players
        private void AssignAllPlayersCardsToControls(Dictionary<int, OtherPlayer> PlayersControlsMapper, 
            List<PlayingCard> thisPlayerCards, List<OtherPlayerData> otherPlayerCards)
        {
            //assigning this player cards
            AssignThisPlayersCardsToControl(thisPlayerCards);

            //and other player cards
            AssignOtherPlayerCardsToCOntrols(PlayersControlsMapper, otherPlayerCards);
        }

        //assigning specific user cards to control
        private void AssignThisPlayersCardsToControl(List<PlayingCard> Cards)
        {
            gameWindow.ThisPlayerControl.RemoveAllCardsFromControl();
            gameWindow.ThisPlayerControl.AddCardsToCOntrol(Cards);
            gameWindow.ThisPlayerControl.ResetAllHighlights();
        }

        //assigning amount of cards for other user controls
        private void AssignOtherPlayerCardsToCOntrols(Dictionary<int, OtherPlayer> PlayersControlsMapper, List<OtherPlayerData> otherPlayerCards)
        {
            foreach (OtherPlayerData item in otherPlayerCards)
            {
                (PlayersControlsMapper[item.OtherPlayerNumber] as OtherPlayer).AdjustCardsAmount(item.OtherPlayerAmountOfCards);
            }
        }
        
        //assigning amount of cards that is in deck to deck representation control
        private void AssignCardsInDeckAmount(int amount)
        {
            gameWindow.DeckRepresentationControl.ModifyCardsNumber(amount);
        }

        //adding only one card to used cards control
        private void AddSingleCardToCardsOnTheTableControl(PlayingCard card)
        {
            List<PlayingCard> cardsList = new List<PlayingCard>()
            {
                card,
            };
            AssignCardsToCardsOnTheTableControl(cardsList);
        }

        //adding multiple cards to used cards control
        private void AssignCardsToCardsOnTheTableControl(List<PlayingCard> cardsList)
        {
            gameWindow.AlreadyUsedCardsControl.AddRange(cardsList);
        }
        
        #endregion
    }
}
