// ----------------------------------------------------------------------------
// <copyright file="LogMessage.cs" company="L0gg3r">
// Copyright (c) L0gg3r Project
// </copyright>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace L0gg3r.Base;

/// <summary>
/// A log message.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct LogMessage : IEquatable<LogMessage>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogMessage"/> struct.
    /// </summary>
    public LogMessage()
    {
        Timestamp = DateTime.Now;
        LogLevel = LogLevel.Info;
        Senders = Array.Empty<string>();
    }

    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// Gets the <see cref="LogLevel"/>.
    /// </summary>
    /// <seealso cref="LogLevel"/>
    public readonly LogLevel LogLevel { get; init; }

    /// <summary>
    /// Gets the payload.
    /// </summary>
    public readonly object? Payload { get; init; }

    /// <summary>
    /// Gets the list of sender.
    /// </summary>
    public IReadOnlyCollection<string> Senders { get; init; }

    public static bool operator ==(LogMessage left, LogMessage right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(LogMessage left, LogMessage right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj is LogMessage logMessage)
        {
            return Equals(logMessage);
        }

        return false;
    }

    /// <inheritdoc/>
    public bool Equals(LogMessage other) => GetHashCode() == other.GetHashCode();

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Timestamp, LogLevel, Payload, Senders);

    private string GetDebuggerDisplay() => $"LogLevel: {LogLevel}, Payload: {Payload}";
}
