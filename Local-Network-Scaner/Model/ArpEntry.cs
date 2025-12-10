using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Local_Network_Scanner.Model
{
    public class ArpEntry
    {
        public string IPAddress { get; set; }
        public string MACAddress { get; set; }
        public string Type { get; set; } // e.g., "dynamic" or "static"
    }
}
