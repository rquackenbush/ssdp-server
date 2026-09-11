using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SsdpServer.Services;

public class SsdpService(ILogger<SsdpService> logger) : BackgroundService()
{
    private static readonly IPAddress LocalAddress = IPAddress.Parse("10.0.0.39"); //IPAddress.Any; //

    private readonly TimeSpan MinimumTimeBetweenSearches = TimeSpan.FromSeconds(30);

    private DateTimeOffset _lastSearch;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // ReuseAddress must be set before Bind, and other apps (e.g. Windows' own
        // "SSDP Discovery" service) commonly already hold port 1900 exclusively.
        using var udpClient = new UdpClient();

        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpClient.Client.Bind(new IPEndPoint(LocalAddress, SsdpEndpoints.Ipv4.Port));

        udpClient.JoinMulticastGroup(SsdpEndpoints.Ipv4.Address, LocalAddress);

        var searchPayload = File.ReadAllBytes("search.txt");

        while (!stoppingToken.IsCancellationRequested)
        {
            var timeSinceSearch = DateTimeOffset.UtcNow - _lastSearch;

            if (timeSinceSearch > MinimumTimeBetweenSearches)
            {

                logger.LogInformation("Sending search request....");
                await udpClient.SendAsync(searchPayload, SsdpEndpoints.Ipv4, stoppingToken);

                _lastSearch = DateTimeOffset.UtcNow;

            }

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, timeoutCts.Token);

            try
            {
                var result = await udpClient.ReceiveAsync(linkedCts.Token);

                if (result.RemoteEndPoint.Address.ToString() == LocalAddress.ToString())
                {
                    logger.LogInformation("request from self ignored");
                }
                else
                {
                    logger.LogInformation("From: {RemoteEndpoint}", result.RemoteEndPoint);

                    var text = Encoding.UTF8.GetString(result.Buffer);

                    if (string.IsNullOrEmpty(text))
                    {
                        logger.LogInformation("<EMPTY>");
                    }
                    else
                    {
                        logger.LogInformation("Message: {Text}", text);
                    }
                } 
            }
            catch (OperationCanceledException) 
            {
                logger.LogInformation("Timeout");
            }
        }

    }
}
