using System;

namespace EngineHost.DataPlaceholders
{
    //class for storage data of one player that joined the room in game
    public class PlayerData
    {
        public string PlayerName { get; set; }
        public int PlayedGames { get; set; }
        public int PlayedAndWonGames { get; set; }
        public string PlayerID { get; set; }
        public Uri PlayerEndpoint { get; set; }
        public int PlayerNumber { get; set; }
        public bool IsHostPlayer { get; set; }
        public bool ReadyToPlay { get; set; }
    }

    //class for storage of games setup
    public class GameSetup
    {
        public int AmountOfPlayers { get; set; }
        public int AmountOfDecks { get; set; }
        public int AmountOfJokers { get; set; }
        public int AmountOfStartCards { get; set; }
        public string RoomPassword { get; set; }
        public string HostID { get; set; }
        public string HostName { get; set; }
        public bool JoiningTimeoutEnabled { get; set; }
        public int JoiningTimeoutTimeInMinutes { get; set; }
        public bool ReadinessTimeoutEnabled { get; set; }
        public int ReadinessTimeoutInMinutes { get; set; }
    }
}
