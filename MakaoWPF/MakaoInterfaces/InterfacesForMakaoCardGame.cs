using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MakaoInterfaces
{
    //enumeration for card suits - colors
    public enum CardSuits
    {
        Spade,          //Pik
        Club,           //Trefl
        Heart,          //Kier
        Diamond,        //Karo
        None,
    }

    //enumeration for cards figures - ranks
    public enum CardRanks
    {
        Two, Three, Four, Five, Six, Seven,  Eight,  Nine, Ten,
        Jack, Queen, King, Ace, Joker, None
    }

    //enuertaion for directions
    public enum CardMoveDirections
    {
        Forward, Backward,
    }

    //enum for demanding - jacks and aces
    public enum DemandOptions
    {
        Suits, Ranks, None
    }

    //enumeration of game status
    public enum GameStatus { Standard, Battle, Stops, StopsAndBattle, RankDemanding, SuitDemanding };

    //declaration of a structure that represents a card in Makao game
    [DataContractAttribute]
    public class PlayingCard : IComparable<PlayingCard>
    {
        #region privateFields

        [DataMemberAttribute]
        private CardSuits suit;
        [DataMemberAttribute]
        private CardRanks rank;
        [DataMemberAttribute]
        private readonly int deckNumber;
        [DataMemberAttribute]
        private bool brave;
        [DataMemberAttribute]
        private int battlPpower;
        [DataMemberAttribute]
        private bool stopsMove;
        [DataMemberAttribute]
        private bool canDemand;
        [DataMemberAttribute]
        private DemandOptions demands;
        [DataMemberAttribute]
        private CardMoveDirections nextMove;
        [DataMemberAttribute]
        private bool thisCardWasMadeByJocker;
        [DataMemberAttribute]
        private bool canBeDemanded;

        //remember the card after changing from jocker into some other card;

        #endregion

        #region PublicProperties

        public CardSuits Suit { get { return suit; } }
        public CardRanks Rank { get { return rank; } }
        public int DeckNumber {  get { return deckNumber; } }
        public bool IsBrave { get { return brave; } }
        public int BattlePower { get { return battlPpower; } }
        public bool StopsMove { get { return stopsMove; } }
        public bool CanDemand { get { return canDemand; } }
        public DemandOptions Demands { get { return demands; } }
        public CardMoveDirections NextMove { get { return nextMove; } }
        public bool CreatedByJocker { get { return thisCardWasMadeByJocker; } }
        public bool CanBeDemanded { get { return canBeDemanded; } }

        #endregion

        //constructor
        public PlayingCard(CardSuits suitInput, CardRanks rankInput, int deckNumber)
        {
            suit = suitInput;
            rank = rankInput;
            this.deckNumber = deckNumber;
            brave = false;
            battlPpower = 0;
            stopsMove = false;
            canDemand = false;
            demands = DemandOptions.None;
            nextMove = CardMoveDirections.Forward;
            canBeDemanded = false;
            thisCardWasMadeByJocker = false;
            CheckIfCardIsBrave(rank, suit);
            SetTheCardBattlePower(this.rank, this.suit);
            CheckIfCardCanStopMove(this.rank);
            CheckIfCardCanDemandAnotherCards(this.rank);
            CheckIfThisCardCanBeDemanded(this.rank);
            EstablishNextMoveDirection(this.rank, this.suit);
        }

        #region ChangingTeCardFromJockerAndBackwards

        //change the card from jocker into another card
        public void ChangeCardFromJocker(CardSuits suitInput, CardRanks rankInput)
        {
            if (this.Rank != CardRanks.Joker)
            {
                throw new ArgumentOutOfRangeException("The card that was tried to change is not a joker");
            }
            else if (suitInput == CardSuits.None || rankInput == CardRanks.Joker)
            {
                throw new ArgumentException("The card that Jocker changes to cannot be Jocker or have no suit");
            }
            else
            {
                this.suit = suitInput;
                this.rank = rankInput;

                CheckIfCardIsBrave(rankInput, suitInput);
                SetTheCardBattlePower(rankInput, suitInput);
                CheckIfCardCanStopMove(rankInput);
                CheckIfCardCanDemandAnotherCards(rankInput);
                CheckIfThisCardCanBeDemanded(rankInput);

                thisCardWasMadeByJocker = true;
            }
        }

        //change the card into jocker back
        public void ChangeCardBackToJocker()
        {
            if (this.thisCardWasMadeByJocker != true)
            {
                throw new ArgumentException("This card was not a Jocker before");
            }
            else
            {
                this.suit = CardSuits.None;
                this.rank = CardRanks.Joker;
                this.brave = false;
                this.battlPpower = 0;
                this.stopsMove = false;
                this.canDemand = false;
                this.demands = DemandOptions.None;
                this.nextMove = CardMoveDirections.Forward;
                this.thisCardWasMadeByJocker = false;
                this.canBeDemanded = false;
            }
        }

        #endregion

        #region InternalMethods

        //method for checking if the card is Brave
        private void CheckIfCardIsBrave(CardRanks cardRank, CardSuits cardSuit)
        {
            if (cardRank == CardRanks.Two || cardRank == CardRanks.Three
                || (cardRank == CardRanks.King && (cardSuit == CardSuits.Spade || cardSuit == CardSuits.Heart)))
            {
                brave = true;
            }
        }
        
        //method that defines battle power of the card
        private void SetTheCardBattlePower(CardRanks cardRank, CardSuits cardSuit)
        {
            switch (cardRank)
            {
                case CardRanks.Two: battlPpower = 2; break;
                case CardRanks.Three: battlPpower = 3; break;
                case CardRanks.Four: battlPpower = 0; break;
                case CardRanks.Five: battlPpower = 0; break;
                case CardRanks.Six: battlPpower = 0; break;
                case CardRanks.Seven: battlPpower = 0; break;
                case CardRanks.Eight: battlPpower = 0; break;
                case CardRanks.Nine: battlPpower = 0; break;
                case CardRanks.Ten: battlPpower = 0; break;
                case CardRanks.Jack: battlPpower = 0; break;
                case CardRanks.Queen: battlPpower = 0; break;
                case CardRanks.King:
                    if (cardSuit == CardSuits.Spade || cardSuit == CardSuits.Heart)
                    {
                        battlPpower = 5;
                    }
                    else
                    {
                        battlPpower = 0;
                    }
                    break;
                case CardRanks.Ace: battlPpower = 0; break;
                case CardRanks.Joker: battlPpower = 0; break;
            }
        }        

        //method for checkin if card can stop move = if it is four
        private void CheckIfCardCanStopMove(CardRanks cardRank)
        {
            if (cardRank == CardRanks.Four) stopsMove = true;
        }

        //method for checking if car can demand AnotherCards
        private void CheckIfCardCanDemandAnotherCards(CardRanks cardRank)
        {
            if (cardRank == CardRanks.Jack)
            {
                canDemand = true;
                demands = DemandOptions.Ranks;
            }

            if (cardRank == CardRanks.Ace)
            {
                canDemand = true;
                demands = DemandOptions.Suits;
            }
        }

        //method for checking next move direction
        private void EstablishNextMoveDirection(CardRanks cardRank, CardSuits cardSuit)
        {
            if (cardRank == CardRanks.King && cardSuit == CardSuits.Spade)
            {
                nextMove = CardMoveDirections.Backward;
            }
            else
            {
                nextMove = CardMoveDirections.Forward;
            }
        }

        //method for checking if the card can be demanded
        private void CheckIfThisCardCanBeDemanded(CardRanks cardRank)
        {
            if (cardRank == CardRanks.Five || cardRank == CardRanks.Six || cardRank == CardRanks.Seven || cardRank == CardRanks.Eight ||
                cardRank == CardRanks.Nine || cardRank == CardRanks.Ten || cardRank == CardRanks.Queen)
            {
                canBeDemanded = true;
            }
        }

        #endregion

        #region Public methods

        //method that returns string representation of the card
        public override string ToString()
        {
            return rank.ToString() + " of " + suit.ToString() + " from deck " + deckNumber.ToString();
        }

        //static method for determining if passed rank can be demanded
        public static bool CanRankBeDemanded(CardRanks rank)
        {
            if (rank == CardRanks.Five || rank == CardRanks.Six || rank == CardRanks.Seven || rank == CardRanks.Eight ||
                rank == CardRanks.Nine || rank == CardRanks.Ten || rank == CardRanks.Queen)
                return true;
            else return false;
        }

        //method for checking if another Playing card is equal to this
        public int CompareTo(PlayingCard otherCard)
        {
            int isEqual = 1;
            if (this.Rank == otherCard.Rank && this.Suit == otherCard.Suit && this.DeckNumber == otherCard.DeckNumber &&
                this.CreatedByJocker == otherCard.CreatedByJocker)
            {
                isEqual = 0;
            }
            return isEqual;
        }

        #endregion
    }

    public interface IMakaoCardGame
    {
        int AmountOfJockerInDeck { get; }
        int AmountOfDecksInGame { get; }
        int AmountOfPlayers { get; }
        int CurrentPlayer { get; }
        int AmountOfCardsToTake { get; }
        CardSuits DemandedSuit { get; }
        CardRanks DemandedRank { get; }
        Dictionary<int, List<PlayingCard>> PlayersCards {get; }
        List<PlayingCard> Deck { get; }
        List<PlayingCard> UsedCards { get; }
        Dictionary<int, SinglePlayerData> PlayersData { get; }
        GameStatus Status { get; }
        

        void CreateGame();
        bool CanTheCardBePlacedOnTheTable(PlayingCard card);
        bool ChangeJockerIntoAnotherCard(int playerNumber, PlayingCard card, CardRanks newRank, CardSuits newSuit, bool calledByEngine);
        bool ChangeCardsIntoJockersBack(int playerNumber, PlayingCard card);
        List<FamiliarCardsData> FindMatchingCardsInPlayerHand(int playerNumber, PlayingCard card);
        bool PutCardsOnTheTable(bool TakeCardsOrPutCards, List<PlayingCard> CardsList, int playerNumber, bool isDemanding,
            CardRanks demandedRank, CardSuits demandedSuit, bool skipTheMove);
    }
}
