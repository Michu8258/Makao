using CardGraphicsLibraryHandler;
using CardsRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakaoGraphicsRepresentation.MainWindowData
{
    public class SavedDataClass
    {
        public int SavedAmountOfPlayers { get; set; }
        public int SavedAmountOfDecks { get; set; }
        public int SavedAmountOfJokers { get; set; }
        public int SavedAmountOfStartCards { get; set; }
        public int SavedAmountOfPlayedGames { get; set; }
        public int SavedAmountOfPlayedAndWonGames { get; set; }
        public bool ReadinessTimeoutEnabled { get; set; }
        public int ReadinessTimeoutMinutesAmount { get; set; }
        public bool JoiningTimeoutEnabled { get; set; }
        public int JoiningTimeoutMinutesAmount { get; set; }
        public BackColor CardsBackColor { get; set; }
        public ThirdPlayerLocation LocationOfThirdPlayersCards { get; set; }
        public string CurrentPlayerName { get; set; }
        public string CurrentAvatarPicture { get; set; }
    }
}
