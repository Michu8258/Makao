using EngineHost.DataPlaceholders;
using MakaoGameClientService.DataTransferObjects;
using MakaoGameClientService.Messages;
using MakaoGameHostService.DataTransferObjects;
using MakaoGameHostService.Messages;
using MakaoGameHostService.ServiceContracts;
using MakaoInterfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace EngineHost.ServiceImplementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class EngineHostServiceImplementation : IMakaoGameHostService
    {
        #region Constructor

        //synchronization context data
        private readonly SynchronizationContext synchCont;

        public EngineHostServiceImplementation()
        {
            synchCont = SynchronizationContext.Current;
        }

        #endregion

        #region Defining Game Setup Data

        //method for assigning setup data to Host (including password to room)
        bool IMakaoGameHostService.AssignGameSetupData(AssignGameSetupDataRequest request)
        {
            return MakaoEngineHostDataPlaceholders.AssignGameData(request);
        }

        #endregion

        #region Gain room details

        GetRoomDetailsWhenJoiningRoomResponse IMakaoGameHostService.GetHostRoomDetails()
        {
            return MakaoEngineHostDataPlaceholders.GetHostRoomDetails();
        }

        #endregion

        #region Assigning new users to Room

        //Adding host player as new player (without info of avatar)
        AddNewPlayerResponse IMakaoGameHostService.CreateNewRoomAndHostPlayer(AddNewPlayerRequest request)
        {
            InfoSenderClass Sender = new InfoSenderClass();
            Sender.DeleteTheRoom(ClientInfoType.ClosedByHost);
            return AddPlayerToTheRoom(request, true);
        }

        //Add new player to the room - not host
        AddNewPlayerResponse IMakaoGameHostService.AddNotHostPlayerToTheRoom(AddNewPlayerRequest request)
        {
            return AddPlayerToTheRoom(request, false);
        }

        //method that does the job - adding player in data placeholder static class
        private AddNewPlayerResponse AddPlayerToTheRoom(AddNewPlayerRequest request, bool hostPlayer)
        {
            //assign data (Property is only readable)
            (bool added, int playerNumber) = MakaoEngineHostDataPlaceholders.AddNewPlayerToTheRoom(request, hostPlayer);
            AddNewPlayerResponse response;

            if (added)
            {
                //find player with specific number
                PlayerData playerData = MakaoEngineHostDataPlaceholders.PlayersData.Single(p => p.PlayerNumber == playerNumber);

                //create response (player number = 0)
                response = new AddNewPlayerResponse()
                {
                    PlayerName = playerData.PlayerName,
                    PlayedGames = playerData.PlayedGames,
                    PlayedAndWonGames = playerData.PlayedAndWonGames,
                    PlayerID = playerData.PlayerID,
                    PlayerNumber = playerData.PlayerNumber,
                    IsHostPlayer = playerData.IsHostPlayer,
                    AddedToGame = added,
                    TotalAMountOfPlayers = MakaoEngineHostDataPlaceholders.TotalAmountOfPLayers,
                };

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Added new player to the room - player " + response.PlayerNumber.ToString());
            }
            else
            {
                //create response (player number = -1)
                response = new AddNewPlayerResponse()
                {
                    PlayerName = "",
                    PlayedGames = 0,
                    PlayerID = "",
                    PlayerNumber = -1,
                    AddedToGame = added,
                };

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Adding new player to the room failed...");
            }

            return response;
        }

        #endregion

        #region Saving and reading Avatars

        //save image of player 0 - ony an avatar (to WS)
        bool IMakaoGameHostService.UploadAvatarImagePlayer(MemoryStream input)
        {
            try
            {
                //deserialize request
                SaveAvatarRequest request = (SaveAvatarRequest)AvatarSerializerDeserializer.DeserializeFromStream(input);
                request.AvatarStream.Position = 0;

                //check if player exists
                if (CheckIfPlayerExists(request.PlayerNumber))
                {
                    //save image in temporary storage place
                    Image img = System.Drawing.Image.FromStream(request.AvatarStream);
                    img.Save(MakaoEngineHostDataPlaceholders.AvatarsLocation + GetProperAvatarName(request.PlayerNumber), ImageFormat.Png);
                    InfoSenderClass Sender = new InfoSenderClass();
                    Sender.StartSendingInfoABoutChangeInCurrentPlayerList();
                    return true;
                }
                else return false;

            }
            catch (Exception ex)
            {
                //log if it turned to be impossible
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Saving image in the Makao Game Host faulted: " + ex.Message);
                return false;
            }
        }

        //methot that provides proper avatar name - to save it
        private string GetProperAvatarName(int playerNumber)
        {
            string avatarName = "";
            if (playerNumber < 10) avatarName += "0";
            avatarName += playerNumber.ToString(); ;
            avatarName += ".png";
            return avatarName;
        }

        //method that checks if avatar for player with number that was passed as
        //parameter can be assigned - if the player with this number exists in the list
        private static bool CheckIfPlayerExists(int playerNumber)
        {
            PlayerData player = MakaoEngineHostDataPlaceholders.PlayersData.Single(p => p.PlayerNumber == playerNumber);
            if (player != null)
            {
                if (player.PlayerNumber == playerNumber) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        //read image of player - only an avatar (from WS)
        Stream IMakaoGameHostService.DownloadAvatarImage(int playerNumber)
        {
            //create new MemoryStream class instance
            MemoryStream memStr = new MemoryStream();
            try
            {
                //construct picture fileName
                string avatarName = "";
                if (playerNumber < 10) avatarName = "0" + playerNumber.ToString();
                else avatarName = playerNumber.ToString();

                //read proper png file
                byte[] bytes = File.ReadAllBytes(MakaoEngineHostDataPlaceholders.AvatarsLocation + avatarName + @".png");
                memStr.Write(bytes, 0, bytes.Length);
                memStr.Position = 0;
            }
            catch (Exception ex)
            {
                memStr = null;
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Reading image in the Makao Game Host faulted: " + ex.Message);
            }

            return memStr;
        }

        #endregion

        #region Players readiness to play

        //with this method player sends info about the fact, that he is ready to play
        bool IMakaoGameHostService.ChangePlayerReadinessToPlay(PlayerIsReadyToPlayGameRequest request)
        {
            //try to assign the readiness status
            bool assigned = MakaoEngineHostDataPlaceholders.ChangeReadinessToPlay(request);
            if (assigned)
            {
                //after changing readiness to play of specific player, send info to clients
                //about changes in current players list
                InfoSenderClass Sender = new InfoSenderClass();
                Sender.SendInfoToClientsAboutChangeOfReadiness(ObtainPlayersReadinessData());

                //and check if all players confirmed readiness to play if yes, start
                //procedure of starting the game
                Task.Run(() => MakaoEngineHostDataPlaceholders.FireUpStartingNewGameAlgorithm(synchCont));

            }
            return assigned;
        }

        //obtaining response which is send to clients - current data of players
        //readiness to play the new game
        private ActualizedPlayersReadinessDataRequest ObtainPlayersReadinessData()
        {
            List<ActualizedDataOfPlayersReadiness> ReadinessList = new List<ActualizedDataOfPlayersReadiness>();
            foreach (PlayerData item in MakaoEngineHostDataPlaceholders.PlayersData)
            {
                ActualizedDataOfPlayersReadiness singlePlayerData = new ActualizedDataOfPlayersReadiness()
                {
                    PlayerName = item.PlayerName,
                    PlayerNumber = item.PlayerNumber,
                    PlayerID = item.PlayerID,
                    IsReadyToPlay = item.ReadyToPlay,
                };
                ReadinessList.Add(singlePlayerData);
            }
            ActualizedPlayersReadinessDataRequest response = new ActualizedPlayersReadinessDataRequest()
            {
                ReadinessDataList = ReadinessList,
            };
            return response;
        }

        #endregion

        #region Sending data about current payers list to client

        //request from client (fired by client)
        CurrentPlayersListDataResponse IMakaoGameHostService.GetCurrentPlayersInTheRoomData()
        {
            List<PlayersInRoomData> responseList = new List<PlayersInRoomData>();
            foreach (PlayerData item in MakaoEngineHostDataPlaceholders.PlayersData)
            {
                PlayersInRoomData onePlayerData = new PlayersInRoomData()
                {
                    PlayerName = item.PlayerName,
                    PlayedGames = item.PlayedGames,
                    PlayedAndWonGames = item.PlayedAndWonGames,
                    Endpoint = item.PlayerEndpoint,
                    IsHost = item.IsHostPlayer,
                    PlayerID = item.PlayerID,
                    PlayerNumber = item.PlayerNumber,
                    ReadyToPlay = item.ReadyToPlay,
                };
                responseList.Add(onePlayerData);
            }

            CurrentPlayersListDataResponse response = new CurrentPlayersListDataResponse
            {
                CurrentPLayersData = responseList,
                RoomIsFull = MakaoEngineHostDataPlaceholders.CheckIfRoomIsFull(),
            };
            return response;
        }

        #endregion

        #region Leaving the room by player

        //request from client
        bool IMakaoGameHostService.DeletePlayerFromRoom(LeaveTheRoomRequest request)
        {
            (bool playerDeleted, bool playerWasHost) = MakaoEngineHostDataPlaceholders.DeletePlayerFromRoom(request);

            if (request.ClosedWindowType == LeavingTheRoomWindowType.MainWindow)
            {
                if (playerDeleted && playerWasHost)
                {
                    InfoSenderClass Sender = new InfoSenderClass();
                    Sender.DeleteTheRoom(ClientInfoType.ClosedByHost);
                }
                else if (playerDeleted)
                {
                    InfoSenderClass Sender = new InfoSenderClass();
                    Sender.StartSendingInfoABoutChangeInCurrentPlayerList();
                }
            }
            else
            {
                InfoSenderClass Sender = new InfoSenderClass();
                Sender.DeleteTheRoom(ClientInfoType.PlayerLeftGame);
            }

            return playerDeleted;
        }

        #endregion

        #region Joker changing handling

        //request from clinent to change joker card into some another card
        ChangeJokerCardResponse IMakaoGameHostService.ChangeJokerIntoANotherCard(ChangeJokerIntoAnotherCardRequest request)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"Engine host received request to change joker into naother card from player {request.PlayerNumber}.");
            bool done = false;
            PlayerData data = DataPlaceholders.MakaoEngineHostDataPlaceholders.PlayersData.Single(x => x.PlayerNumber == request.PlayerNumber);
            if (data.PlayerID == request.PlayerID)
            {
                try
                {
                    done = DataPlaceholders.GameStateHolder.EngineInstance.ChangeJockerIntoAnotherCard(request.PlayerNumber, request.CardToChange, request.NewRank, request.NewSuit, false);
                }
                catch (Exception ex)
                {
                    logger.Error($"Error while changing joker into another card: {ex.Message}.");
                }
            }
            return GenerateJokerChangingResponse(data, done);
        }

        //request from host to change card what used to be joker, to joker back
        ChangeJokerCardResponse IMakaoGameHostService.ChangeCardIntoJokerBack(ChangeJokerBackRequest request)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"Engine host received request to change joker back from player {request.PlayerNumber}.");
            bool done = false;
            PlayerData data = DataPlaceholders.MakaoEngineHostDataPlaceholders.PlayersData.Single(x => x.PlayerNumber == request.PlayerNumber);
            if (data.PlayerID == request.PlayerID)
            {
                try
                {
                    done = DataPlaceholders.GameStateHolder.EngineInstance.ChangeCardsIntoJockersBack(request.PlayerNumber, request.CardToRetrieveJoker);
                }
                catch (Exception ex)
                {
                    logger.Error($"Error while changing joker into another card: {ex.Message}.");
                }
            }
            return GenerateJokerChangingResponse(data, done);
        }

        //joker changing methods response builder method
        private ChangeJokerCardResponse GenerateJokerChangingResponse(PlayerData data, bool done)
        {
            ChangeJokerCardResponse response = new ChangeJokerCardResponse()
            {
                PlayerNumber = data.PlayerNumber,
                PlayerID = data.PlayerID,
                ChangeSuccedeed = done,
                CurrentPlayerCards = DataPlaceholders.GameStateHolder.EngineInstance.PlayersCards[data.PlayerNumber],
            };
            return response;
        }

        #endregion

        #region Player movement handling

        //method for performing movement of player
        /// <summary>
        /// request.TakingCardsOrPutingCards - true is puting cards and false when taking cards
        /// </summary>
        bool IMakaoGameHostService.PerformPlayerMove(MakeAMoveRequest request)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"Engine host received request to make a move from player: {request.PlayerNumber}. Taking or puting cards: {request.TakingCardsOrPutingCards}.");
            bool moveSuccessful = false;

            LogDataFromClient(request);

            try
            {
                //get player number
                int playerNumber = DataPlaceholders.MakaoEngineHostDataPlaceholders.PlayersData.Single(x => x.PlayerID == request.PlayerID).PlayerNumber;
                moveSuccessful = DataPlaceholders.GameStateHolder.EngineInstance.PutCardsOnTheTable(request.TakingCardsOrPutingCards, request.CardsToPutOnTheTable,
                    playerNumber, request.PlayerIsDemanding, request.DemandedRank, request.DemandedSuit, request.SkipTheMove);
            }
            catch (Exception ex)
            {
                logger.Info($"Error while processing make a move request: {ex.Message}.");
            }

            logger.Info($"Was the move successful: {moveSuccessful}.");

            return moveSuccessful;
        }

        //method for logging data received for client = making a move
        private void LogDataFromClient(MakeAMoveRequest request)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"CurrentPlayerNumber: {DataPlaceholders.GameStateHolder.EngineInstance.CurrentPlayer}.");
            logger.Info($"Data received from the client with number: {request.PlayerNumber}:");
            logger.Info($"  Player ID = {request.PlayerID}");
            logger.Info($"  Taking cards or puting them = {request.TakingCardsOrPutingCards}");
            logger.Info($"  Is demanding = {request.PlayerIsDemanding}");
            logger.Info($"  DemandedRank = {request.DemandedRank}");
            logger.Info($"  DemandedSuit = {request.DemandedSuit}");

            logger.Info($"  Cards list (amount =  {request.CardsToPutOnTheTable.Count}):");
            foreach (PlayingCard item in request.CardsToPutOnTheTable)
            {
                logger.Info($"  NewCard = {item.ToString()}");
            }
        }

        #endregion
    }
}
