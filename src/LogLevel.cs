using System;
using System.Collections.Generic;

namespace L0gg3r.Base;


/// <summary>
/// The log level.
/// </summary>
public readonly struct LogLevel : IEquatable<LogLevel>
{
    private static int nextId;

    private readonly int id;

    private LogLevel(string name, string? description = null)
    {
        id = nextId++;

        Name = name;
        Description = description;
    }

    public static LogLevel Info  { get; } = new("Info");

    public static LogLevel Warning  { get; } = new("Warning");
    public static LogLevel Error { get; } = new("Error");

    public static IList<LogLevel> LogLevels { get; } = new List<LogLevel>()
    {
        Info, Warning, Error,
    };

    public string Name { get; }

    public string? Description { get; }

    public static bool operator==(LogLevel left, LogLevel right) => left.id == right.id;

    public static bool operator!=(LogLevel left, LogLevel right) => !(left.id == right.id);

    public static bool operator<(LogLevel left, LogLevel right) => LogLevels.IndexOf(left) < LogLevels.IndexOf(right);

    public static bool operator<=(LogLevel left, LogLevel right) => left == right || left < right;

    public static bool operator>(LogLevel left, LogLevel right) => LogLevels.IndexOf(left) > LogLevels.IndexOf(right);

    public static bool operator>=(LogLevel left, LogLevel right) => left == right || left > right;

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
}