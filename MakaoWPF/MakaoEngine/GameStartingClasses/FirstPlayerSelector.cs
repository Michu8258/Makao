using System;

namespace MakaoEngine.GameStartingClasses
{
    public class FirstPlayerSelector
    {
        public  void ChooseFirstPlayerRandomly(int amountOfPlayers, ref int currentPlayer)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            currentPlayer = random.Next(amountOfPlayers);
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Player choosen as starting one: " + currentPlayer.ToString());
        }
    }
}
