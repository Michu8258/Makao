using CardsRepresentation;

namespace MakaoGraphicsRepresentation.UserAdministrationWindows
{
    class ThirdPlayerLocationConverter
    {
        public int ConvertToNumber(ThirdPlayerLocation location)
        {
            switch (location)
            {
                case ThirdPlayerLocation.Left: return 0;
                case ThirdPlayerLocation.Right: return 1;
            }
            return 0;
        }

        public ThirdPlayerLocation ConvertToEnum(int location)
        {
            switch (location)
            {
                case 0: return ThirdPlayerLocation.Left;
                case 1: return ThirdPlayerLocation.Right;
            }
            return ThirdPlayerLocation.Left;
        }
    }
}
