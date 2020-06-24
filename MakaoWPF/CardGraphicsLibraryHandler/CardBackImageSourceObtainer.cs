using System;
using System.Windows.Media.Imaging;


namespace CardGraphicsLibraryHandler
{
    public static class CardBackImageSourceObtainer
    {
        public static BitmapImage GetBackImageSource(BackColor color)
        {
            BitmapImage output = null;

            try
            {
                output = new BitmapImage(new Uri($"pack://application:,,,/CardGraphicsLibraryHandler;component/Resources/backgraphics{color.ToString()}.png"));
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while reading back card image {color.ToString()} image from resources: {ex.Message}.");
            }

            return output;
        }
    }
}
