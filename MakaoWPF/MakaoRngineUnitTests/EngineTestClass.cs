using System;
using MakaoEngine;
using MakaoInterfaces;
using System.Collections.Generic;
using NLog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MakaoRngineUnitTests
{
    [TestClass]
    public class EngineTestClass
    {
        const int amountOfPlayers = 4;
        const int amuntofDecks = 2;
        const int amountOfJockers = 3;
        const int amountOfCards = 5;
        //const string source = @"C:\Users\Michał\source\repos\MakaoWPF\MakaoWPF\Logs\MakaoGameLogs.txt";

        private Engine engine;

        private void CreateEngine()
        {
            engine = new Engine(amountOfPlayers, amuntofDecks, amountOfJockers, true, amountOfCards);
        }

        #region EngineTests

        [TestMethod]
        public void EngineConstructorTest()
        {
            CreateEngine();
            engine.CreateGame();

            engine.CanTheCardBePlacedOnTheTable(new PlayingCard(CardSuits.Heart, CardRanks.King, 1));

            //Assert.AreEqual(ok, true);
        }

        [TestMethod]
        public void ChangeCardFromJockerByEngine()
        {
            CreateEngine();
            engine.CreateGame();

            PlayingCard card = new PlayingCard(CardSuits.None, CardRanks.Joker, 1);

            //engine.ChangeJockerIntoAnotherCard(ref card, CardRanks.King, CardSuits.Spade);

            Assert.AreEqual(CardSuits.Spade, card.Suit);
            Assert.AreEqual(CardRanks.King, card.Rank);
            Assert.AreEqual(true, card.CreatedByJocker);
            Assert.AreEqual(5, card.BattlePower);

            if (card.CreatedByJocker == true)
            {
                //engine.ChangeCardsIntoJockersBack(ref card);
            }

            Assert.AreEqual(CardSuits.None, card.Suit);
            Assert.AreEqual(CardRanks.Joker, card.Rank);
            Assert.AreEqual(false, card.CreatedByJocker);
            Assert.AreEqual(0, card.BattlePower);
        }

        [TestMethod]
        public void CheckFindingMatchingCardsMethod()
        {
            CreateEngine();
            engine.CreateGame();

            List<int> cardNumbers = new List<int>();
            List<bool> canCards = new List<bool>();
            int amount = 0;

            //(cardNumbers, canCards, amount) =  engine.FindMatchingCardsInPlayerHand(0, new PlayingCard(CardSuits.Spade, CardRanks.Seven, 1));

            Assert.AreEqual(4, amount);            
            Assert.AreEqual(cardNumbers[0], 1);            
            Assert.AreEqual(cardNumbers[1], 2);            
            Assert.AreEqual(cardNumbers[2], 3);
            Assert.AreEqual(cardNumbers[3], 4);
            
            Assert.AreEqual(false, canCards[0]);
            Assert.AreEqual(false, canCards[1]);
            Assert.AreEqual(false, canCards[2]);
            Assert.AreEqual(true, canCards[3]);
            
        }

        #endregion

        #region CardStructureTests

        private PlayingCard CreateCard()
        {
            return new PlayingCard(CardSuits.None, CardRanks.Joker, 1);
        }

        [TestMethod]
        public void ChangeCardFromJockerCorrectly()
        {
            PlayingCard card = CreateCard();

            card.ChangeCardFromJocker(CardSuits.Spade, CardRanks.Ace);

            Assert.AreEqual(CardSuits.Spade, card.Suit);
            Assert.AreEqual(CardRanks.Ace, card.Rank);
            Assert.AreEqual(true, card.CreatedByJocker);
            Assert.AreEqual(DemandOptions.Suits, card.Demands);
        }

        [TestMethod]
        public void ChangeCardBackToJocker()
        {
            PlayingCard card = CreateCard();
            card.ChangeCardFromJocker(CardSuits.Spade, CardRanks.Ace);

            card.ChangeCardBackToJocker();
            Assert.AreEqual(CardSuits.None, card.Suit);
            Assert.AreEqual(CardRanks.Joker, card.Rank);
            Assert.AreEqual(false, card.CreatedByJocker);
            Assert.AreEqual(DemandOptions.None, card.Demands);
            Assert.AreEqual(false, card.IsBrave);
            Assert.AreEqual(0, card.BattlePower);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ChangeCardFromJockerUncorrectly()
        {
            PlayingCard card = CreateCard();

            card.ChangeCardFromJocker(CardSuits.Spade, CardRanks.Joker);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ChangeACardThatIsNotJocker()
        {
            PlayingCard card = new PlayingCard(CardSuits.Spade, CardRanks.King, 1);
            card.ChangeCardFromJocker(CardSuits.Spade, CardRanks.Ace);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SwitchBackToJockerACardThatWasnNotAJocker()
        {
            PlayingCard card = new PlayingCard(CardSuits.Spade, CardRanks.King, 1);
            card.ChangeCardBackToJocker();
        }

        #endregion
    }
}
