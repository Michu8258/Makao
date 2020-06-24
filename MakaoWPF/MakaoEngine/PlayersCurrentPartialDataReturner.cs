using MakaoInterfaces;
using System.Collections.Generic;

namespace MakaoEngine
{
    public static class PlayersCurrentPartialDataReturner
    {
        public static Dictionary<int, List<PlayingCard>> GetGamersCardsDictionary(Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            Dictionary<int, List<PlayingCard>> output = new Dictionary<int, List<PlayingCard>>();
            foreach (var item in PlayersCurrentData)
            {
                output.Add(item.Key, item.Value.PlayerCards);
            }
            return output;
        }
    }
}
