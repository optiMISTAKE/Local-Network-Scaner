using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Local_Network_Scaner.Model
{
    public class DeviceInfo
    {
        public string IPAddress { get; set; }
        public bool IsActive { get; set; }
        public int[] OpenPorts { get; set; }
        public string Banner { get; set; }
        public string MACAddress { get; set; }
        public string Vendor { get; set; }
        public string HostName { get; set; }
    }
}
