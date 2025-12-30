using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Local_Network_Scanner.Services
{
    public class SimplePingService
    {
        public async Task<bool> PingAsync(string ipAddress, int timeout, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var ping = new Ping();

            try
            {
                PingReply reply = await ping.SendPingAsync(ipAddress, timeout);
                Debug.WriteLine($"Ping to {ipAddress} - Status: {reply.Status}");
                return reply.Status == IPStatus.Success;
            }
            catch
            {
                // In case the ping fails (e.g., invalid IP address), we consider it unreachable.
                return false;
            }
        }
    }
}
