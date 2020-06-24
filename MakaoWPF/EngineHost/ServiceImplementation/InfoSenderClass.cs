using EngineHost.DataPlaceholders;
using MakaoGameClientService.Messages;
using MakaoGameClientService.ServiceContracts;
using MakaoGameClientService.ServiceImplementations;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace EngineHost.ServiceImplementation
{
    class InfoSenderClass
    {
        #region Fields, properties and coonstructors

        private readonly List<bool> AlivenessList;

        public InfoSenderClass()
        {
            AlivenessList = new List<bool>();
        }

        #endregion

        #region Infos to send

        public bool CheckAllivenessOfCurrentPlayers()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Checking aliveness of players started");
            SendInfoToAllCLients(ClientInfoType.CheckAliveness, null);
            return CheckAlivenessForAllPlayers();
        }

        //spread info about room removal
        public void DeleteTheRoom(ClientInfoType messageType)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Start sending info about removal of room. Amount of players: " +
                MakaoEngineHostDataPlaceholders.PlayersData.Count.ToString());
            SendInfoToAllCLients(messageType, null);
        }
        
        //spread info about change in players readiness to game
        public void SendInfoToClientsAboutChangeOfReadiness(ActualizedPlayersReadinessDataRequest dataToSend)
        {
            Task.Run(() => SendInfoToAllCLients(ClientInfoType.PlayersReadinessChanged, (object)dataToSend));
        }

        //start sending info to client
        public void StartSendingInfoABoutChangeInCurrentPlayerList()
        {
            Task.Run(() => SendInfoToAllCLients(ClientInfoType.PlayersDataChanged, null));
        }

        #endregion

        #region Creating ChannelFactory and calling apropriate method on clients

        //send info to all clients
        private void SendInfoToAllCLients(ClientInfoType messageType, object dataToSend)
        {
            foreach (PlayerData item in MakaoEngineHostDataPlaceholders.PlayersData)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info(string.Format("Sending message of type: {0}, to player: {1} [{2}], endpoint: {3}.",
                    messageType.ToString(), item.PlayerName, item.PlayerNumber.ToString(), item.PlayerEndpoint));

                ChannelFactoryCreation(item.PlayerEndpoint, messageType, dataToSend);
            }
        }

        //create new channel factory
        private void ChannelFactoryCreation(Uri endpoint, ClientInfoType messageType, object dataToSend)
        {
            try
            {
                ChannelFactory<IMakaoGameClientService> factory = new ChannelFactory<IMakaoGameClientService>
                    (new BasicHttpBinding(), new EndpointAddress(endpoint));
                IMakaoGameClientService proxy = factory.CreateChannel();
                SendInfoMessageToClient(messageType, proxy, dataToSend);
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Couldn't connect to the client endpoint while sending info" +
                    " to every current player from list: " + messageType + "; " + ex.Message);
            }
        }

        //method for sending proper info
        private void SendInfoMessageToClient(ClientInfoType messageType, IMakaoGameClientService proxy, object dataToSend)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Sending info to specific player in the room: " + messageType.ToString());

            switch (messageType)
            {
                case ClientInfoType.PlayersDataChanged: proxy.UpdateTheCurrentListOfPlayers(); break;
                case ClientInfoType.ClosedByHost: proxy.RoomWasDeleted(DeletionReason.ClosedByHost); break;
                case ClientInfoType.PlayersReadinessChanged:
                    proxy.UpdatePlayersGameReadinessData((ActualizedPlayersReadinessDataRequest)dataToSend); break;
                case ClientInfoType.JoiningTimeout: proxy.RoomWasDeleted(DeletionReason.JoiningTimeout); break;
                case ClientInfoType.ReadinessTimeout: proxy.RoomWasDeleted(DeletionReason.ReadinessTimeout); break;
                case ClientInfoType.CheckAliveness: CheckAlivenessOfPlayer(proxy); break;
                case ClientInfoType.LostConnectionToClient: proxy.RoomWasDeleted(DeletionReason.LostConnection); break;
                case ClientInfoType.PlayerLeftGame: proxy.RoomWasDeleted(DeletionReason.PlayerLeftGame); break;
            }
        }

        #endregion

        #region Checking aliveness of all players

        //method for checking aliveness of one player
        private void CheckAlivenessOfPlayer(IMakaoGameClientService proxy)
        {
            bool alive = proxy.CheckIfServiceIsWorking();
            AlivenessList.Add(alive);
        }

        //aliveness of all players as one bool value
        private bool CheckAlivenessForAllPlayers()
        {
            bool aliveness = true;
            foreach (bool item in AlivenessList)
            {
                if (item == false)
                {
                    aliveness = false; break;
                }
            }
            return aliveness;
        }

        #endregion
    }
}
