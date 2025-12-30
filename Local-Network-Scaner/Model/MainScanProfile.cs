using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Local_Network_Scanner.Model
{
    public record MainScanProfile
    (
        int HostMaxConcurrency,
        int PingTimeoutFast,
        int PingTimeoutSlow,
        int ReverseDnsTimeout,
        int TcpPortTimeout,
        int TcpPortConcurrency,
        int BannerGrabTimeout,
        int BannerGrabConcurrency
    );
}
