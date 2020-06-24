using MakaoInterfaces;
using System.Windows;

namespace MakaoGraphicsRepresentation
{
    public class JokerWindowEventArgs : RoutedEventArgs
    {
        public CardRanks CardRank;
        public CardSuits CardSuit;
    }
}
