using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Local_Network_Scanner.Model
{
    public class DeviceInfo
    {
        public string IPAddress { get; set; }
        public bool IsActive { get; set; }
        public List<int> OpenPorts { get; set; } = new List<int>();
        public Dictionary<int, string> PortBanners { get; } = new Dictionary<int, string>();
        public string MACAddress { get; set; }
        public string Vendor { get; set; }
        public string HostName { get; set; }

        public string OpenPortsDisplay
        {
            get
            {
                if (OpenPorts == null || OpenPorts.Count == 0)
                    return "None";
                return string.Join(", ", OpenPorts);
            }
        }

        public string IsActiveDisplay
        {
            get
            {
                return IsActive ? "Active" : "Inactive";
            }
        }

        public string VendorDisplay
        {
            get
            {
                return string.IsNullOrEmpty(Vendor) ? "OUI record not found in the database. The given MAC address is most probably random or private" : Vendor;
            }
        }

        public string Banner
        {
            get
            {
                if (PortBanners.Count == 0)
                    return string.Empty;

                return string.Join(" | ",
                    PortBanners
                        .OrderBy(p => p.Key)
                        .Select(p => $"{p.Key}: {p.Value}")
                );
            }
        }

    }
}
