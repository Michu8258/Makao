using CardGraphicsLibraryHandler;
using MakaoInterfaces;
using System;
using System.Windows.Controls;

namespace MakaoGraphicsRepresentation.RulesWindow.CardsDescriptionsPages
{
    /// <summary>
    /// Interaction logic for TwoCardDescription.xaml
    /// </summary>
    public partial class CardDescriptionPage : Page
    {
        private CardRanks rank;
        private string cardName;
        private string headingText;
        private string descriptionText;

        public CardDescriptionPage(CardRanks rank)
        {
            CreatingPageCommonMethod(rank);
        }

        public void Update(CardRanks newRank)
        {
            CreatingPageCommonMethod(newRank);
        }

        private void CreatingPageCommonMethod(CardRanks newRank)
        {
            rank = newRank;
            InitializeComponent();
            AssignTextSources();
            AssignTextsToTextBlocks();
            AssignCardsPictures();
            LogOpeningOfNewPage();
        }

        private void LogOpeningOfNewPage()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"Opened card description page for rank: {rank.ToString()}.");
        }

        private void AssignTextSources()
        {
            cardName = rank.ToString();
            headingText = $"{cardName}Card01";
            descriptionText = $"{cardName}Card02";
        }

        private void AssignTextsToTextBlocks()
        {
            try
            {
                Heading.Text = Properties.CardDescriptionResource.ResourceManager.GetString(headingText);
                CardDescription.Text = Properties.CardDescriptionResource.ResourceManager.GetString(descriptionText);
            }
            catch (Exception ex)
            {
                Heading.Text = "---";
                CardDescription.Text = "----------";

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while trying to assign texts in card description page for card {rank.ToString()}: {ex.Message}.");
            }
        }

        private void AssignCardsPictures()
        {
            CardImageSourceObtainer obtainer = new CardImageSourceObtainer();

            try
            {
                CardOfSpades.Source = obtainer.GetImageSource(rank, CardSuits.Spade);
                CardOfHearths.Source = obtainer.GetImageSource(rank, CardSuits.Heart);
                CardOfClubs.Source = obtainer.GetImageSource(rank, CardSuits.Club);
                CardOfDiamonds.Source = obtainer.GetImageSource(rank, CardSuits.Diamond);
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while trying to assign cards pictures of {rank.ToString()}: {ex.Message}.");
            }
        }
    }
}
