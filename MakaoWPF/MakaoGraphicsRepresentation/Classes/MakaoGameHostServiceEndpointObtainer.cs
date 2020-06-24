using System;
using System.Collections.Generic;
using System.ServiceModel.Discovery;
using MakaoGameHostService.ServiceContracts;

namespace MakaoGraphicsRepresentation
{
    static class MakaoGameHostServiceEndpointObtainer
    {
        #region Get host endpoint

        //method for finding endpoint of Makao engine host (Windows Service)
        //while current player's computer is hosting the service
        public static Uri GetHostEndpointWhileBeeingHost()
        {
            string IPaddress = MakaoGameClientServiceAddresObtainer.GetLocalIPAddress();
            Uri endpoint;
            if (IPaddress != "0.0.0.0") endpoint = new Uri(String.Concat(@"http://" + IPaddress + @":9500/MakaoGameHostWindowsService"));
            else endpoint = null;
            return endpoint;
        }

        //method for finding endpoints of Makao engine host (Windows Service)
        //while current player's computer is not host
        public static void StartSearchingHostEndpointWhileNotBeingHost()
        {
            TryToFindServiceAsync();
        }

        #endregion

        #region Service discovery - search start

        //method that starts searching matching endpoints asynchrously
        private static void TryToFindServiceAsync()
        {
            DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
            discoveryClient.FindProgressChanged += new EventHandler<FindProgressChangedEventArgs>(DiscoveryClient_FindProgressChanged);
            discoveryClient.FindCompleted += new EventHandler<FindCompletedEventArgs>(DiscoveryClient_FindCompleted);
            discoveryClient.FindAsync(new FindCriteria(typeof(IMakaoGameHostService))
            {
                Duration = TimeSpan.FromSeconds(10)
            });

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Searching for host endpoint in async way started.");
        }

        #endregion

        #region Finding new endpoint events

        //event that occurs while some matching endpoint is found
        private static void DiscoveryClient_FindProgressChanged(object sender, FindProgressChangedEventArgs e)
        {
            try
            {
                //create list of founded endpoints (here is ony one item)
                Uri endpoint = new Uri(e.EndpointDiscoveryMetadata.Address.ToString());
                List<Uri> EndpointList = new List<Uri>
                {
                    endpoint
                };
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Searching Host endpoints - new endpoint founded: " + endpoint.ToString());

                OnFoundedNewEndpoint(EndpointList);
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info($"Error while founding new wndpoint: {ex.Message}.");
            }
        }

        //searching for addresses finished
        private static void DiscoveryClient_FindCompleted(object sender, FindCompletedEventArgs e)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            //create list of founded endpoints
            List<Uri> endpointsList = new List<Uri>();
            try
            {
                foreach (var item in e.Result.Endpoints)
                {
                    Uri endpoint = new Uri(item.Address.ToString());
                    endpointsList.Add(endpoint);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Error while completing search for host endpoints: {ex.Message}. Inner exception: {ex.InnerException}. Stack trace: {ex.StackTrace}.");
            }

            logger.Info("Searching Host endpoints finished. Endpoints founded: " + endpointsList.Count.ToString());

            OnSearchingForEndpointsFinished(endpointsList);
        }

        #endregion

        #region Events for passing founded endpoints

        //searching progress - found new uri
        public delegate void FoundedNewEndpointEventHandler(object sender, EndpointEventArgs e);
        public static event FoundedNewEndpointEventHandler FoundedNewEndpoint;
        static void OnFoundedNewEndpoint(List<Uri> endpointsList)
        {
            FoundedNewEndpoint?.Invoke(null, new EndpointEventArgs { EndpointsList = endpointsList });
        }

        public delegate void FoundedAllEndpointsEventHandler(object sender, EndpointEventArgs e);
        public static event FoundedAllEndpointsEventHandler SearchingForEndpointsFinished;
        static void OnSearchingForEndpointsFinished(List<Uri> endpointsList)
        {
            SearchingForEndpointsFinished?.Invoke(null, new EndpointEventArgs { EndpointsList = endpointsList });
        }

        #endregion
    }

    #region Events arguments classes

    public class EndpointEventArgs : EventArgs
    {
        public List<Uri> EndpointsList;
    }

    #endregion
}
