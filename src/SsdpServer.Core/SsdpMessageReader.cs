using System.Text;

namespace SsdpServer.Core;

public static class SsdpMessageReader
{
    public static SsdpMessage Read(ReadOnlySpan<byte> payload)
    {
        var text = Encoding.UTF8.GetString(payload);

        return Read(text);
    }

    public static SsdpMessage Read(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
            throw new ArgumentException(nameof(payload));

        var lines = payload.Split("\r\n");

        var header = ParseHeader(lines[0]);

        var pairs = lines
            .Skip(1)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => ParseLine(l));

        return new SsdpMessage(header, pairs);
    }

    private static SsdpHeader ParseHeader(string value)
    {
        var split = value.Split(' ');

        if (split.Length != 3)
        {
            throw new InvalidOperationException($"The SSDP header didn't have 3 items: '{value}'");
        }

        return new SsdpHeader(split[0].Trim(), split[1].Trim(), split[2].Trim());
    }

    private static KeyValuePair<string, string> ParseLine(string line)
    {
        var firstColonIndex = line.IndexOf(":");

        if (firstColonIndex == -1)
            throw new InvalidOperationException($"Unable to find a colon in line '{line}'.");

        var attributeName = line.Substring(0, firstColonIndex).Trim();
        var attributeValue = line.Substring(firstColonIndex + 1).Trim();

        return new KeyValuePair<string, string>(attributeName, attributeValue);
    }
}
