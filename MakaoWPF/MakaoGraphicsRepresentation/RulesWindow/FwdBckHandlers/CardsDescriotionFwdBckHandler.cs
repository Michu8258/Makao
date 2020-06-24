using MakaoInterfaces;

namespace MakaoGraphicsRepresentation.RulesWindow.FwdBckHandlers
{
    public static class CardsDescriotionFwdBckHandler
    {
        public static (CardRanks, bool, bool) NextRankFWD(CardRanks currentRank)
        {
            CardRanks nextRank = CardRanks.None;
            bool fwdButtonVis = true;
            bool bckButtonVis = true;

            switch (currentRank)
            {
                case CardRanks.Two: nextRank = CardRanks.Three; break;
                case CardRanks.Three: nextRank = CardRanks.Four; break;
                case CardRanks.Four: nextRank = CardRanks.Five; break;
                case CardRanks.Five: nextRank = CardRanks.Six; break;
                case CardRanks.Six: nextRank = CardRanks.Seven; break;
                case CardRanks.Seven: nextRank = CardRanks.Eight; break;
                case CardRanks.Eight: nextRank = CardRanks.Nine; break;
                case CardRanks.Nine: nextRank = CardRanks.Ten; break;
                case CardRanks.Ten: nextRank = CardRanks.Jack; break;
                case CardRanks.Jack: nextRank = CardRanks.Queen; break;
                case CardRanks.Queen: nextRank = CardRanks.King; break;
                case CardRanks.King: nextRank = CardRanks.Ace; break;
                case CardRanks.Ace: nextRank = CardRanks.Joker; fwdButtonVis = false; break;
                case CardRanks.Joker: nextRank = CardRanks.Joker; fwdButtonVis = false; break;
            }

            return (nextRank, fwdButtonVis, bckButtonVis);
        }

        public static (CardRanks, bool, bool) NextRankBCK(CardRanks currentRank)
        {
            CardRanks nextRank = CardRanks.None;
            bool fwdButtonVis = true;
            bool bckButtonVis = true;

            switch (currentRank)
            {
                case CardRanks.Two: nextRank = CardRanks.Two; bckButtonVis = false; break;
                case CardRanks.Three: nextRank = CardRanks.Two; bckButtonVis = false; break;
                case CardRanks.Four: nextRank = CardRanks.Three; break;
                case CardRanks.Five: nextRank = CardRanks.Four; break;
                case CardRanks.Six: nextRank = CardRanks.Five; break;
                case CardRanks.Seven: nextRank = CardRanks.Six; break;
                case CardRanks.Eight: nextRank = CardRanks.Seven; break;
                case CardRanks.Nine: nextRank = CardRanks.Eight; break;
                case CardRanks.Ten: nextRank = CardRanks.Nine; break;
                case CardRanks.Jack: nextRank = CardRanks.Ten; break;
                case CardRanks.Queen: nextRank = CardRanks.Jack; break;
                case CardRanks.King: nextRank = CardRanks.Queen; break;
                case CardRanks.Ace: nextRank = CardRanks.King; break;
                case CardRanks.Joker: nextRank = CardRanks.Ace; break;
            }

            return (nextRank, fwdButtonVis, bckButtonVis);
        }
    }
}
