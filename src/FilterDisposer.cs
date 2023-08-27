// ----------------------------------------------------------------------------
// <copyright file="FilterDisposer.cs" company="L0gg3r">
// Copyright (c) L0gg3r Project
// </copyright>
// ----------------------------------------------------------------------------

using System;

namespace L0gg3r.Base;

/// <summary>
/// Disposes a filter <see cref="Predicate{T}"/>.
/// </summary>
internal sealed class FilterDisposer : IDisposable
{
    private readonly Predicate<LogMessage> filter;

    private readonly LogMessagePipeline logMessagePipeline;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterDisposer"/> class.
    /// </summary>
    /// <param name="filter">The filter <see cref="Predicate{T}"/> that shall be removed on dispose.</param>
    /// <param name="logMessagePipeline">The <see cref="LogMessagePipeline"/> the <paramref name="filter"/> is attached to.</param>
    public FilterDisposer(Predicate<LogMessage> filter, LogMessagePipeline logMessagePipeline)
    {
        this.filter = filter;
        this.logMessagePipeline = logMessagePipeline;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        logMessagePipeline.RemoveFilter(filter);
    }
}
