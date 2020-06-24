using MakaoInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakaoGameClientService.DataTransferObjects
{
    //data send to clients when change in players readiness to
    //play changed
    public class ActualizedDataOfPlayersReadiness
    {
        public int PlayerNumber { get; set; }
        public string PlayerName { get; set; }
        public string PlayerID { get; set; }
        public bool IsReadyToPlay { get; set; }
    }

    #region Personalized request for user - game creation (data from host)

    public class ThisPlayerData
    {
        public int ThisPlayerNumber { get; set; }
        public string ThisPlayerName { get; set; }
        public string ThisPlayerID { get; set; }
        public List<PlayingCard> ThisPlayerCards { get; set; }
        public bool CanSkipTheMove { get; set; }
        public bool TakenInBattleCardMatching { get; set; }
        public PlayingCard MatchingCard { get; set; }
    }

    public class OtherPlayerData
    {
        public int OtherPlayerNumber { get; set; }
        public string OtherPlayerName { get; set; }
        public string OtherPlayerID { get; set; }
        public int OtherPlayerAmountOfCards { get; set; }
    }

    public class TransferingCard
    {
        public CardRanks RankOfCard { get; set; }
        public CardSuits SuitOfCard { get; set; }
        public int DeckNumber { get; set; }
    }

    public class PlayerPositionDetails
    {
        public int PlayerPosition { get; set; }
        public string PlayerName { get; set; }
        public string PlayerID { get; set; }
        public int PlayedGames { get; set; }
        public int PlayedAndWonGames { get; set; }
        public int PlayerNumber { get; set; }
    }

    #endregion
}
