using MakaoGraphicsRepresentation.RulesWindow.CardsDescriptionsPages;
using MakaoGraphicsRepresentation.RulesWindow.FwdBckHandlers;
using MakaoGraphicsRepresentation.RulesWindow.GameStatusDescriptionPages;
using MakaoGraphicsRepresentation.RulesWindow.MovementPages;
using MakaoGraphicsRepresentation.RulesWindow.PagesEnums;
using MakaoInterfaces;
using NLog;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MakaoGraphicsRepresentation.RulesWindow
{
    /// <summary>
    /// Interaction logic for RulesWindow.xaml
    /// </summary>
    public partial class RulesWindow : Window
    {
        #region Fields and Constructor

        private CardRanks currentCardDescriptionRank;
        private GameStatus currenGameStatusDesription;
        private MakingMoveEnum currentMovementTypeDescription;
        private readonly CardDescriptionPage cardDescriptionPageContent;

        private readonly Logger logger;

        public RulesWindow()
        {
            cardDescriptionPageContent = new CardDescriptionPage(CardRanks.Two);

            logger = NLog.LogManager.GetCurrentClassLogger();
            InitializeComponent();
            AssignContentOfCardsDescriptionTab();
            AssignContentOfGameStatusDescriptionPage();
            AssignContentOfMakingMovePage();
            logger.Info($"Rules window opened");
        }

        #endregion

        #region CardsDescription tab

        private void AssignContentOfCardsDescriptionTab()
        {
            //CardsDescriptionFrame.Content = null;
            CardsDescriptionFrame.Content = cardDescriptionPageContent;
            currentCardDescriptionRank = CardRanks.Two;
            CardsDescriptionBckButton.Visibility = Visibility.Collapsed;
        }

        //changing page - forward direction
        private void CardsDescriptionFwdBckButton_Click(object sender, RoutedEventArgs e)
        {
            bool fwdVis, bckVis;
            (currentCardDescriptionRank, fwdVis, bckVis) = CardsDescriotionFwdBckHandler.NextRankFWD(currentCardDescriptionRank);
            ChangeCardRescriptionPageCommonActions(fwdVis, bckVis);
        }

        //changing page - backward direction
        private void CardsDescriptionBckBckButton_Click(object sender, RoutedEventArgs e)
        {
            bool fwdVis, bckVis;
            (currentCardDescriptionRank, fwdVis, bckVis) = CardsDescriotionFwdBckHandler.NextRankBCK(currentCardDescriptionRank);
            ChangeCardRescriptionPageCommonActions(fwdVis, bckVis);
        }

        private void ChangeCardRescriptionPageCommonActions(bool fwdVis, bool bckVis)
        {
            cardDescriptionPageContent.Update(currentCardDescriptionRank);
            ManageSwitchingPagesButtonsVisibility(CardsDescriptionFwdButton, fwdVis, CardsDescriptionBckButton, bckVis);
        }

        #endregion

        #region Game status description tab

        //assigning page for first time
        private void AssignContentOfGameStatusDescriptionPage()
        {
            currenGameStatusDesription = GameStatus.Standard;
            GameStatusDescriptionFrame.Content = new GameStatusDescriptionPage(GameStatus.Standard);
            GameStatusDescriptionBckButton.Visibility = Visibility.Collapsed;
        }

        //changing page - forward direction
        private void GameStatusDescriptionFwdButton_Click(object sender, RoutedEventArgs e)
        {
            bool fwdVis, bckVis;
            (currenGameStatusDesription, fwdVis, bckVis) = GameStatusDescriptionFwdBckHandler.NextStatusFWD(currenGameStatusDesription);
            ChangeGameStatusDescriptionCommonActions(fwdVis, bckVis);
        }

        //changing page - backward direction
        private void GameStatusDescriptionBckButton_Click(object sender, RoutedEventArgs e)
        {
            bool fwdVis, bckVis;
            (currenGameStatusDesription, fwdVis, bckVis) = GameStatusDescriptionFwdBckHandler.NextStatusBCK(currenGameStatusDesription);
            ChangeGameStatusDescriptionCommonActions(fwdVis, bckVis);
        }

        private void ChangeGameStatusDescriptionCommonActions(bool fwdVis, bool bckVis)
        {
            GameStatusDescriptionPage content = new GameStatusDescriptionPage(currenGameStatusDesription);
            GameStatusDescriptionFrame.Content = content;
            ManageSwitchingPagesButtonsVisibility(GameStatusDescriptionFwdButton, fwdVis, GameStatusDescriptionBckButton, bckVis);
        }

        #endregion

        #region Make a move description tab

        //assigning page for first time
        private void AssignContentOfMakingMovePage()
        {
            MakingMovePagesHandler handler = new MakingMovePagesHandler();
            currentMovementTypeDescription = MakingMoveEnum.PossibilityOfMove;
            MakingMoveDescriptionFrame.Content = handler.GetMakingMoveProperPage(currentMovementTypeDescription);
            MakingMovesDescriptionBckButton.Visibility = Visibility.Collapsed;
        }

        private void MakingMovesDescriptionBckButton_Click(object sender, RoutedEventArgs e)
        {
            bool fwdVis, bckVis;
            (currentMovementTypeDescription, fwdVis, bckVis) = MakingMoveDescrptionFwdBckHandler.NextMoveBCK(currentMovementTypeDescription);
            MakingMoveDescriptionCommonActions(fwdVis, bckVis);
        }

        private void MakingMoveDescriptionFwdButton_Click(object sender, RoutedEventArgs e)
        {
            bool fwdVis, bckVis;
            (currentMovementTypeDescription, fwdVis, bckVis) = MakingMoveDescrptionFwdBckHandler.NextMoveFWD(currentMovementTypeDescription);
            MakingMoveDescriptionCommonActions(fwdVis, bckVis);
        }

        private void MakingMoveDescriptionCommonActions(bool fwdVis, bool bckVis)
        {
            MakingMovePagesHandler handler = new MakingMovePagesHandler();
            MakingMoveDescriptionFrame.Content = handler.GetMakingMoveProperPage(currentMovementTypeDescription);
            ManageSwitchingPagesButtonsVisibility(MakingMoveDescriptionFwdButton, fwdVis, MakingMovesDescriptionBckButton, bckVis);
        }

        #endregion

        #region Common method for button visibility management

        //manage visibility of buttons
        private void ManageSwitchingPagesButtonsVisibility(Button fwdButton, bool fwdButtonVis, Button bckButton, bool bckButtonVis)
        {
            if (fwdButtonVis) fwdButton.Visibility = Visibility.Visible;
            else fwdButton.Visibility = Visibility.Collapsed;

            if (bckButtonVis) bckButton.Visibility = Visibility.Visible;
            else bckButton.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Common event handler method for flushing memory when changing tabs

        private void ChangeTabItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MemoryManagement.FlushMemory();
        }

        #endregion

        #region Event for closing te window

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnWindowClosing();
        }

        #endregion

        #region Event for passing info to main window about closing the window

        public delegate void WindowClosingEventHandler(object sender, EventArgs e);
        public event WindowClosingEventHandler WindowClosing;
        protected virtual void OnWindowClosing()
        {
            WindowClosing?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}