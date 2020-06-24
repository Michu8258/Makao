using MakaoInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakaoGraphicsRepresentation.GameWIndowClasses
{
    static class GameStatusNameProvider
    {
        private static Dictionary<GameStatus, Dictionary<string, string>> GameStatusNames;

        static GameStatusNameProvider()
        {
            PopulateGameStatusNamesDictionary();
        }

        private static void PopulateGameStatusNamesDictionary()
        {
            GameStatusNames = new Dictionary<GameStatus, Dictionary<string, string>>();
            Dictionary<string, string> Standard = new Dictionary<string, string>
            {
                { "pl", "Standard" },
                { "en", "Standard" }
            };
            GameStatusNames.Add(GameStatus.Standard, Standard);
            Dictionary<string, string> RankDemanding = new Dictionary<string, string>
            {
                { "pl", "Żądanie figur" },
                { "en", "Rank demanding" }
            };
            GameStatusNames.Add(GameStatus.RankDemanding, RankDemanding);
            Dictionary<string, string> SuitDemanding = new Dictionary<string, string>
            {
                { "pl", "Żądanie koloru" },
                { "en", "Suit demanding" }
            };
            GameStatusNames.Add(GameStatus.SuitDemanding, SuitDemanding);
            Dictionary<string, string> Stops = new Dictionary<string, string>
            {
                { "pl", "Czekanie" },
                { "en", "Stops" }
            };
            GameStatusNames.Add(GameStatus.Stops, Stops);
            Dictionary<string, string> Battle = new Dictionary<string, string>
            {
                { "pl", "Bitwa" },
                { "en", "Battle" }
            };
            GameStatusNames.Add(GameStatus.Battle, Battle);
        }

        public static string GetStatusName(GameStatus status, string language)
        {
            CheckLanguage(language);
            return (GameStatusNames[status][language]);
        }

        private static void CheckLanguage(string language)
        {
            if (language != "pl" && language != "en") throw new ArgumentException("The only supported languages are Polish: \"pl\" and English: \"en\".");
        }
    }
}
