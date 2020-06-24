using EngineHost.ServiceImplementation;
using MakaoGameClientService.Messages;
using MakaoGameClientService.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace EngineHost.GameStateUpdatesSending
{
    class GameStateDataSender
    {
        #region Fields and properties

        #endregion

        #region Constructor

        //default constructor
        public GameStateDataSender()
        {
            //empty here :)
        }

        #endregion

        #region Method for choosing type of data to send

        public List<ReturnData> SendDataAboutCreationOfNewRoom(List<object> requests, List<string> IDlist)
        {
            List<ReturnData> output = SendDataToAllPlayers(DataSenderType.EngineInstanceCreatedData, requests, IDlist);
            return output;
        }

        public List<ReturnData> SendUpdatedDataToPlayers(List<object> requests, List<string> IDlist)
        {
            return SendDataToAllPlayers(DataSenderType.EngineInstanceUpdateData, requests, IDlist);
        }

        public List<ReturnData> SendGameFinishedDataToPlayers(GameFinishedDataRequest request)
        {
            List<object> requests = new List<object>();
            foreach (var item in request.GamersList)
            {
                requests.Add((object)request);
            }

            List<string> IDlist = new List<string>();
            foreach (var item in request.GamersList)
            {
                IDlist.Add(item.PlayerID);
            }

            return SendDataToAllPlayers(DataSenderType.GameFinished, requests, IDlist);
        }

        #endregion

        #region Sending data algorithm

        //method - algorithm
        private List<ReturnData> SendDataToAllPlayers(DataSenderType dataType, List<object> dataRequests, List<string> IDlist)
        {
            List<ReturnData> output = new List<ReturnData>();

            //iterate through all plauers in Placeholders static class
            foreach (string item in IDlist)
            {
                try
                {
                    //gain player endpoint - based on ID
                    Uri endpoint = (DataPlaceholders.MakaoEngineHostDataPlaceholders.PlayersData.Single(x => x.PlayerID == item)).PlayerEndpoint;
                    int index = IDlist.FindIndex(x => x == item);

                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info($"Sending data of type: {dataType.ToString()}, to player with endpoint equal to: {endpoint.ToString()}.");

                    //create channel factory and call method in client contract
                    ReturnData receivedData = ChannelFactoryCreation(endpoint, dataType, dataRequests[index]);
                    output.Add(receivedData);
                }
                catch (Exception ex)
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Error($"Sending data of type: {dataType.ToString()}, failed: {ex.Message}.");
                }
            }

            return output;
        }

        //method for creating channel factory
        private ReturnData ChannelFactoryCreation(Uri endpoint, DataSenderType dataType, object dataToSend)
        {
            ReturnData output = null;

            try
            {
                //creata channel factory based on tje uri
                ChannelFactory<IMakaoGameClientService> factory = new ChannelFactory<IMakaoGameClientService>
                    (new BasicHttpBinding(), new EndpointAddress(endpoint));
                //now create proxy
                IMakaoGameClientService proxy = factory.CreateChannel();
                //and call some method
                output = SendDataToClient(dataType, proxy, dataToSend);
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Couldn't connect to the client endpoint game state data. DataType: {dataType.ToString()}. Exception: {ex.Message}.");
            }

            return output;
        }

        //method that call a proper method from client contract
        private ReturnData SendDataToClient(DataSenderType dataType, IMakaoGameClientService proxy, object dataToTransfer)
        {
            ReturnData output = null;

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"Sending info to specific player in the room: {dataType.ToString()}.");

            try
            {
                switch (dataType)
                {
                    case DataSenderType.EngineInstanceCreatedData:
                        output = EngineInstanceCreatedDataHandler(proxy, dataToTransfer);
                        break;
                    case DataSenderType.EngineInstanceUpdateData:
                        output = EngineDataUpdateHandler(proxy, dataToTransfer);
                        break;
                    case DataSenderType.GameFinished:
                        output = EndGameHandler(proxy, dataToTransfer);
                        break;
                }
            }
            catch (Exception ex)
            {
                var logger2 = NLog.LogManager.GetCurrentClassLogger();
                logger2.Error($"Sending info to specific player in the room: {dataType.ToString()} failed: {ex.Message}");
            }

            return output;
        }

        #endregion

        #region Client contracts responses handler methods

        //start new game
        private ReturnData EngineInstanceCreatedDataHandler(IMakaoGameClientService proxy, object dataToTransfer)
        {
            UpdatingGameStatusResponse response = null;
            ReturnData output = null;

            try
            {
                response = proxy.StartNewGameWindow((PersonalizedForSpecificPlayerStartGameDataRequest)dataToTransfer);
            }
            catch (Exception ex)
            {
                CatchConnectionException(ex, (dataToTransfer as PersonalizedForSpecificPlayerStartGameDataRequest).DataOfThisPlayer.ThisPlayerNumber);
            }

            if (response != null) output = GenerateReturnData(response);
            return output;
        }

        //update the game
        private ReturnData EngineDataUpdateHandler(IMakaoGameClientService proxy, object dataToTransfer)
        {
            UpdatingGameStatusResponse response = null;
            ReturnData output = null;

            try
            {
                response = proxy.UpdateGameStateAndData((PersonalizedPlayerDataRequest)dataToTransfer);
            }
            catch (Exception ex)
            {
                CatchConnectionException(ex, (dataToTransfer as PersonalizedPlayerDataRequest).DataOfThisPlayer.ThisPlayerNumber);
            }

            if (response != null) output = GenerateReturnData(response);
            return output;
        }

        //send info about game end
        private ReturnData EndGameHandler(IMakaoGameClientService proxy, object dataToTransfer)
        {
            try
            {
                proxy.ShowGameResultsWindow((GameFinishedDataRequest)dataToTransfer);
            }
            catch (Exception ex)
            {
                CatchConnectionException(ex, (dataToTransfer as GameFinishedDataRequest).WinnerPlayerNumber);
            }

            return new ReturnData() { Response = true, PlayerID = "", PlayerNumber = 0 };
        }

        //internal method for catching exception when failed to send data to client
        private void CatchConnectionException(Exception ex, int playerNumber)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Error($"Couldn't obtain response from client with player number {playerNumber.ToString()}: {ex.Message}.");

            //Send info to clients about ending the game, because of no some
            //player did not answer
            InfoSenderClass Sender = new InfoSenderClass();
            Sender.DeleteTheRoom(ClientInfoType.LostConnectionToClient);
        }

        //method for creating the return data object based on response from client
        private ReturnData GenerateReturnData(UpdatingGameStatusResponse response)
        {
            ReturnData output = new ReturnData()
            {
                PlayerID = response.PlayerID,
                PlayerNumber = response.PlayerNumber,
                Response = response.Done,
            };
            return output;
        }

        #endregion
    }
}
