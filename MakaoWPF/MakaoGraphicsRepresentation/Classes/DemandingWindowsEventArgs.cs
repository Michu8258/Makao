using MakaoInterfaces;
using System;

namespace MakaoGraphicsRepresentation
{
    public class DemandingRankWindowsEventArgs : EventArgs
    {
        public CardRanks NewRank;
    }
    public class DemandingSuitWindowsEventArgs : EventArgs
    {
        public CardSuits NewSuit;
    }

}
