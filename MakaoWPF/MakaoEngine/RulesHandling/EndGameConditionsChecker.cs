using MakaoInterfaces;
using System.Collections.Generic;

namespace MakaoEngine.RulesHandling
{
    public class EndGameConditionsChecker
    {
        public bool CheckGameEndingConditions(ref Dictionary<int, int> FinishedPlayers,
            Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            AssignPlayersThatFinishedGame(PlayersCurrentData, ref FinishedPlayers);
            return CheckEndGameConditions(ref FinishedPlayers, PlayersCurrentData);
        }

        public void AddLastPlayerToFinishedList(ref Dictionary<int, int> FinishedPlayers,
            Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            foreach (var item in PlayersCurrentData)
            {
                if (!CheckIfPlayerIsInTheList((item.Key), FinishedPlayers))
                {
                    AddPlayerToLstOfFinishedPlayers(item.Key, ref FinishedPlayers);
                }
            }
        }

        private bool CheckEndGameConditions(ref Dictionary<int, int> FinishedPlayers,
            Dictionary<int, SinglePlayerData> PlayersCurrentData)
        {
            return (FinishedPlayers.Count + 1 == PlayersCurrentData.Count);
        }

        private void AssignPlayersThatFinishedGame(Dictionary<int, SinglePlayerData> PlayersCurrentData,
            ref Dictionary<int, int> FinishedPlayers)
        {
            for (int i = 0; i < PlayersCurrentData.Count; i++)
            {
                if (PlayersCurrentData[i].PlayerCards.Count == 0) AddPlayerToLstOfFinishedPlayers(i, ref FinishedPlayers);
            }
        }

        private void AddPlayerToLstOfFinishedPlayers(int playerNumber,
            ref Dictionary<int, int> FinishedPlayers)
        {
            if (!CheckIfPlayerIsInTheList(playerNumber, FinishedPlayers))
            {
                FinishedPlayers.Add(FinishedPlayers.Count + 1, playerNumber);
            }
        }

        private bool CheckIfPlayerIsInTheList(int playerNumber,
            Dictionary<int, int> FinishedPlayers)
        {
            bool output = false;
            foreach (var item in FinishedPlayers)
            {
                if (item.Value == playerNumber)
                {
                    output = true;
                    break;
                }
            }
            return output;
        }
    }
}
