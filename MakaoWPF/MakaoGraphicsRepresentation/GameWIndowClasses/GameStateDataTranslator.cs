using MakaoInterfaces;

namespace MakaoGraphicsRepresentation.GameWIndowClasses
{
    class GameStateDataTranslator
    {
        public bool ThisPlayerPauses { get; set; }
        public int AmountOfPauseTurns { get; set; }
        public bool CardRankIsDemanded { get; set; }
        public string DemandedRank { get; set; }
        public bool CardSuitIsDemanded { get; set; }
        public string DemandedSuit { get; set; }
        public int CurrentPlayerNumber { get; set; }
        public bool AmountOfCardsToTakeVisibility { get; set; }
        public int AmountOfCardsToTakeLostBattle { get; set; }
        public bool BlockPossibilityOfTakingCardsFromDeck { get; set; }
        public string CurrentStatusOfTheGame { get; set; }
        public bool CanPlayerSkipTheMove { get; set; }
        public bool CardTakenInBattleModeMatches { get; set; }
        public PlayingCard MatchingCardInBattleMode { get; set; }
    }
}
