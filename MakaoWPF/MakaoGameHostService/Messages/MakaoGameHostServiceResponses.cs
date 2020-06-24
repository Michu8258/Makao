using MakaoGameHostService.DataTransferObjects;
using MakaoInterfaces;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MakaoGameHostService.Messages
{
    //Data used for sreating new player
    [DataContract]
    public class AddNewPlayerResponse
    {
        [DataMember]
        public string PlayerName { get; set; }

        [DataMember]
        public int PlayedGames { get; set; }

        [DataMember]
        public int PlayedAndWonGames { get; set; }

        [DataMember]
        public string PlayerID { get; set; }

        [DataMember]
        public int PlayerNumber { get; set; }

        [DataMember]
        public bool IsHostPlayer { get; set; }

        [DataMember]
        public bool AddedToGame { get; set; }

        [DataMember]
        public int TotalAMountOfPlayers { get; set; }
    }

    //Data for obtaining and updating data about current
    //list of players in the room
    [DataContract]
    public class CurrentPlayersListDataResponse
    {
        [DataMember]
        public List<PlayersInRoomData> CurrentPLayersData { get; set; }

        [DataMember]
        public bool RoomIsFull { get; set; }
    }

    //Data used for gaining host details when searching for active hosts
    //when user wants to join some existing room
    [DataContract]
    public class GetRoomDetailsWhenJoiningRoomResponse
    {
        [DataMember]
        public string HostName { get; set; }

        [DataMember]
        public int AmountOfPlayersInRoom { get; set; }

        [DataMember]
        public int RoomCapacity { get; set; }
    }

    //Data for changing joker into another card
    [DataContract]
    public class ChangeJokerCardResponse
    {
        [DataMember]
        public int PlayerNumber { get; set; }

        [DataMember]
        public string PlayerID { get; set; }

        [DataMember]
        public bool ChangeSuccedeed { get; set; }

        [DataMember]
        public List<PlayingCard> CurrentPlayerCards { get; set; }
    }
}
