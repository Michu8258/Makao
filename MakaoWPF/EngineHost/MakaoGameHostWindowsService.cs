using EngineHost.ServiceImplementation;
using MakaoGameHostService.ServiceContracts;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.ServiceProcess;

namespace EngineHost
{
    public partial class MakaoGameHostWindowsService : ServiceBase
    {
        //private fields
        private ServiceHost host;
        private List<string> IPaddresses;

        //constructor
        public MakaoGameHostWindowsService()
        {
            InitializeComponent();

            //NLOG configutation methos
            string logsLocation = DataPlaceholders.MakaoEngineHostDataPlaceholders.CurrentLogFile;
            NLogConfigMethod(logsLocation);
        }

        #region NLog configuration

        //Nlog configuration
        private void NLogConfigMethod(string source)
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = source };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;

            //log construction of the engine
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("EngineHost service started");
        }

        #endregion

        #region Establishing list of IP addresses

        //method that returns the list of current IP addresses with internet connection
        private List<string> GetLocalIpList()
        {
            List<string> IPlist = new List<string>();

            //checking if computer is connected to internet
            bool connected = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            if (connected)
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        IPlist.Add(ip.ToString());
                        var logger = NLog.LogManager.GetCurrentClassLogger();
                        logger.Info("Founded IP address: {0}", ip);
                    }
                }
            }
            else
            {
                //log the fact, that there is no internet connection
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("There is no valid internet connection");
                IPlist.Clear();
            }

            return IPlist;
        }

        #endregion

        #region Starting and stopping the service

        //when starting, check if there is an internet connection
        //and if there is not, ctop the service
        protected override void OnStart(string[] args)
        {
            try
            {
                IPaddresses = GetLocalIpList();

                //confirm founded IP addresses
                foreach (string item in IPaddresses)
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info("Confirming founded arrdesses: {0}", item);
                }

                //if no founded addresses, or null - stop the service
                if ((IPaddresses == null) || (IPaddresses.Count == 0))
                {
                    OnStop();
                }
                else
                {
                    RunInlineHostConfiguration(IPaddresses[0]);
                }
            }
            catch (Exception ex)
            {
                //log eny exception
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "Exception while starting host: " + ex.Message;
                logger.Error(text);
            }
        }

        //method for stopping the service
        protected override void OnStop()
        {
            if (host != null)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("The host application is closing");
                host.Close();
                host = null;
            }
        }

        #endregion

        #region Service configuration

        private void RunInlineHostConfiguration(string IPaddress)
        {
            try
            {
                //creation of the url - with changable IP address
                Uri url = new Uri(String.Concat(@"http://", IPaddress, @":9500/MakaoGameHostWindowsService"));
                host = new ServiceHost(typeof(EngineHostServiceImplementation), url);

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

                //Add custom binding
                BasicHttpBinding basicHTTPBinding = new BasicHttpBinding
                {
                    TransferMode = TransferMode.Streamed
                };
                basicHTTPBinding.ReaderQuotas.MaxArrayLength = 100000;

                //add endpoints
                host.AddServiceEndpoint(typeof(IMakaoGameHostService), basicHTTPBinding, url);
                host.AddServiceEndpoint(new ServiceMetadataEndpoint(new EndpointAddress(url + "/mex")));

                //enable Discoverying
                host.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
                host.AddServiceEndpoint(new UdpDiscoveryEndpoint());

                //opening the host
                host.Open();

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Host started");
            }
            catch (Exception ex)
            {
                //log eny exception
                var logger = NLog.LogManager.GetCurrentClassLogger();
                string text = "Exception while configuring host: " + ex.Message;
                logger.Info(text);
            }
        }

        #endregion
    }
}
