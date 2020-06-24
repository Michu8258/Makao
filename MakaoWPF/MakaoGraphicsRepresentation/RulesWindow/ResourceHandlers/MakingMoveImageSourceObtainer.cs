using MakaoGraphicsRepresentation.RulesWindow.PagesEnums;
using System;
using System.Windows.Media.Imaging;

namespace MakaoGraphicsRepresentation.RulesWindow.ResourceHandlers
{
    public class MakingMoveImageSourceObtainer
    {
        public BitmapImage GetBackImageSource(MakingMoveEnum moveType)
        {
            BitmapImage output = null;

            try
            {
                output = new BitmapImage(new Uri($"pack://application:,,,/MakaoGraphicsRepresentation;component/ResourcesMovement/{moveType.ToString()}.png"));
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while reading picture of movement type {moveType.ToString()} image from resources: {ex.Message}.");
            }

            return output;
        }
    }
}
