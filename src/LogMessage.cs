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
public readonly record struct LogMessage : IEquatable<LogMessage>
{
    // ┌────────────────────────────────────────────────────────────────────────────────┐
    // │ Public Constructors                                                            │
    // └────────────────────────────────────────────────────────────────────────────────┘

    /// <summary>
    /// Initializes a new instance of the <see cref="LogMessage"/> class.
    /// </summary>
    public LogMessage()
    {
    }

    // ┌────────────────────────────────────────────────────────────────────────────────┐
    // │ Public Properties                                                              │
    // └────────────────────────────────────────────────────────────────────────────────┘

    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.Now;

    /// <summary>
    /// Gets the <see cref="LogLevel"/>.
    /// </summary>
    /// <seealso cref="LogLevel"/>
    public readonly LogLevel LogLevel { get; init; } = LogLevel.Info;

    /// <summary>
    /// Gets the payload.
    /// </summary>
    public readonly object? Payload { get; init; }

    /// <summary>
    /// Gets the list of sender.
    /// </summary>
    public IReadOnlyCollection<string> Senders { get; init; } = Array.Empty<string>();

    // ┌────────────────────────────────────────────────────────────────────────────────┐
    // │ Public Methods                                                                 │
    // └────────────────────────────────────────────────────────────────────────────────┘

    /// <inheritdoc/>
    public bool Equals(LogMessage other) => GetHashCode() == other.GetHashCode();

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Timestamp, LogLevel, Payload, Senders);

    // ┌────────────────────────────────────────────────────────────────────────────────┐
    // │ Private Methods                                                                │
    // └────────────────────────────────────────────────────────────────────────────────┘
    private string GetDebuggerDisplay() => $"LogLevel: {LogLevel}, Payload: {Payload}";
}
