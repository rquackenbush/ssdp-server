using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SsdpServer.Core;

public class SsdpMessage : IReadOnlyDictionary<string, string>
{
    private readonly ImmutableList<KeyValuePair<string, string>> _attributes;
    private readonly ImmutableDictionary<string, string> _dictionary;

    public SsdpMessage(SsdpHeader header, IEnumerable<KeyValuePair<string, string>> attributes)
    {
        _attributes = attributes.ToImmutableList();
        _dictionary = attributes.ToImmutableDictionary<string, string>();
        Header = header;
    }

    public string this[string key] => _dictionary[key];

    public IEnumerable<string> Keys => _dictionary.Keys;

    public IEnumerable<string> Values => _dictionary.Values;

    public int Count => _dictionary.Count;

    public SsdpHeader Header { get; }

    public bool ContainsKey(string key)
    {
        return _dictionary.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return _attributes.GetEnumerator();
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _attributes.GetEnumerator();
    }

    public override string ToString()
    {
        return this.AsString();
    }
}
