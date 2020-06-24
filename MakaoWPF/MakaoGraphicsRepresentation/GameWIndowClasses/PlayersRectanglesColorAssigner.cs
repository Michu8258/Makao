using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MakaoGraphicsRepresentation.GameWIndowClasses
{
    public static class PlayersRectanglesColorAssigner
    {
        //method for assigning one player as the current player
        public static void AssignPlayersRectanglesColors(ref Dictionary<int, Rectangle> ActivePlayerRectangles, int currentPlayer)
        {
            foreach (var item in ActivePlayerRectangles)
            {
                if (item.Key == currentPlayer) item.Value.Fill = Brushes.Gold;
                else item.Value.Fill = Brushes.LightSkyBlue;
            }
        }
    }
}
