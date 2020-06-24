using MakaoInterfaces;
using NLog;
using System.Collections.Generic;

namespace MakaoEngine
{
    public static class CardListLogger
    {
        public static void LogCardsList(List<PlayingCard> cardsList, string text)
        {
            if (Engine.ExtendedLogging)
            {
                Logger logger = NLog.LogManager.GetCurrentClassLogger();

                if (cardsList.Count> 0)
                {
                    foreach (PlayingCard item in cardsList)
                    {
                        logger.Info($"      {text}: {item.ToString()}.");
                    }
                }
                else
                {
                    logger.Info($"No cards to log.");
                }
            }
        }
    }
}
