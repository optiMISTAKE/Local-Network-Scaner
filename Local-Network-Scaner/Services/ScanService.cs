using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Local_Network_Scanner.Model;
using static Local_Network_Scanner.Services.TcpConnectActiveService;
using System.IO;

namespace Local_Network_Scanner.Services
{
    public class ScanService
    {
        private readonly OuiDatabaseService _ouiDb = new OuiDatabaseService();
        private readonly SimplePingService _pingService = new SimplePingService();
        private readonly ArpService _arpService = new ArpService();
        private readonly TcpConnectActiveService _tcpConnectService = new TcpConnectActiveService();
        private readonly ReverseDnsService _reverseDnsService = new ReverseDnsService();
        private readonly TcpActBannerGrabService _tcpBannerGrabService = new TcpActBannerGrabService();

        // TO-DO: Delete the following later
        private const bool DEV_TEST_BANNERS = true;


        // Parameterless constructor to load OUI database
        public ScanService()
        {
            // Load the OUI database on initialization
            string path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources",
            "oui.csv"
            );

            _ouiDb.LoadDatabaseCSV(path);
        }
        public async Task ScanSubnetAsync(string currentIp, string[] maskParts, ScanSpeedPreset preset,
            IProgress<DeviceInfo> progress, IProgress<int> scanProgress, CancellationToken cancellationToken)
        {
            int scannedCount = 0;
            
            var profile = MainScanSpeedProfiles.FromPreset(preset);

            using var sem = new SemaphoreSlim(profile.HostMaxConcurrency);

            var tasks = new List<Task>();

            (int[] firstIp, int[] endIp) = IpRangeService.GetIpRange(currentIp, maskParts);
            Debug.WriteLine($"Scanning IP range: {string.Join('.', firstIp)} - {string.Join('.', endIp)}");

            uint start = HelperIpConverter.IpToUInt(firstIp);
            uint end = HelperIpConverter.IpToUInt(endIp);

            uint totalHosts = end - start + 1;

            if (totalHosts > 10000)
                throw new InvalidOperationException("Subnet too large to scan safely.");
            // TO-DO: allow user to decide whether to proceed or not.

            for (uint ip = start; ip <= end; ip++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string ipAddress = HelperIpConverter.UIntToIp(ip);
                Debug.WriteLine($"Scanning {ipAddress}");

                await sem.WaitAsync(cancellationToken);

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var deviceInfo = await ScanSingleHost(ipAddress, profile, cancellationToken);
                        if (deviceInfo != null && deviceInfo.IsActive)
                        {
                            progress.Report(deviceInfo);
                        }
                    }
                    finally
                    {
                        Interlocked.Increment(ref scannedCount);
                        scanProgress.Report(scannedCount);
                        sem.Release();
                    }
                }, cancellationToken));
            }

            await Task.WhenAll(tasks);

        }

        public async Task<DeviceInfo> ScanSingleHost(string ipAddress, MainScanProfile profile, CancellationToken cancellationToken)
        {
            var device = new DeviceInfo { IPAddress = ipAddress };

            // 1. Ping the host
            cancellationToken.ThrowIfCancellationRequested();
            device.IsActive = await _pingService.PingAsync(ipAddress, profile.PingTimeoutFast, cancellationToken);

            if (!device.IsActive)
            {
                device.IsActive = await _pingService.PingAsync(ipAddress, profile.PingTimeoutSlow, cancellationToken);
            }

            // 2. Reverse DNS Lookup
            cancellationToken.ThrowIfCancellationRequested();
            if (device.IsActive)
            {
                device.HostName = await _reverseDnsService.TryGetHostname(ipAddress, profile.ReverseDnsTimeout);
            }

            // 3. Get MAC Address and Vendor
            cancellationToken.ThrowIfCancellationRequested();
            string? hostMacAddress = _arpService.GetMacAddress(ipAddress);
            device.MACAddress = hostMacAddress;

            if (!string.IsNullOrEmpty(device.MACAddress))
            {
                OuiRecord ouiRecord = _ouiDb.GetVendor(device.MACAddress);
                Debug.WriteLine($"OUI Lookup for {device.MACAddress}: {ouiRecord?.Vendor}");
                if (ouiRecord != null)
                {
                    device.Vendor = ouiRecord.Vendor;
                }
            }

            // 4. TCP Connect Scan
            cancellationToken.ThrowIfCancellationRequested();
            int timeout = profile.TcpPortTimeout;
            using var sem = new SemaphoreSlim(profile.TcpPortConcurrency);
            var tasks = new List<Task>();

            foreach (var port in _tcpConnectService.CommonPorts)
            {
                await sem.WaitAsync(cancellationToken);
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var result = await _tcpConnectService.ProbeTcpPort(ipAddress, port, timeout, cancellationToken);
                        
                        if (result.IsOpen)
                        {
                            lock (device.OpenPorts)
                                device.OpenPorts.Add(port);
                        }

                        if (DEV_TEST_BANNERS && device.OpenPorts.Count > 0)
                        {
                            device.PortBanners[22] =
                                "SSH-2.0-OpenSSH_9.3p1 Debian-1+deb12u1 Protocol mismatch detected. " +
                                "This is a simulated banner used for UI testing and text wrapping validation.";

                            device.PortBanners[80] =
                                "HTTP/1.1 200 OK Server: nginx/1.24.0 (Ubuntu) " +
                                "Content-Type: text/html; charset=UTF-8 Connection: keep-alive " +
                                "This banner is intentionally very long to exceed 120 characters.";
                        }

                        return result;
                    }
                    finally
                    {
                        sem.Release();
                    }
                }, cancellationToken));
            }

            // 5. TCP Active Banner Grab (for open ports only)
            // ... (TO-DO)
            cancellationToken.ThrowIfCancellationRequested();

            using var semBanner = new SemaphoreSlim(profile.BannerGrabConcurrency);
            var bannerTasks = new List<Task>();

            foreach (var port in device.OpenPorts)
            {
                await semBanner.WaitAsync(cancellationToken);
                bannerTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var banner = await _tcpBannerGrabService.GrabBannerAsync(ipAddress, port, profile.BannerGrabTimeout, cancellationToken);
                        var normalizedBanner = TcpBannerNormalizer.NormalizeBanner(banner);

                        if (!string.IsNullOrEmpty(normalizedBanner))
                        {
                            lock (device.PortBanners)
                                device.PortBanners[port] = normalizedBanner;
                        }
                    }
                    finally
                    {
                        semBanner.Release();
                    }
                }, cancellationToken));
            }


            // 6. Wait for all tasks to complete and finalize
            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch(OperationCanceledException)
            {
                // TO-DO: Handle cancellation if needed
            }

            return device;

        }
    }
}
