using CardGraphicsLibraryHandler;
using CardsRepresentation;
using MakaoGameClientService.DataTransferObjects;
using MakaoGameClientService.Messages;
using MakaoGraphicsRepresentation.MainWindowData;
using MakaoGraphicsRepresentation.Pages;
using MakaoGraphicsRepresentation.Windows;
using MakaoInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;

namespace MakaoGraphicsRepresentation.GameWIndowClasses
{
    class GameWindowConstructor
    {
        //window instance holding field
        private readonly GameWindow gameWindow;

        #region Constructor

        public GameWindowConstructor(GameWindow window, PersonalizedForSpecificPlayerStartGameDataRequest inputData,
            ref bool willBeClosedByHost, ref int amoutOfPlayersShown, ref int thisPlayerNumber, ref string thisPlayerID, ref string thisPlayerName,
            ref int minimumPlayerNumber, ref int maximumPlayerNumber, ref Dictionary<int, OtherPlayer> PlayersControlsMapper,
            ThirdPlayerLocation locationOfThirdPlayer, BackColor cardsBackColor, ref Dictionary<int, Rectangle> ActivePlayerRectangles,
            ref GameStateData GameStateData, ref List<PlayingCard> CardsChoosenByThisPlayer, ref CardRanks demandedRankRequest,
            ref CardSuits demandedSuitRequest, ref GameStateDataTranslator GameDataTranslator)
        {
            gameWindow = window;

            //Data context
            AssignControlsDataContext();

            //at the window starts, if it is closed, closing is caused by user
            willBeClosedByHost = false;

            //initializing list of cards choosen by this player
            CardsChoosenByThisPlayer = new List<PlayingCard>();

            //assign demanding options
            demandedRankRequest = CardRanks.None;
            demandedSuitRequest = CardSuits.None;

            //assign other player cards control type (horizontal/vertical)
            AssignOtherPlayersControlsOrientation();

            //assigning current game state data for first time
            GameStateData = inputData.CurrentGameStatusData;
            GameDataTranslator = new GameStateDataTranslator();

            //set local fields based on data passed in
            InitializeFields(inputData, ref amoutOfPlayersShown, ref thisPlayerNumber, ref thisPlayerID, ref thisPlayerName,
                ref minimumPlayerNumber, ref maximumPlayerNumber);

            //set controls visibility
            ManageControlVisibility(locationOfThirdPlayer, ref amoutOfPlayersShown);

            //assign player number properties of controlls
            AssignPlayersNumberIntoPropertiesOfControls(inputData, ref PlayersControlsMapper, ref maximumPlayerNumber, ref thisPlayerNumber,
                ref minimumPlayerNumber, ref ActivePlayerRectangles);

            //Assigning color of cards back
            AssignBackCardColor(cardsBackColor);

            //assign players Images and players names
            AssignPlayersAvatarsImagesAndNames(inputData);

            //assigning colors of rectangles that are backrounds of players names
            PlayersRectanglesColorAssigner.AssignPlayersRectanglesColors(ref ActivePlayerRectangles, inputData.CurrentPlayerNumber);

            //enabling this player controls
            ThisUserControlsEnabler.EnableOrDisableThisUserControls(ref gameWindow.ThisPlayerControl,
                ref gameWindow.DeckRepresentationControl, inputData.CurrentPlayerNumber);
        }

        #endregion

        #region Assign data c0ntext to binded controls

        private void AssignControlsDataContext()
        {
            gameWindow.DemandedRankTextBlock.DataContext = gameWindow;
            gameWindow.DemRank.DataContext = gameWindow;
            gameWindow.DemandedSuitTextBlock.DataContext = gameWindow;
            gameWindow.DemSuit.DataContext = gameWindow;
            gameWindow.AmountOfCardsTextBlock.DataContext = gameWindow;
            gameWindow.AmountsOfCardsLostBattle.DataContext = gameWindow;
            gameWindow.ThisPlayerIsPausingRectangle.DataContext = gameWindow;
            gameWindow.StopsTextBlock.DataContext = gameWindow;
            gameWindow.StopsAmountTextBlock.DataContext = gameWindow;
            gameWindow.ThisPlayerIsPausingRectangle.DataContext = gameWindow;
            gameWindow.PlayerStopsGrid.DataContext = gameWindow;
            gameWindow.PauseAmountPauseRectangle.DataContext = gameWindow;
            gameWindow.StopsAmountTextBlock.DataContext = gameWindow;
            gameWindow.ThisPlayerDemandedRankText.DataContext = gameWindow;
            gameWindow.ThisPlayerDemandedRank.DataContext = gameWindow;
            gameWindow.ThisPlayerDemandedSuitText.DataContext = gameWindow;
            gameWindow.ThisPlayerDemandedSuit.DataContext = gameWindow;
            gameWindow.MakeAMoveButton.DataContext = gameWindow;
            gameWindow.StatusText.DataContext = gameWindow;
            gameWindow.SkipTheMoveButton.DataContext = gameWindow;
        }

