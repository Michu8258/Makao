using MakaoInterfaces;
using System.Collections.Generic;

namespace MakaoEngine.RulesHandling
{
    public class HoldingMovesReseter
    {
        public void ResetHoldingMovesForAllPlayers(ref Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            foreach (var item in PlayersCurrentData)
            {
                item.Value.BattleModeMatchingCard = null;
                item.Value.FirstCardInBattleModeTakenMatches = false;
            }
        }
    }
}
