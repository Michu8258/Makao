using MakaoGameClientService.ServiceContracts;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Windows;

namespace MakaoGraphicsRepresentation
{
    public static class EngineClientHandler
    {
        #region Private fields

        //host instance
        private static ServiceHost host;

        //Synchronization context
        private static SynchronizationContext SynchCont;

        //flag that indicates that class was inicialized
        private static bool wasInicialiced;

        //current endpoint
        private static Uri endpoint;
        private static Uri secondEndpoint;

        //factoryChannel
        private static ChannelFactory<IMakaoGameClientService> factory;

        //client service could not start
        private static bool clientServiceNotStarted;
        public static bool ClienrServiceNotStarted { get { return clientServiceNotStarted; } }

        #endregion

        #region Public properties

        public static Uri CurrentEndpoint { get { return endpoint; } }
        public static ChannelFactory<IMakaoGameClientService> CurrentChannelFactory { get { return factory; } }

        #endregion

        #region Constructing and destroying the static class

        //method for starting Makao Game Client Service - selfhost in another thread
        public static void StartTheMakaoGameClientService(Uri endpoint)
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var udpListenerPorts = properties.GetActiveTcpListeners().Where(n => n.Port >= 9501).Select(n => n.Port);

            var port = Enumerable.Range(9501, ushort.MaxValue).Where(i => !udpListenerPorts.Contains(i)).FirstOrDefault();

            string newEndpointString = endpoint.ToString().Replace("9501", port.ToString());
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"New endpoint 9502: {newEndpointString}");
            secondEndpoint = new Uri(String.Concat(newEndpointString));

            MainWindow.SetThisClientEndpoint(secondEndpoint);
            StartGameClientService(secondEndpoint);
        }

        private static void StartGameClientService(Uri inputEndpoint)
        {
            //Synchronization context and endpoint assignment
            SynchCont = SynchronizationContext.Current;
            endpoint = inputEndpoint;

            //assigning factry channel value
            factory = new ChannelFactory<IMakaoGameClientService>(new BasicHttpBinding(),
            new EndpointAddress(inputEndpoint));

            //while creating the host it is by default considered that client will start
            clientServiceNotStarted = false;

            Thread thread = new Thread(() => StartHost(inputEndpoint));
            thread.Start();
            wasInicialiced = true;
        }

        //method for shutting the client service down
        public static void Dispose()
        {
            if (wasInicialiced)
            {
                try
                {
                    host.Close();
                }
                catch (Exception ex)
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Error("Makao Game Client Service could not be stopped: " + ex.Message);
                }

                endpoint = null;
                wasInicialiced = false;
            }
        }

        #endregion

        #region Starting the service and checking if it is alive

        private static void StartHost(Uri endpoint)
        {
            RunInlineHostConfiguration(endpoint);
        }

        //configura and open host
        private static void RunInlineHostConfiguration(Uri endpoint)
        {
            try
            {
                //creation of the url - with changable IP address
                //http://192.168.1.107:9501/MakaoGameCliestServiceHost
                Uri url = endpoint;
                host = new ServiceHost(typeof(MakaoGameClientService.ServiceImplementations.MakaoGameClientServiceImplementation), url);

                //Enable MEX
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    HttpsGetEnabled = true,
                };
                host.Description.Behaviors.Clear();

                //adding service behaviors to list
                host.Description.Behaviors.Add(new ServiceBehaviorAttribute());
                host.Description.Behaviors.Add(smb);
                host.Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });

                //Add endpoints
                host.AddServiceEndpoint(typeof(IMakaoGameClientService), new BasicHttpBinding(), url);
                host.AddServiceEndpoint(new ServiceMetadataEndpoint(new EndpointAddress(url + "/mex")));

                //opening the host
                host.Open();

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Client started");
            }
            catch (Exception ex)
            {
                //log eny exception
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "Exception while configuring host: " + ex.Message;
                logger.Info(text);

                //save info about the fact that client service did not start properly
                clientServiceNotStarted = true;

                MessageBox.Show("Uruchomienie wewnętrznego klienta gry nie powiodło się.",
                    "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //calling the method that returns true if the service is running
        public static bool CheckIfServiceIsAlive()
        {
            bool response = false;
            try
            {
                IMakaoGameClientService proxy = factory.CreateChannel();
                bool proxyResponse = proxy.CheckIfServiceIsWorking();

                response = proxyResponse & !clientServiceNotStarted;
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("MakaoGameClientService is not running: " + ex.Message);
            }
            return response;
        }

        #endregion
    }
}