        #endregion

        #region Assigning data to fields

        //method for setting fields based on constructor's inputdata
        private void InitializeFields(PersonalizedForSpecificPlayerStartGameDataRequest inputData, ref int amoutOfPlayersShown,
            ref int thisPlayerNumber, ref string thisPlayerID, ref string thisPlayerName, ref int minimumPlayerNumber,
            ref int maximumPlayerNumber)
        {
            //check amount of players
            if (inputData.AmountOfPlayers == inputData.DataOfOtherPlayers.Count + 1) amoutOfPlayersShown = inputData.AmountOfPlayers;
            else amoutOfPlayersShown = inputData.DataOfOtherPlayers.Count + 1;

            //assign other player information data
            thisPlayerNumber = inputData.DataOfThisPlayer.ThisPlayerNumber;
            thisPlayerID = inputData.DataOfThisPlayer.ThisPlayerID;
            thisPlayerName = inputData.DataOfThisPlayer.ThisPlayerName;

            //assign min and max player number
            minimumPlayerNumber = inputData.MinimumPlayerNumber;
            maximumPlayerNumber = inputData.MaximumPlayerNumber;
        }

        #endregion

        #region Controls Visibility and modes

        //assigning orientation of players controls (those which shows amount of cards)
        private void AssignOtherPlayersControlsOrientation()
        {
            gameWindow.LeftPlayerControl.HorizontalVertical = true;
            gameWindow.UpperPlayerControl.HorizontalVertical = false;
            gameWindow.RightPlayerControl.HorizontalVertical = true;
        }

        //method for displaying or collapsing UI controls - depends of users preferences and amunt o players
        private void ManageControlVisibility(ThirdPlayerLocation locationOfThirdPlayer, ref int amoutOfPlayersShown)
        {
            gameWindow.ThisPlayerArea.Visibility = Visibility.Visible;
            gameWindow.UpperPlayerArea.Visibility = Visibility.Visible;
            switch (amoutOfPlayersShown)
            {
                case 1: throw new ArgumentOutOfRangeException("There can not be less than 2 players in the game!");
                case 2:
                    gameWindow.LeftPlayerArea.Visibility = Visibility.Collapsed;
                    gameWindow.RightPlayerArea.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    if (locationOfThirdPlayer == ThirdPlayerLocation.Left) gameWindow.RightPlayerArea.Visibility = Visibility.Collapsed;
                    else gameWindow.LeftPlayerArea.Visibility = Visibility.Collapsed;
                    break;
                default: break;
            }
        }

        //method for assigning back color of cards
        private void AssignBackCardColor(BackColor color)
        {
            gameWindow.LeftPlayerControl.ChangeBackColor(color);
            gameWindow.UpperPlayerControl.ChangeBackColor(color);
            gameWindow.RightPlayerControl.ChangeBackColor(color);
            gameWindow.DeckRepresentationControl.ChangeBackColor(color);
        }

        #endregion

        #region Assigning images and player names

