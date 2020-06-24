using System;
using System.Collections.Generic; 
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace MakaoGraphicsRepresentation
{
    static class MakaoGameClientServiceAddresObtainer
    {
        private static string thisClientIPAddress;

        static MakaoGameClientServiceAddresObtainer()
        {
            thisClientIPAddress = "0.0.0.0";
        }

        #region Pblic methods for obtaining IP addresses or Uri

        //method that returns service endpoint
        //service hosted in every player computer
        public static Uri GetClientServiceEndpoint()
        {
            IPtype ProperIP = GetProperIPAddress(GetLocalIpList());
            Uri endpoint;

            //if no founded addresses, or null - stop the service
            if (ProperIP.IPaddress != "0.0.0.0" || ProperIP.ConnectionType == IPConnectionType.None)
            {
                endpoint = new Uri(String.Concat(@"http://", ProperIP.IPaddress, @":9501/MakaoGameCliestServiceHost"));

                thisClientIPAddress = ProperIP.IPaddress.ToString();

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Assigned client endpoint: " + endpoint.ToString());
            }
            else
            {
                endpoint = null;
            }

            return endpoint;
        }

        public static string GetLocalIPAddressString()
        {
            return thisClientIPAddress;
        }

        //method for obtaining only IP address - local address
        public static string GetLocalIPAddress()
        {
            IPtype ProperIP = GetProperIPAddress(GetLocalIpList());
            return ProperIP.IPaddress;
        }

        #endregion

        #region Collecting list of IPs or specific IP

        //method for obtaining list of current IP addresses, that
        //have connection to the internet
        private static List<IPtype> GetLocalIpList()
        {
            List<IPtype> IPlist = new List<IPtype>();

            //checking if computer is connected to internet
            bool connected = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            if (connected)
            {
                //get list of active internet adapters
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var adapter in interfaces)
                {
                    //if it is Wi-Fi or Ethernet, add Its IP to the list
                    if(adapter.Name == "Wi-Fi" || adapter.Name.Contains("Ethernet"))
                    {
                        var ipProps = adapter.GetIPProperties();
                        foreach (var ip in ipProps.UnicastAddresses)
                        {
                            //Add only if it works
                            if ((adapter.OperationalStatus == OperationalStatus.Up)
                            && (ip.Address.AddressFamily == AddressFamily.InterNetwork))
                            {
                                IPConnectionType type = IPConnectionType.None;
                                if (adapter.Name == "Wi-Fi") type = IPConnectionType.WiFi;
                                else if (adapter.Name == "Ethernet") type = IPConnectionType.Ethernet;

                                IPtype IP = new IPtype()
                                {
                                    IPaddress = ip.Address.ToString(),
                                    ConnectionType = type,
                                };
                                IPlist.Add(IP);
                                var logger = NLog.LogManager.GetCurrentClassLogger();
                                logger.Info("Founded IP address: {0}", ip.Address.ToString());
                            }
                        }
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

        //method for chosing only ine IP address
        private static IPtype GetProperIPAddress(List<IPtype> IPconnectionsList)
        {
            //if there is at least one IP
            if (IPconnectionsList.Count > 0)
            {
                //if there is at least one Wi-Fi IP
                int addressNumber = 10000;
                for (int i = 0; i < IPconnectionsList.Count; i++)
                {
                    if (IPconnectionsList[i].ConnectionType == IPConnectionType.WiFi)
                    {
                        addressNumber = i;
                        break;
                    }
                }

                //if there is at least one Ethernet IP, if there is no Wi-Fi address
                if (addressNumber > 9999)
                {
                    for (int i = 0; i < IPconnectionsList.Count; i++)
                    {
                        if (IPconnectionsList[i].ConnectionType == IPConnectionType.Ethernet)
                        {
                            addressNumber = i;
                            break;
                        }
                    }
                }

                return IPconnectionsList[addressNumber];
            }
            else
            {
                return new IPtype() { IPaddress = "0.0.0.0", ConnectionType = IPConnectionType.None };
            }
        }

        #endregion

    }

    #region Data for storage information about founded IPs

    //class for saving what kind of connection has IP address
    public class IPtype
    {
        public string IPaddress { get; set; }
        public IPConnectionType ConnectionType { get; set; }
    }

    //enum for connection type
    public enum IPConnectionType
    {
        WiFi,
        Ethernet,
        None,
    }

    #endregion
}
