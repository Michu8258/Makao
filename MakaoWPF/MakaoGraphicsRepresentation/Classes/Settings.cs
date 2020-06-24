using CardGraphicsLibraryHandler;
using CardsRepresentation;

namespace MakaoGraphicsRepresentation
{
    //class for holding all data in one object
    public class SettingsData
    {
        public string UserName { get; set; }
        public string TypeOfAvatar { get; set; }
        public int AmountOfPlayers { get; set; }
        public int AmountOfDecks { get; set; }
        public int AmountOfJokers { get; set; }
        public int AmountOfStartCards { get; set; }
        public int PlayedGames { get; set; }
        public int PlayedAndWonGames { get; set; }
        public bool ReadinessTimeoutEnabled { get; set; }
        public int WaitingForPlayersReadinessTimeout { get; set; }
        public bool JoiningTimeoutEnabled { get; set; }
        public int WaitingForPlayersJoiningTimeout { get; set; }
        public BackColor CardsBackColor { get; set; }
        public ThirdPlayerLocation LocationOfThirdPlayer { get; set; }
    }

}
