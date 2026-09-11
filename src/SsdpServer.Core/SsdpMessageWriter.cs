using System.Text;

namespace SsdpServer.Core;

public static class SsdpMessageWriter
{
    public static void WriteToStream(this SsdpMessage message, Stream stream, SsdpMessageWriterOptions? options = null)
    {
        options = options ?? new SsdpMessageWriterOptions();

        using var writer = new StreamWriter(stream);

        writer.NewLine = options.NewLine;

        writer.WriteLine($"{message.Header.RequestMethod} {message.Header.RequestUri} {message.Header.RequestVersion}");

        foreach (var pair in message)
        {
            writer.WriteLine($"{pair.Key}:{options.AfterColon}{pair.Value}");
        }

        writer.WriteLine();
    }

    public static byte[] AsBytes(this SsdpMessage message, SsdpMessageWriterOptions? options = null)
    {
        using var stream = new MemoryStream();

        WriteToStream(message, stream);

        return stream.ToArray();
    }

    public static string AsString(this SsdpMessage message, SsdpMessageWriterOptions? options = null)
    {
        var buffer = AsBytes(message);

        return Encoding.UTF8.GetString(buffer);
    }
}
