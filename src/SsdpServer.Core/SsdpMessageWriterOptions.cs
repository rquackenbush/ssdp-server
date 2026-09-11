namespace SsdpServer.Core;

public class SsdpMessageWriterOptions
{
    public const string DefaultNewLine = "\r\n";

    public string NewLine { get; set; } = DefaultNewLine;

    public string AfterColon { get; set; } = " ";
}
