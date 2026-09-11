using System.Net;

namespace SsdpServer;

public static class SsdpEndpoints
{
    public static IPEndPoint Ipv4 = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);
}
