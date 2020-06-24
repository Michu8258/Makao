using CardGraphicsLibraryHandler;
using CardsRepresentation;

namespace MakaoGraphicsRepresentation
{
    public static class DefaultUserSettings
    {
        public static SettingsData GetDefaultSettings()
        {
            SettingsData output = new SettingsData()
            {
                UserName = "User",
                TypeOfAvatar = MainWindow.LogLocation.Substring(0, MainWindow.LogLocation.Length - 5) + @"\Avatars\01.png",
                AmountOfPlayers = 2,
                AmountOfDecks = 1,
                AmountOfJokers = 3,
                AmountOfStartCards = 5,
                PlayedGames = 0,
                PlayedAndWonGames = 0,
                ReadinessTimeoutEnabled = false,
                WaitingForPlayersReadinessTimeout = 5,
                JoiningTimeoutEnabled = false,
                WaitingForPlayersJoiningTimeout = 5,
                CardsBackColor = BackColor.Blue,
                LocationOfThirdPlayer = ThirdPlayerLocation.Left,
            };
            return output;
        }
    }
}
