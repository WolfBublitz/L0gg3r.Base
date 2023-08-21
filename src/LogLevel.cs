// ----------------------------------------------------------------------------
// <copyright file="LogLevel.cs" company="L0gg3r">
// Copyright (c) L0gg3r Project
// </copyright>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace L0gg3r.Base;

/// <summary>
/// The log level.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly record struct LogLevel : IEquatable<LogLevel>, IComparable, IComparable<LogLevel>
{
    private static Lazy<List<LogLevel>> order = new(() => new List<LogLevel>
    {
        Info,
        Warning,
        Error,
    });

    private static int nextId;

    private readonly int id;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogLevel"/> class.
    /// </summary>
    /// <param name="name">The name of the <see cref="LogLevel"/>.</param>
    /// <param name="description">An optional description.</param>
    internal LogLevel(string name, string? description = null)
    {
        id = nextId++;

        Name = name;
        Description = description;
    }

    /// <summary>
    /// Gets the <see cref="LogLevel.Info"/> log level.
    /// </summary>
    public static LogLevel Info { get; } = new("Info");

    /// <summary>
    /// Gets the <see cref="LogLevel.Warning"/> log level.
    /// </summary>
    public static LogLevel Warning { get; } = new("Warning");

    /// <summary>
    /// Gets the <see cref="LogLevel.Error"/> log level.
    /// </summary>
    public static LogLevel Error { get; } = new("Error");

    /// <summary>
    /// Gets the order of the <see cref="LogLevel"/>s.
    /// </summary>
    /// <remarks>
    /// The order of the <see cref="LogLevel"/>s is a list of <see cref="LogLevel"/>s ordered by their severity. The
    /// default order is <see cref="Info"/>, <see cref="Warning"/>, <see cref="Error"/>.
    /// </remarks>
    /// <seealso cref="InsertLogLevelBefore(LogLevel, string, string)"/>
    /// <seealso cref="InsertLogLevelAfter(LogLevel, string, string)"/>
    /// <seealso cref="ResetOrder"/>
    public static IReadOnlyList<LogLevel> Order => order.Value;

    /// <summary>
    /// Gets the name of the <see cref="LogLevel"/>.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the description of the <see cref="LogLevel"/>.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="left"/> is less than <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The <see cref="LogLevel"/> on the left side.</param>
    /// <param name="right">The <see cref="LogLevel"/>on the right side.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
    public static bool operator <(LogLevel left, LogLevel right)
    {
        return order.Value.IndexOf(left) < order.Value.IndexOf(right);
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="left"/> is less or equal than <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The <see cref="LogLevel"/> on the left side.</param>
    /// <param name="right">The <see cref="LogLevel"/>on the right side.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is less or equal than <paramref name="right"/>.</returns>
    public static bool operator <=(LogLevel left, LogLevel right)
    {
        return left == right || left < right;
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="left"/> is greater than <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The <see cref="LogLevel"/> on the left side.</param>
    /// <param name="right">The <see cref="LogLevel"/>on the right side.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
    public static bool operator >(LogLevel left, LogLevel right)
    {
        return order.Value.IndexOf(left) > order.Value.IndexOf(right);
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="left"/> is greater or equal than <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The <see cref="LogLevel"/> on the left side.</param>
    /// <param name="right">The <see cref="LogLevel"/>on the right side.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is greater or equal than <paramref name="right"/>.</returns>
    public static bool operator >=(LogLevel left, LogLevel right)
    {
        return left == right || left > right;
    }

    /// <summary>
    /// Inserts a new <see cref="LogLevel"/> with <paramref name="name"/> and <paramref name="description"/> before <paramref name="logLevel"/>
    /// in the <see cref="Order"/> of <see cref="LogLevel"/>s.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/> before which the new <see cref="LogLevel"/> shall be inserted.</param>
    /// <param name="name">The name of the new <see cref="LogLevel"/>.</param>
    /// <param name="description">The optional description of the new <see cref="LogLevel"/>.</param>
    /// <returns>The newly created <see cref="LogLevel"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when a same-named <see cref="LogLevel"/> already exists in the <see cref="Order"/> of <see cref="LogLevel"/>s or
    /// when the <see cref="Order"/> of <see cref="LogLevel"/> does not contain <paramref name="logLevel"/>.</exception>
    public static LogLevel InsertLogLevelBefore(LogLevel logLevel, string name, string description = "")
    {
        if (order.Value.Any(ll => ll.Name == name))
        {
            throw new ArgumentException($"The log level {logLevel.Name} already exists.");
        }

        if (!order.Value.Contains(logLevel))
        {
            throw new ArgumentException($"The log level {logLevel.Name} does not exist.");
        }

        LogLevel newLogLevel = new(name, description);

        order.Value.Insert(order.Value.IndexOf(logLevel), newLogLevel);

        return newLogLevel;
    }

    /// <summary>
    /// Inserts a new <see cref="LogLevel"/> with <paramref name="name"/> and <paramref name="description"/> after <paramref name="logLevel"/>
    /// in the <see cref="Order"/> of <see cref="LogLevel"/>s.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/> after which the new <see cref="LogLevel"/> shall be inserted.</param>
    /// <param name="name">The name of the new <see cref="LogLevel"/>.</param>
    /// <param name="description">The optional description of the new <see cref="LogLevel"/>.</param>
    /// <returns>The newly created <see cref="LogLevel"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when a same-named <see cref="LogLevel"/> already exists in the <see cref="Order"/> of <see cref="LogLevel"/>s or
    /// when the <see cref="Order"/> of <see cref="LogLevel"/>s does not contain <paramref name="logLevel"/>.</exception>
    public static LogLevel InsertLogLevelAfter(LogLevel logLevel, string name, string description = "")
    {
        if (order.Value.Any(ll => ll.Name == name))
        {
            throw new ArgumentException($"The log level {logLevel.Name} already exists.");
        }

        if (!order.Value.Contains(logLevel))
        {
            throw new ArgumentException($"The log level {logLevel.Name} does not exist.");
        }

        LogLevel newLogLevel = new(name, description);

        order.Value.Insert(order.Value.IndexOf(logLevel) + 1, newLogLevel);

        return newLogLevel;
    }

    /// <summary>
    /// Resets the <see cref="Order"/> of <see cref="LogLevel"/>s to the default order.
    /// </summary>
    /// <seealso cref="Order"/>
    public static void ResetOrder()
    {
        order = new Lazy<List<LogLevel>>(() => new List<LogLevel>
        {
            Info,
            Warning,
            Error,
        });
    }

    /// <inheritdoc/>
    public bool Equals(LogLevel other) => this == other;

    /// <inheritdoc/>
    public int CompareTo(object? obj)
    {
        if (obj is LogLevel logLevel)
        {
            return CompareTo(logLevel);
        }
        else
        {
            throw new ArgumentException($"Object must be of type {nameof(LogLevel)}");
        }
    }

    /// <inheritdoc/>
    public int CompareTo(LogLevel other)
    {
        if (this == other)
        {
            return 0;
        }

        if (this < other)
        {
            return -1;
        }

        return 1;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => id.GetHashCode() + Name.GetHashCode(StringComparison.InvariantCulture);

    /// <inheritdoc/>
    public override string ToString() => Name;

    private string GetDebuggerDisplay() => $"LogLevel: {Name}";
}
