using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using MakaoGameHostService.ServiceContracts;

namespace MakaoGameConsoleTestClient
{
    class Program
    {
        private static EndpointDiscoveryMetadata discoveredService;

        static void Main()
        {
            Console.WriteLine("Discoverying of service: IMakaoGameHostService, started.");
            ShowIpList();
            TryToFindServiceAsync();
            Console.WriteLine("Waiting for the client to find service...");
            Console.ReadLine();
        }

        private static void TryToFindServiceAsync()
        {
            Console.WriteLine("Trying to find service");

            DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
            discoveryClient.FindProgressChanged += new
                EventHandler<FindProgressChangedEventArgs>(DiscoveryClient_FindProgressChanged);
            discoveryClient.FindCompleted += new
                EventHandler<FindCompletedEventArgs>(DiscoveryClient_FindCompleted);
            discoveryClient.FindAsync(new FindCriteria(typeof(IMakaoGameHostService))
            {
                Duration = TimeSpan.FromSeconds(10)
            });
        }

        private static void DiscoveryClient_FindProgressChanged(object sender, FindProgressChangedEventArgs e)
        {
            Console.WriteLine("I have found {0}", e.EndpointDiscoveryMetadata.Address.ToString());

            Console.WriteLine("\nContracts");
            foreach (var item in e.EndpointDiscoveryMetadata.ContractTypeNames)
            {
                Console.WriteLine(item.ToString());
            }
            Console.WriteLine("\nExtensions");
            foreach (var item in e.EndpointDiscoveryMetadata.Extensions)
            {
                Console.WriteLine(item.ToString());
            }
            Console.WriteLine("\nListenUris");
            foreach (var item in e.EndpointDiscoveryMetadata.ListenUris)
            {
                Console.WriteLine(item.ToString());
            }
            Console.WriteLine("\nScopes");
            foreach (var item in e.EndpointDiscoveryMetadata.Scopes)
            {
                Console.WriteLine(item.ToString());
            }
            Console.WriteLine("\nVersion");
            Console.WriteLine(e.EndpointDiscoveryMetadata.Version.ToString());
        }

        private static void DiscoveryClient_FindCompleted(object sender, FindCompletedEventArgs e)
        {
            Console.WriteLine("Find finished");
            if (e.Result.Endpoints.Count > 0)
            {
                discoveredService = e.Result.Endpoints[0];
                CallService();
            }
            else
            {
                Console.WriteLine("Service not found");
            }

            Console.WriteLine("Now the client really finished");
            Console.ReadLine();
        }

        private static void CallService()
        {
            Console.WriteLine("\nCalling the address: " + discoveredService.Address.ToString() + "\n");
            /*
            ChannelFactory<IMakaoGameHostService> factory =
                new ChannelFactory<IMakaoGameHostService>(new BasicHttpBinding(), discoveredService.Address);
            IMakaoGameHostService proxy = factory.CreateChannel();
            var response = proxy.StartNewGameRequest(
                new MakaoGameHostService.Messages.CreateGameRequest()
                {
                    AmountOfPlayers = 4,
                    AmountOfDecks = 2,
                    AmountOfJokersInDeck = 3,
                    Test = false,
                    AmountOfCardsForOnePlayer = 5
                });

            Console.WriteLine("\nReceived response:");
            Console.WriteLine(response.GameSuccessfullyCreated.ToString());
            Console.WriteLine(response.TestInteger.ToString());
            */
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
        }

        private static List<string> GetLocalIpList()
        {
            //checking if computer is connected to internet
            bool connected = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            if (connected)
            {
                List<string> IPlist = new List<string>();
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        IPlist.Add(ip.ToString());
                    }
                }
                return IPlist;
            }
            else
            {
                return null;
            }
        }

        private static void ShowIpList()
        {
            List<string> IPlist = GetLocalIpList();

            if ((IPlist != null) && (IPlist.Count > 0))
            {
                foreach (string item in IPlist)
                {
                    Console.WriteLine("I have found IP address: {0} \n", item);
                }
            }
        }
    }
}
