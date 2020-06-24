using MakaoGameClientService.DataTransferObjects;
using MakaoGraphicsRepresentation.MainWindowData;
using MakaoInterfaces;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace MakaoGameClientService.Messages
{
    //Data for receiving from host actualized data of
    //players readiness to play the game
    [DataContract]
    public class ActualizedPlayersReadinessDataRequest
    {
        [DataMember]
        public List<ActualizedDataOfPlayersReadiness> ReadinessDataList { get; set; }
    }

    //Data for showing window with game results
    //a table with players Psition
    [DataContract]
    public class GameFinishedDataRequest
    {
        [DataMember]
        public List<PlayerPositionDetails> GamersList { get; set; }

        [DataMember]
        public int WinnerPlayerNumber { get; set; }
        
        [DataMember]
        public TimeSpan GameTimerTimeSpan { get; set; }

        [DataMember]
        public long GameTimerTimeSpanMiliseconds { get; set; }
    }

    //sending game data to clients base class
    [DataContract]
    public class PersonalizedPlayerDataRequest
    {
        [DataMember]
        public string PlayerID { get; set; }

        [DataMember]
        public int CurrentPlayerNumber { get; set; }

        [DataMember]
        public int AmountOfCardsInDeck { get; set; }

        [DataMember]
        public List<PlayingCard> NewCardsOnTheTableList { get; set; }

        [DataMember]
        public GameStateData CurrentGameStatusData { get; set; }

        [DataMember]
        public ThisPlayerData DataOfThisPlayer { get; set; }

        [DataMember]
        public List<OtherPlayerData> DataOfOtherPlayers { get; set; }

        [DataMember]
        public bool CanSkipTheMove { get; set; }
    }

    //Data for sending game status at the start of the game
    [DataContract]
    public class PersonalizedForSpecificPlayerStartGameDataRequest : PersonalizedPlayerDataRequest
    {
        [DataMember]
        public int MinimumPlayerNumber { get; set; }

        [DataMember]
        public int MaximumPlayerNumber { get; set; }

        [DataMember]
        public int AmountOfPlayers { get; set; }
    }
}
