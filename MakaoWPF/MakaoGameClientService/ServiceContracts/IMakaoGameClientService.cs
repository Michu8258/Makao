using System.ServiceModel;
using MakaoGameClientService.Messages;
using MakaoGameClientService.ServiceImplementations;

namespace MakaoGameClientService.ServiceContracts
{
    [ServiceContract]
    public interface IMakaoGameClientService
    {
        [OperationContract]
        bool CheckIfServiceIsWorking();

        [OperationContract]
        void UpdateTheCurrentListOfPlayers();

        [OperationContract]
        void RoomWasDeleted(DeletionReason request);

        [OperationContract]
        void UpdatePlayersGameReadinessData(ActualizedPlayersReadinessDataRequest request);

        #region Game state contracts

        [OperationContract]
        UpdatingGameStatusResponse StartNewGameWindow(PersonalizedForSpecificPlayerStartGameDataRequest request);

        [OperationContract]
        UpdatingGameStatusResponse UpdateGameStateAndData(PersonalizedPlayerDataRequest request);

        [OperationContract]
        void ShowGameResultsWindow(GameFinishedDataRequest request);

        #endregion
    }
}
