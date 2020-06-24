using MakaoGraphicsRepresentation.RulesWindow.PagesEnums;
using MakaoGraphicsRepresentation.RulesWindow.ResourceHandlers;
using NLog;
using System;
using System.Windows.Controls;

namespace MakaoGraphicsRepresentation.RulesWindow.MovementPages
{
    public class ContentAssigner
    {
        protected Logger logger = LogManager.GetCurrentClassLogger();

        public ContentAssigner(ref TextBlock heading, ref TextBlock description01,
            ref TextBlock description02, ref Image image, MakingMoveEnum moveType)
        {
            AssignContent(ref heading, ref description01, ref description02, ref image, moveType);
        }


        protected void AssignContent(ref TextBlock heading, ref TextBlock description01,
            ref TextBlock description02, ref Image image, MakingMoveEnum moveType)
        {
            AssignPicture(ref image, moveType);
            AssignHeading(ref heading, moveType);
            AssignTexts(ref description01, ref description02, moveType);
            logger.Info($"{moveType.ToString()}Page opened");
        }

        protected void AssignPicture(ref Image image, MakingMoveEnum moveType)
        {
            try
            {
                MakingMoveImageSourceObtainer obtainer = new MakingMoveImageSourceObtainer();
                image.Source = obtainer.GetBackImageSource(moveType);
            }
            catch (Exception ex)
            {
                logger.Error($"Error while trying to assign picture in page {moveType.ToString()}Page: {ex.Message}.");
            }
        }

        protected void AssignHeading(ref TextBlock heading, MakingMoveEnum moveType)
        {
            try
            {
                heading.Text = Properties.MakingMoveDescriptionResource.ResourceManager.GetString($"{moveType.ToString()}Heading");
            }
            catch (Exception ex)
            {
                logger.Error($"Error while trying to assign heading in page {moveType.ToString()}Page: {ex.Message}.");
            }
        }

        protected void AssignTexts(ref TextBlock description01, ref TextBlock description02, MakingMoveEnum moveType)
        {
            try
            {
                description01.Text = Properties.MakingMoveDescriptionResource.ResourceManager.GetString($"{moveType.ToString()}Text01");
                description02.Text = Properties.MakingMoveDescriptionResource.ResourceManager.GetString($"{moveType.ToString()}Text02");
            }
            catch (Exception ex)
            {
                logger.Error($"Error while trying to assign texts in page {moveType.ToString()}Page: {ex.Message}.");
            }
        }
    }
}
