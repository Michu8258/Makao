using MakaoInterfaces;
using System;

namespace MakaoGraphicsRepresentation
{
    public static class StringCardNameGenerator
    {
        public static string GenerateCardPictureName(PlayingCard card)
        {
            if (card.Rank == CardRanks.Joker)
            {
                return @"CardGraphics/Cards/black_joker.png";
            }
            else
            {
                try
                {
                    string result = @"CardGraphics/Cards/";
                    result += GetRankPNGString(card);
                    result += GetSuitPNGString(card);
                    result += ".png";
                    return result;
                }
                catch
                {
                    return @"CardGraphics/Backs/blue_back.png";
                }
            }
        }

        private static string GetRankPNGString(PlayingCard card)
        {
            string result;
            switch (card.Rank)
            {
                case CardRanks.Two: result = "2_of_"; break;
                case CardRanks.Three: result = "3_of_"; break;
                case CardRanks.Four: result = "4_of_"; break;
                case CardRanks.Five: result = "5_of_"; break;
                case CardRanks.Six: result = "6_of_"; break;
                case CardRanks.Seven: result = "7_of_"; break;
                case CardRanks.Eight: result = "8_of_"; break;
                case CardRanks.Nine: result = "9_of_"; break;
                case CardRanks.Ten: result = "10_of_"; break;
                case CardRanks.Jack: result = "jack_of_"; break;
                case CardRanks.Queen: result = "queen_of_"; break;
                case CardRanks.King: result = "king_of_"; break;
                case CardRanks.Ace: result = "ace_of_"; break;
                default:
                    {
                        var logger = NLog.LogManager.GetCurrentClassLogger();
                        logger.Error("PNG string determination error (Joker or None rank passed to method) - GetRankPNGString");
                        throw new ArgumentException("Not permitted Card Rank");
                    }

            }
            return result;
        }

        private static string GetSuitPNGString(PlayingCard card)
        {
            string result;
            switch (card.Suit)
            {
                case CardSuits.Spade: result = "spades"; break;
                case CardSuits.Club: result = "clubs"; break;
                case CardSuits.Heart: result = "hearts"; break;
                case CardSuits.Diamond: result = "diamonds"; break;
                default:
                    {
                        var logger = NLog.LogManager.GetCurrentClassLogger();
                        logger.Error("PNG string determination error (None Suit passed to method) - GetSuitPNGString");
                        throw new ArgumentException("Not permitted Card Suit");
                    }
            }

            return result;
        }
    }


}
