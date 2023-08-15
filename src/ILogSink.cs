// ----------------------------------------------------------------------------
// <copyright file="ILogSink.cs" company="L0gg3r">
// Copyright (c) L0gg3r Project
// </copyright>
// ----------------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace L0gg3r.Base;

/// <summary>
/// A log sink.
/// </summary>
public interface ILogSink : IAsyncDisposable
{
    /// <summary>
    /// Processes the <paramref name="logMessage"/> asynchronously.
    /// </summary>
    /// <param name="logMessage">The log message to process.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ProcessAsync(in LogMessage logMessage);

    /// <summary>
    /// Flushes the log sink asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task FlushAsync();
}
