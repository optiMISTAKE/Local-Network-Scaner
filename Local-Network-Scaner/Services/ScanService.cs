using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Local_Network_Scaner.Model;

namespace Local_Network_Scaner.Services
{
    public class ScanService
    {
        public async Task<List<DeviceInfo>> ScanSubnetAsync(string baseIp)
        {
            var tasks = new List<Task<DeviceInfo>>();

            for (int i = 1; i <= 254; i++)
            {
                string ipAddress = $"{baseIp}.{i}";
                Debug.WriteLine($"Scanning {ipAddress}");
                tasks.Add(ScanSingleHost(ipAddress));
            }

            return (await Task.WhenAll(tasks)).Where(d => d != null && d.IsActive).ToList();

        }

        public async Task<DeviceInfo> ScanSingleHost(string ipAddress)
        {
            var device = new DeviceInfo { IPAddress = ipAddress };

            // 1. Ping the host
            device.IsActive = await new SimplePingService().PingAsync(ipAddress, 300);

            if (!device.IsActive)
            {
                device.IsActive = await new SimplePingService().PingAsync(ipAddress, 1000);
            }

            // ...

            return device;
        }
    }
}
