using MakaoGameClientService.Messages;
using MakaoGameClientService.ServiceContracts;

namespace MakaoGameClientService.ServiceImplementations
{
    public class MakaoGameClientServiceImplementation : IMakaoGameClientService
    {
        //return true if service is running
        bool IMakaoGameClientService.CheckIfServiceIsWorking()
        {
            return true;
        }

        //command from host service - refresh playerslist.
        void IMakaoGameClientService.UpdateTheCurrentListOfPlayers()
        {
            DataPlaceholder.OnRefreshCurrentPlayersList();
        }

        //receiving information from host that The Room was deleted
        void IMakaoGameClientService.RoomWasDeleted(DeletionReason request)
        {
            DataPlaceholder.OnTheRoomWasDeleted(request);
        }

        //receiving information from host about players rediness to play
        void IMakaoGameClientService.UpdatePlayersGameReadinessData(ActualizedPlayersReadinessDataRequest request)
        {
            DataPlaceholder.OnPlayersReadinesChanged(request);
        }

        //method for showing game finished window
        void IMakaoGameClientService.ShowGameResultsWindow(GameFinishedDataRequest request)
        {
            DataPlaceholder.OnGameEnded(request);
        }

        //data received from host - game window opening
        UpdatingGameStatusResponse IMakaoGameClientService.StartNewGameWindow(PersonalizedForSpecificPlayerStartGameDataRequest request)
        {
            DataPlaceholder.OnNewGameStarted(request);
            return GenerateGameUpdatingResponse();
        }

        //data received from host - update of game state
        UpdatingGameStatusResponse IMakaoGameClientService.UpdateGameStateAndData(PersonalizedPlayerDataRequest request)
        {
            DataPlaceholder.OnUpdteTheGame(request);
            return GenerateGameUpdatingResponse();
        }

        private UpdatingGameStatusResponse GenerateGameUpdatingResponse()
        {
            UpdatingGameStatusResponse response = new UpdatingGameStatusResponse()
            {
                Done = true,
                PlayerID = CurrentPlayerDataStorage.PlayerID,
                PlayerNumber = CurrentPlayerDataStorage.PlayerNumber,
            };
            return response;
        }
    }
}
