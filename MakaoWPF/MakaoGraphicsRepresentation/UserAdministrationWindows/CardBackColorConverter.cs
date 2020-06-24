using CardGraphicsLibraryHandler;

namespace MakaoGraphicsRepresentation.UserAdministrationWindows
{
    class CardBackColorConverter
    {
        public int ConvertToNumber(BackColor color)
        {
            switch (color)
            {
                case BackColor.Blue: return 0;
                case BackColor.Gray: return 1;
                case BackColor.Green: return 2;
                case BackColor.Purple: return 3;
                case BackColor.Red: return 4;
                case BackColor.Yellow: return 5;
            }
            return 0;
        }

        public BackColor ConvertToEnum(int colorNumber)
        {
            switch (colorNumber)
            {
                case 0: return BackColor.Blue;
                case 1: return BackColor.Gray;
                case 2: return BackColor.Green;
                case 3: return BackColor.Purple;
                case 4: return BackColor.Red;
                case 5: return BackColor.Yellow;
            }
            return BackColor.Blue;
        }
    }
}
