using MakaoGameHostService.DataTransferObjects;
using MakaoInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace MakaoGameHostService.Messages
{
    //adding new player to the room
    [DataContract]
    public class AddNewPlayerRequest
    {
        [DataMember]
        public string PlayerName { get; set; }

        [DataMember]
        public int PlayedGames { get; set; }

        [DataMember]
        public int PlayedAndWonGames { get; set; }

        [DataMember]
        public Uri PlayerEndpoint { get; set; }

        [DataMember]
        public string Password { get; set; }
    }

    //saving an avatar in the hosts directory  
    [DataContract]
    [Serializable]
    public class SaveAvatarRequest
    {
        [DataMember]
        public Stream AvatarStream { get; set; }

        [DataMember]
        public int PlayerNumber { get; set; }
    }

    //defining game setup data
    [DataContract]
    public class AssignGameSetupDataRequest
    {
        [DataMember]
        public int AmountOfPlayers { get; set; }

        [DataMember]
        public int AmountOfDecks { get; set; }

        [DataMember]
        public int AmountOfJokers { get; set; }

        [DataMember]
        public int AmountOfStartCards { get; set; }

        [DataMember]
        public string RoomPassword { get; set; }

        [DataMember]
        public string HostID { get; set; }

        [DataMember]
        public string HostName { get; set; }

        [DataMember]
        public bool JoiningTimeoutEnabled { get; set; }

        [DataMember]
        public int JoiningTimeoutInMinutes { get; set; }

        [DataMember]
        public bool ReadinessTimeoutEnabled { get; set; }

        [DataMember]
        public int ReadinessTimeoutInMinutes { get; set; }
    }

    //request from host about changing status of readiness to game
    [DataContract]
    public class PlayerIsReadyToPlayGameRequest
    {
        [DataMember]
        public int PlayerNumber { get; set; }

        [DataMember]
        public bool IsReadyToPlay { get; set; }

        [DataMember]
        public string PlayerID { get; set; }
    }

    //request for leaving the room
    [DataContract]
    public class LeaveTheRoomRequest
    {
        [DataMember]
        public string PlayerName { get; set; }

        [DataMember]
        public string PlayerID { get; set; }

        [DataMember]
        public int PlayerNumber { get; set; }

        [DataMember]
        public bool IsHostPlayer { get; set; }

        [DataMember]
        public LeavingTheRoomWindowType ClosedWindowType { get; set; }
    }

    #region Data send in game

    //Data for changing Joker into another card
    [DataContract]
    public class ChangeJokerIntoAnotherCardRequest
    {
        [DataMember]
        public int PlayerNumber { get; set; }

        [DataMember]
        public string PlayerID { get; set; }

        [DataMember]
        public PlayingCard CardToChange { get; set; }

        [DataMember]
        public CardRanks NewRank { get; set; }

        [DataMember]
        public CardSuits NewSuit { get; set; }
    }

    //Data for changing a card back to joker
    [DataContract]
    public class ChangeJokerBackRequest
    {
        [DataMember]
        public int PlayerNumber { get; set; }

        [DataMember]
        public string PlayerID { get; set; }

        [DataMember]
        public PlayingCard CardToRetrieveJoker { get; set; }
    }

    //data rewuest for making a move by player
    [DataContract]
    public class MakeAMoveRequest
    {
        [DataMember]
        public string PlayerID { get; set; }

        [DataMember]
        public int PlayerNumber { get; set; }

        [DataMember]
        public bool TakingCardsOrPutingCards { get; set; }

        [DataMember]
        public List<PlayingCard> CardsToPutOnTheTable { get; set; }

        [DataMember]
        public bool PlayerIsDemanding { get; set; }

        [DataMember]
        public CardRanks DemandedRank { get; set; }

        [DataMember]
        public CardSuits DemandedSuit { get; set; }

        [DataMember]
        public bool SkipTheMove { get; set; }
    }

    #endregion
}
