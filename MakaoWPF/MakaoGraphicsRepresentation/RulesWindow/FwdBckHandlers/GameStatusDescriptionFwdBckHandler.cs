using MakaoInterfaces;

namespace MakaoGraphicsRepresentation.RulesWindow.FwdBckHandlers
{
    public static class GameStatusDescriptionFwdBckHandler
    {
        public static (GameStatus, bool, bool) NextStatusFWD(GameStatus status)
        {
            GameStatus nextStatus = GameStatus.Standard;
            bool fwdButtonVis = true;
            bool bckButtonVis = true;

            switch (status)
            {
                case GameStatus.Standard: nextStatus = GameStatus.RankDemanding; break;
                case GameStatus.RankDemanding: nextStatus = GameStatus.SuitDemanding; break;
                case GameStatus.SuitDemanding: nextStatus = GameStatus.Stops; break;
                case GameStatus.Stops: nextStatus = GameStatus.Battle; fwdButtonVis = false; break;
                case GameStatus.Battle: nextStatus = GameStatus.Battle; fwdButtonVis = false; break;
            }

            return (nextStatus, fwdButtonVis, bckButtonVis);
        }

        public static (GameStatus, bool, bool) NextStatusBCK(GameStatus status)
        {
            GameStatus nextStatus = GameStatus.Standard;
            bool fwdButtonVis = true;
            bool bckButtonVis = true;

            switch (status)
            {
                case GameStatus.Standard: nextStatus = GameStatus.Standard; bckButtonVis = false; break;
                case GameStatus.RankDemanding: nextStatus = GameStatus.Standard; bckButtonVis = false; break;
                case GameStatus.SuitDemanding: nextStatus = GameStatus.RankDemanding; break;
                case GameStatus.Stops: nextStatus = GameStatus.SuitDemanding; break;
                case GameStatus.Battle: nextStatus = GameStatus.Stops; break;
            }

            return (nextStatus, fwdButtonVis, bckButtonVis);
        }
    }
}
