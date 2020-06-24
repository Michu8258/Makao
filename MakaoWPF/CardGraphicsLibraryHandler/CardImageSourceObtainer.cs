using MakaoInterfaces;
using System;
using System.Windows.Media.Imaging;

namespace CardGraphicsLibraryHandler
{
    public class CardImageSourceObtainer
    {
        public BitmapImage GetImageSource(PlayingCard card)
        {
            return GetImageSourceFromPropertiesObject(card.Suit, card.Rank);
        }

        public BitmapImage GetImageSource(CardRanks rank, CardSuits suit)
        {
            return GetImageSourceFromPropertiesObject(suit, rank);
        }

        public BitmapImage GetGreenHighlight()
        {
            BitmapImage output = null;

            try
            {
                output = new BitmapImage(new Uri($"pack://application:,,,/CardGraphicsLibraryHandler;component/Resources/highlight_of_green.png"));
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while reading green highlight image from resources: {ex.Message}.");
            }

            return output;
        }

        private BitmapImage GetImageSourceFromPropertiesObject(CardSuits suit, CardRanks rank)
        {
            BitmapImage output = null;

            try
            {
                string name = GenerateResourceName(suit, rank);
                output = new BitmapImage(new Uri($"pack://application:,,,/CardGraphicsLibraryHandler;component/Resources/{name}.png"));
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while reading specific card image ({suit.ToString()}, {rank.ToString()}) from resources: {ex.Message}.");
            }

            return output;
        }

        private static string GenerateResourceName(CardSuits suit, CardRanks rank)
        {
            if (rank == CardRanks.Joker) return $"{rank.ToString()}_of_none";
            else if (rank != CardRanks.Joker && rank != CardRanks.None) return $"{rank.ToString()}_of_{suit.ToString()}";
            else return "";
        }
    }
}
