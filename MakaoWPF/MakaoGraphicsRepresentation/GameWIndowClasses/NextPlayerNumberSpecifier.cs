namespace MakaoGraphicsRepresentation.GameWIndowClasses
{
    class NextPlayerNumberSpecifier
    {
        //variables for defining range of numbers
        private readonly int minNumber;
        private readonly int maxNumber;
        private int currentIndex;
        public int CurrentIndex { get { return currentIndex; } }

        //constructor
        public NextPlayerNumberSpecifier(int maxNumber, int startIndex = 0, int minNumber = 0)
        {
            this.minNumber = minNumber;
            this.maxNumber = maxNumber;
            currentIndex = startIndex;
        }

        //method for geting next player number
        public int GetNextPlayerNumber()
        {
            if (currentIndex < maxNumber) currentIndex += 1;
            else currentIndex = minNumber;
            return currentIndex;
        }

        //method for getting previous player number
        public int GetPreviousPlayerNumber()
        {
            if (currentIndex > minNumber) currentIndex -= 1;
            else currentIndex = maxNumber;
            return currentIndex;
        }
    }
}
