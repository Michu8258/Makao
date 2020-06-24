using System;
using System.Windows.Media.Imaging;

namespace CardGraphicsLibraryHandler
{
    public class BackgroundImageObtainer
    {
        public BitmapImage GetBackgroundGraphics()
        {
            BitmapImage output = null;

            try
            {
                output = new BitmapImage(new Uri("pack://application:,,,/CardGraphicsLibraryHandler;component/Resources/mainbackground.png"));
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while reading main window background image from resources: {ex.Message}.");
            }

            return output;
        }
    }
}

