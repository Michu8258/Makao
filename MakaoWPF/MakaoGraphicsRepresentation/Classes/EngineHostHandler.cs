using MakaoGameHostService.Messages;
using MakaoGameHostService.ServiceContracts;
using MakaoGameClientService.ServiceImplementations;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.ServiceModel;
using System.ServiceProcess;

namespace MakaoGraphicsRepresentation
{
    public static class EngineHostHandler
    {
        #region Private fields

        private static int internalLocalTimeout;
        private static int internalNotLocalTimeout;
        private static string internalServiceName;
        private static ChannelFactory<IMakaoGameHostService> factory;
        private static string password;
        private static bool constructed;

        #endregion

        #region Public roperties

        public static int Timeout { get { return internalNotLocalTimeout; } }
        public static int LocalTimeout { get { return internalLocalTimeout; } }
        public static string ServiceName { get { return internalServiceName; } }

        #endregion

        #region Constructing and destroying the static class

        //constructing method
        public static void EngineHostHandlerConstructor(int localTimeout, int notLocalTimeout, string serviceName, string inputPassword, bool startAsHost)
        {
            password = inputPassword;
            internalLocalTimeout = localTimeout;
            internalNotLocalTimeout = notLocalTimeout;
            internalServiceName = serviceName;
            //assign Host endpoint to property in MainWindow
            if (startAsHost) MainWindow.AssignHostEndpoint(MakaoGameHostServiceEndpointObtainer.GetHostEndpointWhileBeeingHost());
            //create new channel factory
            factory = new ChannelFactory<IMakaoGameHostService>(new BasicHttpBinding(),
                new EndpointAddress(MainWindow.MakaoGameHostEndpoint));

            //set flag that informs of the fact that this methos was fired
            constructed = true;

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("EngineHostHandler constructed.");
        }

        //disposing method
        public static void Dispose()
        {
            if(constructed)
            {
                StopService(internalServiceName);
                internalLocalTimeout = 0;
                internalNotLocalTimeout = 0;
                internalServiceName = null;
                factory = null;
                password = null;
            }
            constructed = false;

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("EngineHostHandler disposed.");
        }

        #endregion

        #region Start and stop the service

        //start new service
        public static bool StartNewService()
        {
            //check if service is already running
            ServiceController sc = new ServiceController(internalServiceName);
            if (sc.Status  == ServiceControllerStatus.Running)
            {
                sc.Dispose();
                return true;
            }
            else
            {
                //stop service if running
                //bool stopped = StopService(internalServiceName);

                //start service
                bool started = StartService();

                sc.Dispose();
                return started;// && stopped;
            }
        }

        //start the service method
        private static bool StartService()
        {
            bool result = false;

            ServiceController service = new ServiceController(internalServiceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(internalLocalTimeout);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("The: " + internalServiceName + " started successfully");

                result = true;
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info(String.Format("{0} {1} {2} {3}", "The windows service:", internalServiceName, "did not start. Esception:", ex.Message));
            }
            finally
            {
                service.Dispose();
            }

            return result;
        }

        //dtop the service if it is trully running
        public static bool StopService(string serviceName)
        {
            bool result = false;

            ServiceController service = new ServiceController(serviceName);

            if (service.Status != ServiceControllerStatus.Stopped)
            {
                try
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(internalLocalTimeout);

                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info("The: " + internalServiceName + " stopped successfully");

                    result = true;
                }
                catch(Exception ex)
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info(String.Format("{0} {1} {2} {3}", "The windows service:", internalServiceName,
                        "cannot be stopped Esception:", ex.Message));
                }
            }
            else
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("The: " + internalServiceName + " did not stop. It was already stopped.");

