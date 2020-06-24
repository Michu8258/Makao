using MakaoInterfaces;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MakaoEngine.RulesHandling
{
    public class CardsDemandingHandler
    {
        #region Public methods

        //control demanding options
        public bool CheckDemandPossibilities(CardRanks newDemRank, CardSuits newDemSuit, int playerNumber,
            ref CardRanks demandedRank, ref CardSuits demandedSuit, ref GameStatus status,
            ref Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            string demandingType = "";
            if (Engine.ExtendedLogging)
            {
                logger.Info($"+++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                logger.Info($"+++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                logger.Info($"Demand possibilities checker current gme state: {status.ToString()}.");
                logger.Info($"+++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                logger.Info($"+++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            }

            //can't demand rank and suit at the same time
            if (newDemRank != CardRanks.None && newDemSuit != CardSuits.None)
            {
                string text = "Rank and Suit can not be demanded both at the same time";
                logger.Error(text);
                throw new ArgumentException(text);
            }

            //set demanding after placing the card on the table
            if (newDemRank == CardRanks.None && newDemSuit == CardSuits.None)
            {
                demandingType = "none";
                demandedRank = CardRanks.None;
                demandedSuit = CardSuits.None;
                //status
                if (status != GameStatus.Battle && status != GameStatus.Stops &&
                    status != GameStatus.StopsAndBattle) status = GameStatus.Standard;
                ResetDemandingOptionsForAllPlayers(ref PlayersCurrentData, false);
            }
            else if (newDemRank != CardRanks.None && newDemSuit == CardSuits.None)
            {
                demandingType = "rank";
                SetRankDemandOption(playerNumber, newDemRank, ref demandedRank, ref demandedSuit, ref PlayersCurrentData);
                //status
                status = GameStatus.RankDemanding;
            }
            else if (newDemRank == CardRanks.None && newDemSuit != CardSuits.None)
            {
                demandingType = "suit";
                SetSuitDemandOption(playerNumber, newDemSuit, ref demandedRank, ref demandedSuit, ref PlayersCurrentData);
                //status
                status = GameStatus.SuitDemanding;
            }

            if (Engine.ExtendedLogging) logger.Info($"Demand possibilities checking done. Demanded rank: {demandedRank.ToString()}, demanded suit: {demandedSuit.ToString()}, type: {demandingType}.");

            return true;
        }

        //method for stop suit demanding after one card putted with proper suit
        public void ManageSuitEndDemanding(ref GameStatus status, ref CardSuits demandedSuit, PlayingCard firsCardPutedByPlayerOnTheTable,
            ref Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            if (demandedSuit != CardSuits.None && CheckIfAnyPlayerStartedDemandingSuits(PlayersCurrentData))
            {
                if (firsCardPutedByPlayerOnTheTable.Rank != CardRanks.Ace)
                {
                    ResetDemandingOptionsForAllPlayers(ref PlayersCurrentData, true);
                    demandedSuit = CardSuits.None;
                    if (status == GameStatus.SuitDemanding) status = GameStatus.Standard;
                }
            }
        }

        #endregion

        #region Private internal methods

        /// <summary>
        /// Rank = false, Suit = true
        /// </summary>
        /// <param name="PlayersCurrentData"></param>
        /// <param name="RankOrSuit"></param>
        public static void ResetDemandingOptionsForAllPlayers (ref Dictionary<int, SinglePlayerData> PlayersCurrentData, bool RankOrSuit)
        {
            foreach (var item in PlayersCurrentData)
            {
                if (!RankOrSuit) item.Value.ThisPlayerStartedRankDemanding = false;
                else item.Value.ThisPlayerStartedSuitDemanding = false;
            }
        }

        /// <summary>
        /// Rank = false, Suit = true
        /// </summary>
        /// <param name="PlayersCurrentData"></param>
        /// <param name="RankOrSuit"></param>
        /// <returns></returns>
        public static int GetPlayerNumberWhoStartedDemanding (Dictionary<int, SinglePlayerData> PlayersCurrentData, bool RankOrSuit)
        {
            int output = -1000;
            try
            {
                if (!RankOrSuit) output = PlayersCurrentData.Single(x => x.Value.ThisPlayerStartedRankDemanding == true).Key;
                else output = PlayersCurrentData.Single(x => x.Value.ThisPlayerStartedSuitDemanding == true).Key;
            }
            catch
            {
                if (Engine.ExtendedLogging)
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    if (!RankOrSuit) logger.Info($"No player started RANK demanding.");
                    else logger.Info($"No player started SUIT demanding.");
                }
            }
            return output;
        }


        private bool CheckIfAnyPlayerStartedDemandingSuits(Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            bool output = false;
            foreach (var item in PlayersCurrentData)
            {
                if (item.Value.ThisPlayerStartedSuitDemanding == true)
                {
                    output = true;
                    break;
                }
            }
            return output;
        }

        //method for establishing demanded rank
        private void SetRankDemandOption(int playerNumber, CardRanks demRank, ref CardRanks demandedRank,
            ref CardSuits demandedSuit, ref Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            bool demandOK = PlayingCard.CanRankBeDemanded(demRank);
            if (!demandOK)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "The rank: " + demRank.ToString() + " can not be demanded";
                logger.Error(text);
            }
            else
            {
                ResetDemandingOptionsForAllPlayers(ref PlayersCurrentData, false);
                demandedRank = demRank;
                demandedSuit = CardSuits.None;
                PlayersCurrentData[playerNumber].ThisPlayerStartedRankDemanding = true;
            }
        }

        //method for establishing demanded suit
        private void SetSuitDemandOption(int playerNumber, CardSuits demSuit, ref CardRanks demandedRank,
            ref CardSuits demandedSuit, ref Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            ResetDemandingOptionsForAllPlayers(ref PlayersCurrentData, true);
            demandedRank = CardRanks.None;
            demandedSuit = demSuit;
            PlayersCurrentData[playerNumber].ThisPlayerStartedSuitDemanding = true;
        }

        #endregion
    }
}
