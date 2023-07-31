using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace L0gg3r.Base;


/// <summary>
/// The log level.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct LogLevel : IEquatable<LogLevel>
{
    private static int nextId;

    private readonly int id;

    private static Lazy<List<LogLevel>> logLevels = new(() => new List<LogLevel>
    {
        Info,
        Warning,
        Error,
    });

    internal LogLevel(string name, string? description = null)
    {
        id = nextId++;

        Name = name;
        Description = description;
    }

    public static LogLevel Info { get; } = new("Info");

    public static LogLevel Warning { get; } = new("Warning");

    public static LogLevel Error { get; } = new("Error");

    public static IReadOnlyList<LogLevel> LogLevels => logLevels.Value;

    public string Name { get; }

    public string? Description { get; }

    public static bool operator ==(LogLevel left, LogLevel right) => left.id == right.id;

    public static bool operator !=(LogLevel left, LogLevel right) => !(left.id == right.id);

    public static bool operator <(LogLevel left, LogLevel right) => logLevels.Value.IndexOf(left) < logLevels.Value.IndexOf(right);

    public static bool operator <=(LogLevel left, LogLevel right) => left == right || left < right;

    public static bool operator >(LogLevel left, LogLevel right) => logLevels.Value.IndexOf(left) > logLevels.Value.IndexOf(right);

    public static bool operator >=(LogLevel left, LogLevel right) => left == right || left > right;

    public static LogLevel InsertLogLevelBefore(LogLevel logLevel, string name, string description = "")
    {
        if (logLevels.Value.Any(ll => ll.Name == name))
        {
            throw new ArgumentException($"The log level {logLevel.Name} already exists.");
        }

        if (!logLevels.Value.Contains(logLevel))
        {
            throw new ArgumentException($"The log level {logLevel.Name} does not exist.");
        }

        LogLevel newLogLevel = new(name, description);

        logLevels.Value.Insert(logLevels.Value.IndexOf(logLevel), newLogLevel);

        return newLogLevel;
    }

    public static LogLevel InsertLogLevelAfter(LogLevel logLevel, string name, string description = "")
    {
        if (logLevels.Value.Any(ll => ll.Name == name))
        {
            throw new ArgumentException($"The log level {logLevel.Name} already exists.");
        }

        if (!logLevels.Value.Contains(logLevel))
        {
            throw new ArgumentException($"The log level {logLevel.Name} does not exist.");
        }

        LogLevel newLogLevel = new(name, description);

        logLevels.Value.Insert(logLevels.Value.IndexOf(logLevel) + 1, newLogLevel);

        return newLogLevel;
    }

    public static void ResetLogLevels()
    {
        logLevels = new Lazy<List<LogLevel>>(() => new List<LogLevel>
        {
            Info,
            Warning,
            Error,
        });
    }

    public override bool Equals(object? other)
    {
        if (other is LogLevel logLevel)
        {
            return Equals(logLevel);
        }

        return false;
    }

    public bool Equals(LogLevel logLevel) => this == logLevel;

    public override int GetHashCode() => id.GetHashCode() + Name.GetHashCode();

    private string GetDebuggerDisplay() => $"LogLevel: {Name}";
}
