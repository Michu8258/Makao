using System.ServiceModel;
using MakaoGameHostService.Messages;
using System.IO;

namespace MakaoGameHostService.ServiceContracts
{
    [ServiceContract()]
    public interface IMakaoGameHostService
    {
        [OperationContract]
        bool AssignGameSetupData(AssignGameSetupDataRequest request);

        [OperationContract]
        AddNewPlayerResponse CreateNewRoomAndHostPlayer(AddNewPlayerRequest request);

        [OperationContract]
        AddNewPlayerResponse AddNotHostPlayerToTheRoom(AddNewPlayerRequest request);

        [OperationContract]
        GetRoomDetailsWhenJoiningRoomResponse GetHostRoomDetails();

        #region Avatars handling

        [OperationContract]
        bool UploadAvatarImagePlayer(MemoryStream input);

        [OperationContract]
        Stream DownloadAvatarImage(int playerNumber);

        #endregion

        [OperationContract]
        CurrentPlayersListDataResponse GetCurrentPlayersInTheRoomData();

        [OperationContract]
        bool ChangePlayerReadinessToPlay(PlayerIsReadyToPlayGameRequest request);

        [OperationContract]
        bool DeletePlayerFromRoom(LeaveTheRoomRequest request);

        #region Game contracts

        [OperationContract]
        ChangeJokerCardResponse ChangeJokerIntoANotherCard(ChangeJokerIntoAnotherCardRequest request);

        [OperationContract]
        ChangeJokerCardResponse ChangeCardIntoJokerBack(ChangeJokerBackRequest request);

        [OperationContract]
        bool PerformPlayerMove(MakeAMoveRequest request);

        #endregion
    }
}
