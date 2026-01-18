using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
                    return Application.Current.Resources["Device_PortsNone"] as string ?? "None";
                return string.Join(", ", OpenPorts);
            }
        }

        public string IsActiveDisplay
        {
            get
            {
                if (IsActive)
                    return Application.Current.Resources["Device_Active"] as string ?? "Active";
                else
                    return Application.Current.Resources["Device_Inactive"] as string ?? "Inactive";
            }
        }

        public string VendorDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(Vendor))
                {
                    return Application.Current.Resources["Device_VendorUnknown"] as string
                           ?? "OUI record not found...";
                }
                return Vendor;
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
