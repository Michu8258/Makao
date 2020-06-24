using CardGraphicsLibraryHandler;
using CardsRepresentation;
using Realms;

namespace MakaoGraphicsRepresentation.UserAdministrationWindows
{
    class PlayerData : RealmObject
    {
        public int ID { get; set; }
        public string AvatarName { get; set; }
        public int AmountOfPlayers { get; set; }
        public int AmountOfDecks { get; set; }
        public int AmountOfJokers { get; set; }
        public int AmountOfStartCards { get; set; }
        public int PlayedGames { get; set; }

        //0 - blue, 1 - gray, 2 - green, 3 - purple, 4 - red, 5 - yellow
        public int CardBack { get; set; }
        public bool ReadinesTimeoutEnabled { get; set; }
        public int WaitingForReadinessTimeout { get; set; }
        public bool JoiningTimeoutEnabled { get; set; }
        public int WaitingForJoiningTimeout { get; set; }
        public int PlayedAndWonGames { get; set; }

        //0 - left, 1 - right
        public int LocationOfThirdPlayer { get; set; }
    }
}
