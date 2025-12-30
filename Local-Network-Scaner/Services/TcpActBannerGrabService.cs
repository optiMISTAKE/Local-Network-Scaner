using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Local_Network_Scanner.Services
{
    public class TcpActBannerGrabService
    {
        public async Task<string?> GrabBannerAsync(string ipAddress, int port, int timeout, CancellationToken cancellationToken)
        {
            try
            {
                using var client = new TcpClient();

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(timeout);

                await client.ConnectAsync(ipAddress, port, cts.Token);

                using var stream = client.GetStream();
                stream.ReadTimeout = timeout;
                stream.WriteTimeout = timeout;

                // Some services may need a new line to respond
                byte[] request = Encoding.ASCII.GetBytes("\r\n");
                await stream.WriteAsync(request, 0, request.Length, cancellationToken);

                var buffer = new byte[1024]; // because banners are usually short
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                if (bytesRead <= 0) return null;

                return Encoding.ASCII.GetString(buffer, 0, bytesRead);
            }
            catch (OperationCanceledException)
            {
                throw; // Rethrow to let the caller handle cancellation
            }
            catch
            {
                return null; // benner grabbing often fails, so we return null on any exception
            }
        }
    }
}
