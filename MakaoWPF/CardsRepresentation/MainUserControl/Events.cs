using MakaoInterfaces;
using System.Windows;

namespace CardsRepresentation
{
    public class MainUserEventArgs : RoutedEventArgs
    {
        public PlayingCard PlayingCard;
        public Visibility NotPermitted;
        public Visibility AlreadySelected;
        public bool WasMadeByJoker;
        public int ChildrenNumber;
    }
}