        //assigning images for players avatars
        private void AssignPlayersAvatarsImagesAndNames(PersonalizedForSpecificPlayerStartGameDataRequest inputData)
        {
            //this player
            if (gameWindow.ThisPlayerControl.PlayerNumber >= 0)
            {
                gameWindow.ThisPlayerImage.Source = PlayersListPage.GetAvatarFullName(gameWindow.ThisPlayerControl.PlayerNumber);
                gameWindow.ThisPlayerNameControl.Text = inputData.DataOfThisPlayer.ThisPlayerName;
            }

            //left player
            if (gameWindow.LeftPlayerControl.PlayerNumber >= 0)
            {
                gameWindow.LeftPlayerImage.Source = PlayersListPage.GetAvatarFullName(gameWindow.LeftPlayerControl.PlayerNumber);
                gameWindow.LeftPlayerNameControl.Text = (inputData.DataOfOtherPlayers.Single(x => x.OtherPlayerNumber == gameWindow.LeftPlayerControl.PlayerNumber)).OtherPlayerName;
            }

            //upper player
            if (gameWindow.UpperPlayerControl.PlayerNumber >= 0)
            {
                gameWindow.UpperPlayerImage.Source = PlayersListPage.GetAvatarFullName(gameWindow.UpperPlayerControl.PlayerNumber);
                gameWindow.UpperPlayerNameControl.Text = (inputData.DataOfOtherPlayers.Single(x => x.OtherPlayerNumber == gameWindow.UpperPlayerControl.PlayerNumber)).OtherPlayerName;
            }

            //right player
            if (gameWindow.RightPlayerControl.PlayerNumber >= 0)
            {
                gameWindow.RightPlayerImage.Source = PlayersListPage.GetAvatarFullName(gameWindow.RightPlayerControl.PlayerNumber);
                gameWindow.RightPlayerNameControl.Text = (inputData.DataOfOtherPlayers.Single(x => x.OtherPlayerNumber == gameWindow.RightPlayerControl.PlayerNumber)).OtherPlayerName;
            }
        }

        #endregion

        #region Populating dictionaries

        //Assigning player numbers ito controls and into identification list
        private void AssignPlayersNumberIntoPropertiesOfControls(PersonalizedForSpecificPlayerStartGameDataRequest inputData,
            ref Dictionary<int, OtherPlayer> PlaerControlsMapper, ref int maximumPlayerNumber, ref int thisPlayerNumber, 
            ref int minimumPlayerNumber, ref Dictionary<int, Rectangle> ActivePlayerRectangles)
        {
            PlaerControlsMapper = new Dictionary<int, OtherPlayer>();
            ActivePlayerRectangles = new Dictionary<int, Rectangle>();

            //this user handling
            gameWindow.ThisPlayerControl.AssignPlayerNumber(inputData.DataOfThisPlayer.ThisPlayerNumber);
            ActivePlayerRectangles.Add(inputData.DataOfThisPlayer.ThisPlayerNumber, gameWindow.ThisPlayerRectangle);

            //other players handling - clockwise
            NextPlayerNumberSpecifier NextNumber = new NextPlayerNumberSpecifier(maximumPlayerNumber, thisPlayerNumber, minimumPlayerNumber);
            foreach (OtherPlayerData item in inputData.DataOfOtherPlayers)
            {
                int nextPlayerNumber = NextNumber.GetNextPlayerNumber();

                if (gameWindow.LeftPlayerArea.Visibility == Visibility.Visible && gameWindow.LeftPlayerControl.PlayerNumber < 0) //left controll
                {
                    gameWindow.LeftPlayerControl.AssignPlayerNumber(nextPlayerNumber);
                    PlaerControlsMapper.Add(nextPlayerNumber, gameWindow.LeftPlayerControl);
                    ActivePlayerRectangles.Add(nextPlayerNumber, gameWindow.LeftPlayerRectangle);
                }
                else if (gameWindow.UpperPlayerArea.Visibility == Visibility.Visible && gameWindow.UpperPlayerControl.PlayerNumber < 0) //upper control
                {
                    gameWindow.UpperPlayerControl.AssignPlayerNumber(nextPlayerNumber);
                    PlaerControlsMapper.Add(nextPlayerNumber, gameWindow.UpperPlayerControl);
                    ActivePlayerRectangles.Add(nextPlayerNumber, gameWindow.UpperPlayerRectangle);
                }
                else if (gameWindow.RightPlayerArea.Visibility == Visibility.Visible && gameWindow.RightPlayerControl.PlayerNumber < 0) //right control
                {
                    gameWindow.RightPlayerControl.AssignPlayerNumber(nextPlayerNumber);
                    PlaerControlsMapper.Add(nextPlayerNumber, gameWindow.RightPlayerControl);
                    ActivePlayerRectangles.Add(nextPlayerNumber, gameWindow.UpperPlayerRectangle);
                }
            }
        }

        #endregion
    }
}
