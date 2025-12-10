using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Local_Network_Scanner.Model
{
    public class LocalNetworksInfo
    {
        public string Name { get; set; }
        public string IPv4Address { get; set; }
        public string SubnetMask { get; set; }

        public string CIDR { get; set; }

        public string NetworkAddress { get; set; }
        public string BroadcastAddress { get; set; }

        public string DisplayNetworkName => $"{Name} ({IPv4Address})";
    }
}