                result = true;
            }

            service.Dispose();
            return result;
        }

        #endregion

        #region Setting Game Setup

        public static bool SetGameSetup()
        {
            //create request
            AssignGameSetupDataRequest request = new AssignGameSetupDataRequest()
            {
                AmountOfPlayers = MainWindow.SavedData.SavedAmountOfPlayers,
                AmountOfDecks = MainWindow.SavedData.SavedAmountOfDecks,
                AmountOfJokers = MainWindow.SavedData.SavedAmountOfJokers,
                AmountOfStartCards = MainWindow.SavedData.SavedAmountOfStartCards,
                RoomPassword = password,
                HostID = MainWindow.PlayerID,
                HostName = MainWindow.SavedData.CurrentPlayerName,
                JoiningTimeoutEnabled = MainWindow.SavedData.JoiningTimeoutEnabled,
                JoiningTimeoutInMinutes = MainWindow.SavedData.JoiningTimeoutMinutesAmount,
                ReadinessTimeoutEnabled = MainWindow.SavedData.ReadinessTimeoutEnabled,
                ReadinessTimeoutInMinutes = MainWindow.SavedData.ReadinessTimeoutMinutesAmount,
            };

            //create new proxy
            IMakaoGameHostService proxy = factory.CreateChannel();

            bool ssetupAssigned = proxy.AssignGameSetupData(request);
            return ssetupAssigned;
        }

        #endregion

        #region AddHostPlayerToTheRoom

        //method for adding the host player to the room
        public static AddNewPlayerResponse AddHostPlayerToRoom()
        {           
            //create the request
            AddNewPlayerRequest addPlayerRequest = new AddNewPlayerRequest(){ PlayerName = MainWindow.SavedData.CurrentPlayerName,
                PlayedGames = MainWindow.SavedData.SavedAmountOfPlayedGames, PlayerEndpoint = MainWindow.ThisClientEndpoint, Password = password,
            PlayedAndWonGames = MainWindow.SavedData.SavedAmountOfPlayedAndWonGames};

            //creating new proxy
            IMakaoGameHostService proxy = factory.CreateChannel();

            //sending data without an image
            AddNewPlayerResponse response = proxy.CreateNewRoomAndHostPlayer(addPlayerRequest);

            //if user was added to room
            if (response.AddedToGame)
            {
                //assigning an ID for player
                MainWindow.PlayerID = response.PlayerID;
                MainWindow.PlayerNumber = response.PlayerNumber;
                MainWindow.IsHostPlayer = response.IsHostPlayer;
                MainWindow.TotalAMountOfPlayerObtainedFromHost = response.TotalAMountOfPlayers;

                //assign data to player info storage in client service
                CurrentPlayerDataStorage.PlayerNumber = response.PlayerNumber;
                CurrentPlayerDataStorage.PlayerID = response.PlayerID;
                CurrentPlayerDataStorage.PlayerName = response.PlayerName;

                //change app status
                MainWindow.ChangeGameStatus(response.IsHostPlayer);

                //sending data with image
                MemoryStream avatarMemoryStream = new MemoryStream();
                byte[] bytes = File.ReadAllBytes(MainWindow.SavedData.CurrentAvatarPicture);
                avatarMemoryStream.Write(bytes, 0, bytes.Length);
                avatarMemoryStream.Position = 0;

                //calling method for sending to and downloding avatar from host
                SendAnImage(avatarMemoryStream, response.PlayerNumber, proxy);
            }            

            return response;
        }

        //internal method for sending user's current avatar to the host
        private static void SendAnImage(MemoryStream avatarMemoryStream, int playerNumber, IMakaoGameHostService proxy)
        {
            //uploading an avatar image
            bool streamSuccess = SendAvatarToHost(avatarMemoryStream, playerNumber, proxy);

            //if image succesfully saved in host, download it back and save
            if (streamSuccess)
            {
                Stream downloadedImage = proxy.DownloadAvatarImage(playerNumber);
                SaveAnImage(downloadedImage, playerNumber);
            }
        }

        //method for sending avatar to the host (host will store it)
        public static bool SendAvatarToHost(MemoryStream avatarMemoryStream, int playerNumber, IMakaoGameHostService proxy)
        {
            //building game host service request object for sending an avatar
            SaveAvatarRequest addAvatarRequest = new SaveAvatarRequest()
            {
                AvatarStream = avatarMemoryStream,
                PlayerNumber = playerNumber,
            };

            //and serializing it
            MemoryStream requestSerialized = EngineHost.DataPlaceholders.AvatarSerializerDeserializer.SerializeToStream(addAvatarRequest);
            requestSerialized.Position = 0;

            //uploading an avatar image
            bool streamSuccess = proxy.UploadAvatarImagePlayer(requestSerialized);

            return streamSuccess;
        }

        //saving player avatar as new png image in special folder
        public static void SaveAnImage(Stream input, int playerNumber)
        {
            string imageNumber;
            if (playerNumber < 10) imageNumber = "0" + playerNumber.ToString();
            else imageNumber = playerNumber.ToString();

            Image img = System.Drawing.Image.FromStream(input);
            try
            {
                img.Save(MainWindow.AvatarsTempLocation + imageNumber + ".png", ImageFormat.Png);
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Saving image from the string for player " + playerNumber + " ended successfully");
            }
            catch (Exception ex)
            {
                img.Save(MainWindow.AvatarsTempLocation + "1" + imageNumber + ".png", ImageFormat.Png);
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Saving image from the string for player " + playerNumber + " failed: " + ex.Message);
            }
        }

        #endregion
    }
}
