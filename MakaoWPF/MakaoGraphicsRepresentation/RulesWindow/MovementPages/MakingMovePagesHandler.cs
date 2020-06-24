using MakaoGraphicsRepresentation.RulesWindow.PagesEnums;
using System.Windows.Controls;

namespace MakaoGraphicsRepresentation.RulesWindow.MovementPages
{
    public class MakingMovePagesHandler
    {
        public Page GetMakingMoveProperPage(MakingMoveEnum pageType)
        {
            switch (pageType)
            {
                case MakingMoveEnum.PossibilityOfMove: return new PossibilityOfMovePage();
                case MakingMoveEnum.ImpossibilityOfMove: return new ImpossibilityOfMovePage();
                case MakingMoveEnum.MoreThanOneCard: return new MoreThanOneCardPage();
                case MakingMoveEnum.ChoosenCards: return new ChoosenCardsPage();
                case MakingMoveEnum.RankDemanding: return new RankDemandingPage();
                case MakingMoveEnum.SuitDemanding: return new SuitDemandingPage();
                case MakingMoveEnum.FirstCardJoker: return new FirstCardJokerPage();
                case MakingMoveEnum.FirstCardJokerChange: return new FirstCardJokerChangePage();
                case MakingMoveEnum.JokerChange: return new JokerChangePage();
                case MakingMoveEnum.PlayerCardsWithChangedJoker: return new PlayerCardsWithChangedJokerPage();
                case MakingMoveEnum.WaitingInStopsMode: return new WaitingInStopsModePage();
                case MakingMoveEnum.StopsAmountInfo: return new StopsAmountInfoPage();
                case MakingMoveEnum.GameEnded: return new GameEndedPage();
                default: return null;
            }
        }
    }
}
