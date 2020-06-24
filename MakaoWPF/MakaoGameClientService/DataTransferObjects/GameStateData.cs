using MakaoInterfaces;
using System.Runtime.Serialization;

namespace MakaoGraphicsRepresentation.MainWindowData
{
    [DataContractAttribute]
    public class GameStateData
    {
        [DataMemberAttribute]
        public int AmountOfPausingTurns { get; set; }
        [DataMemberAttribute]
        public CardRanks CurrentlyDemandedRank { get; set; }
        [DataMemberAttribute]
        public CardSuits CurrentlyDemandedSuit { get; set; }
        [DataMemberAttribute]
        public int CurrentPlayerNumber { get; set; }
        [DataMemberAttribute]
        public int AmountOfCardsToTakeIfLostBattle { get; set; }
        [DataMemberAttribute]
        public bool BlockPossibilityOfTakingCardsFromDeck { get; set; }
        [DataMemberAttribute]
        public GameStatus CurrentStatusOfTheGame { get; set; }

        public void ReseValues()
        {
            AmountOfPausingTurns = 0;
            CurrentlyDemandedRank = CardRanks.None;
            CurrentlyDemandedSuit = CardSuits.None;
            CurrentPlayerNumber = -10;
            AmountOfCardsToTakeIfLostBattle = 0;
            BlockPossibilityOfTakingCardsFromDeck = false;
        }
    }
}
