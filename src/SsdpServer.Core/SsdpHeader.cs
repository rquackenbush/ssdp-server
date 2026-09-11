namespace SsdpServer.Core;

public record class SsdpHeader(string RequestMethod, string RequestUri, string RequestVersion)
{
    public override string ToString()
    {
        return $"{RequestMethod} {RequestUri} {RequestVersion}";
    }
}
