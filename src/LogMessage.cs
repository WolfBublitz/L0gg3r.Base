using System.Collections.Generic;

namespace L0gg3r.Base;

/// <summary>
/// The log message.
/// </summary>
public readonly struct LogMessage
{
    private readonly List<string> senders;

    public LogMessage(LogLevel logLevel, object payload, string sender)
    {
        LogLevel = logLevel;
        Payload = payload;

        senders = new List<string> { sender };
    }

    /// <summary>
    /// Gets the <see cref="LogLevel"/>.
    /// </summary>
    /// <seealso cref="LogLevel"/>
    public LogLevel LogLevel { get; }

    /// <summary>
    /// Gets the payload.
    /// </summary>
    public object Payload { get; }

    /// <summary>
    /// Gets the list of sender.
    /// </summary>
    public IEnumerable<string> Senders => senders;

    internal void AddSender(string sender) => senders.Add(sender);
}
