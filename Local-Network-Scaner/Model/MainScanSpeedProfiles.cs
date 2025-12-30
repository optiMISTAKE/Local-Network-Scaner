using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Local_Network_Scanner.Model
{
    public static class MainScanSpeedProfiles
    {
        public static MainScanProfile FromPreset(ScanSpeedPreset preset)
        {
            return preset switch
            {
                ScanSpeedPreset.Slow => new MainScanProfile(
                    HostMaxConcurrency: 10,
                    PingTimeoutFast: 800,
                    PingTimeoutSlow: 1500,
                    ReverseDnsTimeout: 1000,
                    TcpPortTimeout: 1200,
                    TcpPortConcurrency: 20,
                    BannerGrabTimeout: 2000,
                    BannerGrabConcurrency: 2
                ),

                ScanSpeedPreset.Aggressive => new MainScanProfile(
                    HostMaxConcurrency: 100,
                    PingTimeoutFast: 150,
                    PingTimeoutSlow: 400,
                    ReverseDnsTimeout: 300,
                    TcpPortTimeout: 300,
                    TcpPortConcurrency: 200,
                    BannerGrabTimeout: 500,
                    BannerGrabConcurrency: 10
                ),

                _ => // Normal (fallback)
                    new MainScanProfile(
                        HostMaxConcurrency: 50,
                        PingTimeoutFast: 300,
                        PingTimeoutSlow: 1000,
                        ReverseDnsTimeout: 500,
                        TcpPortTimeout: 500,
                        TcpPortConcurrency: 100,
                        BannerGrabTimeout: 1000,
                        BannerGrabConcurrency: 5
                    )
            };
        }
    }
}
