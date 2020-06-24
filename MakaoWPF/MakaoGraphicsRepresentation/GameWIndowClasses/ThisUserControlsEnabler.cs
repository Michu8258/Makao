using CardsRepresentation;

namespace MakaoGraphicsRepresentation.GameWIndowClasses
{
    public static class ThisUserControlsEnabler
    {
        //method for enabling controls of current player
        public static void EnableOrDisableThisUserControls(ref MainUser ThisPlayerControl, ref DeckRepresentation DeckRepresentationControl, int currentPlayerNumber)
        {
            if (ThisPlayerControl.PlayerNumber == currentPlayerNumber)
            {
                ThisPlayerControl.IsEnabled = true;
                DeckRepresentationControl.IsEnabled = true;
            }
            else
            {
                ThisPlayerControl.IsEnabled = false;
                DeckRepresentationControl.IsEnabled = false;
            }
        }
    }
}
