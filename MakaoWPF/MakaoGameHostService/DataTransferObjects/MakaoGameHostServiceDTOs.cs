using System;

namespace MakaoGameHostService.DataTransferObjects
{
    //class for passing current players data
    //(players currently in the room)
    public class PlayersInRoomData
    {
        public string PlayerName { get; set; }
        public int PlayedGames { get; set; }
        public int PlayedAndWonGames { get; set; }
        public bool IsHost { get; set; }
        public Uri Endpoint { get; set; }
        public string PlayerID { get; set; }
        public int PlayerNumber { get; set; }
        public bool ReadyToPlay { get; set; }
    }

    //enum for choice of window that has een closed
    //amd that causes ending the game for all players
    public enum LeavingTheRoomWindowType
    {
        MainWindow,
        GameWindow,
    }
}
