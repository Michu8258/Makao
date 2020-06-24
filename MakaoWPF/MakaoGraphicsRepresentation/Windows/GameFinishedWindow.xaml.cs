using MakaoGameClientService.DataTransferObjects;
using MakaoGameClientService.Messages;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MakaoGraphicsRepresentation.Windows
{
    /// <summary>
    /// Interaction logic for GameFinishedWindow.xaml
    /// </summary>
    public partial class GameFinishedWindow : Window
    {
        private bool canWindowBeClosed;
        private readonly string thisPlayerID;
        private readonly GameFinishedDataRequest inputData;

        public GameFinishedWindow(GameFinishedDataRequest inputData, string thisPlayerID)
        {
            InitializeComponent();

            this.inputData = inputData;
            this.canWindowBeClosed = false;
            this.thisPlayerID = thisPlayerID;

            DisplayTotalGameTime(inputData.GameTimerTimeSpan);
            PopulateListVieWithPlayers(inputData);
        }

        #region Displaying players in list

        private void PopulateListVieWithPlayers(GameFinishedDataRequest inputData)
        {
            int amountOfPlayers = inputData.GamersList.Count;

            GamersListView.Items.Clear();

            for (int i = 0; i < amountOfPlayers; i++)
            {
                var item = inputData.GamersList.Single(x => x.PlayerPosition == i + 1);
                GameFinishedListViewItem element = new GameFinishedListViewItem(Pages.PlayersListPage.GetAvatarFullName(item.PlayerNumber),
                    item.PlayerName, item.PlayedGames, item.PlayedAndWonGames, inputData.WinnerPlayerNumber, item.PlayerNumber,
                    item.PlayerID, item.PlayerPosition);

                GamersListView.Items.Add(element);
            }
        }

        #endregion

        #region Displaying of total game time

        //method for displaying total gme duration
        private void DisplayTotalGameTime(TimeSpan time)
        {
            string additionString = "";
            if (time.Hours < 10) additionString += "0";
            additionString += time.Hours.ToString() + ":";
            if (time.Minutes < 10) additionString += "0";
            additionString += time.Minutes.ToString() + ":";
            if (time.Seconds < 10) additionString += "0";
            additionString += time.Seconds.ToString();
            GameDurationLabel.Content += additionString;
        }

        #endregion

        #region Changed selection of list view - no item can be selected

        private void GamersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as ListView).SelectedIndex = -1;
        }

        #endregion

        #region Closing window - raise event, whitch will close game window, and clear page in main window

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //if closing by x button - cancel
            if (!canWindowBeClosed) e.Cancel = true;
        }

        private void WindowClosingButton_Click(object sender, RoutedEventArgs e)
        {
            canWindowBeClosed = true;

            try
            {
                PlayerPositionDetails positionDetails = inputData.GamersList.Single(x => x.PlayerID == thisPlayerID);
                OnGameResultsWindowClosing(positionDetails.PlayedGames, positionDetails.PlayedAndWonGames);
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while trying to get info about this player from input data: {ex.Message}.");

                OnGameResultsWindowClosing(-5, -5);
            }
            this.Close();
        }

        #endregion

        #region Closing window by nutton event

        public delegate void CloseResultsWindowEventHandler(object sender, GameFinishedEventArgs e);
        public event CloseResultsWindowEventHandler GameResultsWindowClosing;
        public void OnGameResultsWindowClosing(int playedGames, int wonGames)
        {
            GameResultsWindowClosing?.Invoke(null, new GameFinishedEventArgs { PlayedGames = playedGames, PlayedAndWonGames = wonGames});
        }

        #endregion
    }
}
