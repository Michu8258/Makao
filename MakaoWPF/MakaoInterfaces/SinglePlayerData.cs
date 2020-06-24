using System;
using System.Collections.Generic;
using System.Text;

namespace MakaoInterfaces
{
    public class SinglePlayerData
    {
        public int PauseTurnsAmount { get; set; }
        public bool TookFirstCardLostBattle { get; set; }
        public bool ThisPlayerStartedRankDemanding { get; set; }
        public bool ThisPlayerStartedSuitDemanding { get; set; }
        public bool ThisPlayerStartedFours { get; set; }
        public List<PlayingCard> PlayerCards { get; set; }
        public bool CanSkipTheMove { get; set; }
        public bool FirstCardInBattleModeTakenMatches { get; set; }
        public PlayingCard BattleModeMatchingCard { get; set; }
    }
}

