using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Local_Network_Scanner.Model;
using System.Net.NetworkInformation;
using System.Net;

namespace Local_Network_Scanner.Services
{
    public class NetworkInterfaceService
    {
        public List<LocalNetworksInfo> GetActiveNetworkInterfaces()
        {
            var results = new List<LocalNetworksInfo>();

            foreach (NetworkInterface netIn in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Skip disconnected and loopback interfaces
                if (netIn.OperationalStatus != OperationalStatus.Up ||
                    netIn.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                var ipProps = netIn.GetIPProperties();

                foreach(var unicastAddr in ipProps.UnicastAddresses)
                {
                    // Skip non-IPv4 addresses
                    if (unicastAddr.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        continue;
                    }
                    
                    string ipv4 = unicastAddr.Address.ToString();
                    string subnetMask = unicastAddr.IPv4Mask.ToString();
                    int prefix = SubnetMaskToCIDR(subnetMask);
                    string networkAddress = CalculateNetworkAddress(ipv4, subnetMask);
                    string broadcastAddress = CalculateBroadcastAddress(ipv4, subnetMask);

                    results.Add(new LocalNetworksInfo
                    {
                        Name = netIn.Name,
                        IPv4Address = ipv4,
                        SubnetMask = subnetMask,
                        CIDR = $"/{prefix}",
                        NetworkAddress = networkAddress,
                        BroadcastAddress = broadcastAddress
                    });
                }
            }

            return results;

        }

        // Converts the subnet mask to CIDR notation using bit counting
        private int SubnetMaskToCIDR(string subnetMask)
        {
            var bytes = IPAddress.Parse(subnetMask).GetAddressBytes();
            int cidr = 0;
            foreach (var b in bytes)
            {
                cidr += Convert.ToString(b, 2).Count(c => c == '1');
            }
            return cidr;
        }

        // Calculates the broadcast address given an IP and subnet mask
        private string CalculateNetworkAddress(string ipAddress, string subnetMask)
        {
            byte[] ipBytes = IPAddress.Parse(ipAddress).GetAddressBytes();
            byte[] maskBytes = IPAddress.Parse(subnetMask).GetAddressBytes();
            byte[] networkBytes = new byte[ipBytes.Length];
            for (int i = 0; i < ipBytes.Length; i++)
            {
                networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
            }
            return new IPAddress(networkBytes).ToString();
        }

        // Calculates the broadcast address given an IP and subnet mask
        private string CalculateBroadcastAddress(string ipAddress, string subnetMask)
        {
            byte[] ipBytes = IPAddress.Parse(ipAddress).GetAddressBytes();
            byte[] maskBytes = IPAddress.Parse(subnetMask).GetAddressBytes();
            byte[] broadcastBytes = new byte[ipBytes.Length];
            for (int i = 0; i < ipBytes.Length; i++)
            {
                broadcastBytes[i] = (byte)(ipBytes[i] | (~maskBytes[i]));
            }
            return new IPAddress(broadcastBytes).ToString();
        }
    }
}
