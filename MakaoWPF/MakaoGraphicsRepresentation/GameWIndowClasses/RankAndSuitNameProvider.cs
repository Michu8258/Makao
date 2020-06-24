using MakaoInterfaces;
using System;
using System.Collections.Generic;

namespace MakaoGraphicsRepresentation.GameWIndowClasses
{
    static class RankAndSuitNameProvider
    {
        private static Dictionary<CardRanks, Dictionary<string, string>> RanksNames;
        private static Dictionary<CardSuits, Dictionary<string, string>> SuitNames;

        static RankAndSuitNameProvider()
        {
            PopulateSuitNamesDictionary();
            PopulateRankNamesDictionary();
        }

        private static void PopulateSuitNamesDictionary()
        {
            SuitNames = new Dictionary<CardSuits, Dictionary<string, string>>();
            Dictionary<string, string> None = new Dictionary<string, string>
            {
                { "pl", "Brak" },
                { "en", "None" }
            };
            SuitNames.Add(CardSuits.None, None);
            Dictionary<string, string> Club = new Dictionary<string, string>
            {
                { "pl", "Trefl" },
                { "en", "Clubs" }
            };
            SuitNames.Add(CardSuits.Club, Club);
            Dictionary<string, string> Diamond = new Dictionary<string, string>
            {
                { "pl", "Karo" },
                { "en", "Diamonds" }
            };
            SuitNames.Add(CardSuits.Diamond, Diamond);
            Dictionary<string, string> Heart = new Dictionary<string, string>
            {
                { "pl", "Kier" },
                { "en", "Hearts" }
            };
            SuitNames.Add(CardSuits.Heart, Heart);
            Dictionary<string, string> Spade = new Dictionary<string, string>
            {
                { "pl", "Pik" },
                { "en", "Spades" }
            };
            SuitNames.Add(CardSuits.Spade, Spade);
        }

        private static void PopulateRankNamesDictionary()
        {
            RanksNames = new Dictionary<CardRanks, Dictionary<string, string>>();
            Dictionary<string, string> Two = new Dictionary<string, string>
            {
                { "pl", "Dwójka" },
                { "en", "Two" }
            };
            RanksNames.Add(CardRanks.Two, Two);
            Dictionary<string, string> Three = new Dictionary<string, string>
            {
                { "pl", "Trójka" },
                { "en", "Three" }
            };
            RanksNames.Add(CardRanks.Three, Three);
            Dictionary<string, string> Four = new Dictionary<string, string>
            {
                { "pl", "Czwórka" },
                { "en", "Four" }
            };
            RanksNames.Add(CardRanks.Four, Four);
            Dictionary<string, string> Five = new Dictionary<string, string>
            {
                { "pl", "Piątka" },
                { "en", "Five" }
            };
            RanksNames.Add(CardRanks.Five, Five);
            Dictionary<string, string> Six = new Dictionary<string, string>
            {
                { "pl", "Szóstka" },
                { "en", "Six" }
            };
            RanksNames.Add(CardRanks.Six, Six);
            Dictionary<string, string> Seven = new Dictionary<string, string>
            {
                { "pl", "Siódemka" },
                { "en", "Seven" }
            };
            RanksNames.Add(CardRanks.Seven, Seven);
            Dictionary<string, string> Eight = new Dictionary<string, string>
            {
                { "pl", "Ósemka" },
                { "en", "Eight" }
            };
            RanksNames.Add(CardRanks.Eight, Eight);
            Dictionary<string, string> Nine = new Dictionary<string, string>
            {
                { "pl", "Dziewiątka" },
                { "en", "Nine" }
            };
            RanksNames.Add(CardRanks.Nine, Nine);
            Dictionary<string, string> Ten = new Dictionary<string, string>
            {
                { "pl", "Dziesiątka" },
                { "en", "Ten" }
            };
            RanksNames.Add(CardRanks.Ten, Ten);
            Dictionary<string, string> Jack = new Dictionary<string, string>
            {
                { "pl", "Walet" },
                { "en", "Jack" }
            };
            RanksNames.Add(CardRanks.Jack, Jack);
            Dictionary<string, string> Queen = new Dictionary<string, string>
            {
                { "pl", "Dama" },
                { "en", "Queen" }
            };
            RanksNames.Add(CardRanks.Queen, Queen);
            Dictionary<string, string> King = new Dictionary<string, string>
            {
                { "pl", "Król" },
                { "en", "King" }
            };
            RanksNames.Add(CardRanks.King, King);
            Dictionary<string, string> Jocker = new Dictionary<string, string>
            {
                { "pl", "Joker" },
                { "en", "Jocker" }
            };
            RanksNames.Add(CardRanks.Joker, Jocker);
            Dictionary<string, string> Ace = new Dictionary<string, string>
            {
                { "pl", "As" },
                { "en", "Ace" }
            };
            RanksNames.Add(CardRanks.Ace, Ace);
            Dictionary<string, string> None = new Dictionary<string, string>
            {
                { "pl", "Brak" },
                { "en", "None" }
            };
            RanksNames.Add(CardRanks.None, None);
        }

        public static string GetRankName(CardRanks rank, string language)
        {
            CheckLanguage(language);
            return (RanksNames[rank][language]);
        }

        public static string GetSuitName(CardSuits suit, string language)
        {
            CheckLanguage(language);
            return (SuitNames[suit][language]);
        }

        private static void CheckLanguage(string language)
        {
            if (language != "pl" && language != "en") throw new ArgumentException("The only supported languages are Polish: \"pl\" and English: \"en\".");
        }
    }
}
