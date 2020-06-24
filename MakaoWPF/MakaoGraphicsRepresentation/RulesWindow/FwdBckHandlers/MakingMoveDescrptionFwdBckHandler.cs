using MakaoGraphicsRepresentation.RulesWindow.PagesEnums;

namespace MakaoGraphicsRepresentation.RulesWindow.FwdBckHandlers
{
    public static class MakingMoveDescrptionFwdBckHandler
    {
        public static (MakingMoveEnum, bool, bool) NextMoveFWD(MakingMoveEnum moveType)
        {
            MakingMoveEnum nextMove = MakingMoveEnum.PossibilityOfMove;
            bool fwdButtonVis = true;
            bool bckButtonVis = true;

            switch (moveType)
            {
                case MakingMoveEnum.PossibilityOfMove: nextMove = MakingMoveEnum.ImpossibilityOfMove; break;
                case MakingMoveEnum.ImpossibilityOfMove: nextMove = MakingMoveEnum.MoreThanOneCard; break;
                case MakingMoveEnum.MoreThanOneCard: nextMove = MakingMoveEnum.ChoosenCards; break;
                case MakingMoveEnum.ChoosenCards: nextMove = MakingMoveEnum.RankDemanding; break;
                case MakingMoveEnum.RankDemanding: nextMove = MakingMoveEnum.SuitDemanding; break;
                case MakingMoveEnum.SuitDemanding: nextMove = MakingMoveEnum.FirstCardJoker; break;
                case MakingMoveEnum.FirstCardJoker: nextMove = MakingMoveEnum.FirstCardJokerChange; break;
                case MakingMoveEnum.FirstCardJokerChange: nextMove = MakingMoveEnum.JokerChange; break;
                case MakingMoveEnum.JokerChange: nextMove = MakingMoveEnum.PlayerCardsWithChangedJoker; break;
                case MakingMoveEnum.PlayerCardsWithChangedJoker: nextMove = MakingMoveEnum.WaitingInStopsMode; break;
                case MakingMoveEnum.WaitingInStopsMode: nextMove = MakingMoveEnum.StopsAmountInfo; break;
                case MakingMoveEnum.StopsAmountInfo: nextMove = MakingMoveEnum.GameEnded; fwdButtonVis = false; break;
                case MakingMoveEnum.GameEnded: nextMove = MakingMoveEnum.GameEnded; fwdButtonVis = false; break;
            }

            return (nextMove, fwdButtonVis, bckButtonVis);
        }

        public static (MakingMoveEnum, bool, bool) NextMoveBCK(MakingMoveEnum moveType)
        {
            MakingMoveEnum nextMove = MakingMoveEnum.PossibilityOfMove;
            bool fwdButtonVis = true;
            bool bckButtonVis = true;

            switch (moveType)
            {
                case MakingMoveEnum.PossibilityOfMove: nextMove = MakingMoveEnum.PossibilityOfMove; bckButtonVis = false; break;
                case MakingMoveEnum.ImpossibilityOfMove: nextMove = MakingMoveEnum.PossibilityOfMove; bckButtonVis = false; break;
                case MakingMoveEnum.MoreThanOneCard: nextMove = MakingMoveEnum.ImpossibilityOfMove; break;
                case MakingMoveEnum.ChoosenCards: nextMove = MakingMoveEnum.MoreThanOneCard; break;
                case MakingMoveEnum.RankDemanding: nextMove = MakingMoveEnum.ChoosenCards; break;
                case MakingMoveEnum.SuitDemanding: nextMove = MakingMoveEnum.RankDemanding; break;
                case MakingMoveEnum.FirstCardJoker: nextMove = MakingMoveEnum.SuitDemanding; break;
                case MakingMoveEnum.FirstCardJokerChange: nextMove = MakingMoveEnum.FirstCardJoker; break;
                case MakingMoveEnum.JokerChange: nextMove = MakingMoveEnum.FirstCardJokerChange; break;
                case MakingMoveEnum.PlayerCardsWithChangedJoker: nextMove = MakingMoveEnum.JokerChange; break;
                case MakingMoveEnum.WaitingInStopsMode: nextMove = MakingMoveEnum.PlayerCardsWithChangedJoker; break;
                case MakingMoveEnum.StopsAmountInfo: nextMove = MakingMoveEnum.WaitingInStopsMode; break;
                case MakingMoveEnum.GameEnded: nextMove = MakingMoveEnum.StopsAmountInfo; break;
            }

            return (nextMove, fwdButtonVis, bckButtonVis);
        }
    }
}
