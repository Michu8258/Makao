using MakaoInterfaces;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CardGraphicsLibraryHandler
{
    public static class CardSuitsImageSourceObtainer
    {
        public static BitmapImage GetSuitImageSource(CardSuits suit)
        {
            BitmapImage output = null;

            try
            {
                output = new BitmapImage(new Uri($"pack://application:,,,/CardGraphicsLibraryHandler;component/Resources/{suit.ToString()}suit.png"));
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while reading suit image from resources: {ex.Message}.");
            }

            return output;
        }
    }
}
