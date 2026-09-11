using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SsdpServer.Services;
using System.Net;
using System.Net.Sockets;

public static class Program
{
    private static readonly IPAddress LocalAddress = IPAddress.Any;

    private const int MulticastPort = 1900;

    private static readonly IPAddress MulticastAddress = new IPAddress(new byte[] { 239, 255,255,250 });

    private static readonly IPEndPoint MulticastEndpoint = new IPEndPoint(MulticastAddress, MulticastPort);

    public static async Task<int> Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddHostedService<SsdpService>();

        var app = builder.Build();

        await app.RunAsync();

        return 0;
    }
}



