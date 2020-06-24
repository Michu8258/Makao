using MakaoInterfaces;
using NLog;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MakaoGraphicsRepresentation.RulesWindow.GameStatusDescriptionPages
{
    /// <summary>
    /// Interaction logic for GameStatusDescriptionPage.xaml
    /// </summary>
    public partial class GameStatusDescriptionPage : Page
    {
        private readonly GameStatus status;
        private string statusName;
        private string headingText;
        private string descriptionText1;
        private string descriptionText2;
        private readonly Logger logger;

        public GameStatusDescriptionPage(GameStatus status)
        {
            this.status = status;
            logger = LogManager.GetCurrentClassLogger();
            InitializeComponent();
            AssignTextNames();
            AssignTextsToControls();
            AssignPictureToImageControl();
            LogOpeningOfNewPage();
        }

        private void LogOpeningOfNewPage()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"Opened card description page for game status: {status.ToString()}.");
        }

        private void AssignTextNames()
        {
            statusName = status.ToString();
            headingText = $"{statusName}Status01";
            descriptionText1 = $"{statusName}Status02";
            descriptionText2 = $"{statusName}Status03";
        }

        private void AssignTextsToControls()
        {
            try
            {
                Heading.Text = Properties.GameStatusDescriptionResource.ResourceManager.GetString(headingText);
                DescriptionText01.Text = Properties.GameStatusDescriptionResource.ResourceManager.GetString(descriptionText1);
                DescriptionText02.Text = Properties.GameStatusDescriptionResource.ResourceManager.GetString(descriptionText2);
            }
            catch (Exception ex)
            {
                logger.Error($"Error while trying to read texts from resource file: {ex.Message}.");
            }
        }

        private void AssignPictureToImageControl()
        {
            try
            {
                GameStatusPicture.Source = new BitmapImage(new Uri($"pack://application:,,,/MakaoGraphicsRepresentation;component/Resources/{status.ToString()}status.png"));
            }
            catch (Exception ex)
            {
                logger.Error($"Error while assigning game status picture to image control: {ex.Message}.");
            }
        }
    }
}
