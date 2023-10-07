// ----------------------------------------------------------------------------
// <copyright file="LogMessagePipeline.cs" company="L0gg3r">
// Copyright (c) L0gg3r Project
// </copyright>
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace L0gg3r.Base;

/// <summary>
/// A pipeline for <see cref="LogMessage"/>s.
/// </summary>
public sealed class LogMessagePipeline : IAsyncDisposable, IEnumerable<ILogSink>, IEnumerable<Predicate<LogMessage>>
{
    // ┌────────────────────────────────────────────────────────────────────────────────┐
    // │ Private Fields                                                                 │
    // └────────────────────────────────────────────────────────────────────────────────┘
    private Channel<LogMessage> channel = Channel.CreateUnbounded<LogMessage>();

    private Task task;

    private Func<LogMessage, LogMessage>? transform;

    private ImmutableArray<Predicate<LogMessage>> filters = ImmutableArray<Predicate<LogMessage>>.Empty;

    private ImmutableArray<ILogSink> logSinks = ImmutableArray<ILogSink>.Empty;

    // ┌────────────────────────────────────────────────────────────────────────────────┐
    // │ Public Constructors                                                            │
    // └────────────────────────────────────────────────────────────────────────────────┘

    /// <summary>
    /// Initializes a new instance of the <see cref="LogMessagePipeline"/> class.
    /// </summary>
    public LogMessagePipeline()
    {
        task = StartAsync();
    }

    // ┌────────────────────────────────────────────────────────────────────────────────┐
    // │ Public Properties                                                              │
    // └────────────────────────────────────────────────────────────────────────────────┘

    /// <summary>
    /// Gets or sets a transform <see cref="Func{T, TResult}"/>.
    /// </summary>
    /// <remarks>
    /// The transform <see cref="Func{T, TResult}"/> receives a <see cref="LogMessage"/> and returns a <see cref="LogMessage"/>.
    /// The default value is a <see cref="Func{T, TResult}"/> that returns the <see cref="LogMessage"/> unchanged.
    /// </remarks>
    public Func<LogMessage, LogMessage>? Transform
    {
        get => transform;
        set
        {
            Flush();

            transform = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="LogMessagePipeline"/> is disposed (<c>true</c>) or not (<c>false</c>).
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Gets the collection of <see cref="ILogSink"/>s that are registered with the pipeline.
    /// </summary>
    public IReadOnlyCollection<ILogSink> LogSinks => logSinks;

    /// <summary>
    /// Gets the collection of filters that are registered with the pipeline.
    /// </summary>
    public IReadOnlyCollection<Predicate<LogMessage>> Filters => filters;

    // ┌────────────────────────────────────────────────────────────────────────────────┐
    // │ Public Methods                                                                 │
    // └────────────────────────────────────────────────────────────────────────────────┘

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    public async ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        IsDisposed = true;

        channel.Writer.Complete();

        await task.ConfigureAwait(false);
    }

    /// <summary>
    /// Posts a <see cref="LogMessage"/> to the pipeline.
    /// </summary>
    /// <param name="logMessage">The <see cref="LogMessage"/> to post.</param>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    /// <returns>The <see cref="LogMessagePipeline"/> for chaining.</returns>
    public LogMessagePipeline Write(LogMessage logMessage)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        channel.Writer.TryWrite(logMessage);

        return this;
    }

    /// <summary>
    /// Attaches a <see cref="Func{T, TResult}"/> output handler to the pipeline.
    /// </summary>
    /// <remarks>
    /// The <paramref name="logSink"/> is invoked for each <see cref="LogMessage"/> that passes the pipeline.
    /// </remarks>
    /// <param name="logSink">The output handler to attach.</param>
    /// <returns>An <see cref="IDisposable"/> that detaches the <paramref name="logSink"/> on dispose.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    public IDisposable AddLogSink(ILogSink logSink)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        if (logSink is null)
        {
            throw new ArgumentNullException(nameof(logSink));
        }

        Flush();

        logSinks = logSinks.Add(logSink);

        return new Disposable(() => RemoveLogSink(logSink));
    }

    /// <summary>
    /// Adds a new log sink to the pipeline.
    /// </summary>
    /// <param name="logSink">The log sink to add.</param>
    /// <returns>An <see cref="IDisposable"/> object that can be used to remove the log sink from the pipeline.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    public IDisposable Add(ILogSink logSink)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        return AddLogSink(logSink);
    }

    /// <summary>
    /// Removes the <see cref="Func{T, TResult}"/> output handler from the pipeline.
    /// </summary>
    /// <param name="logSink">The output handler to remove.</param>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    public void RemoveLogSink(ILogSink logSink)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        Flush();

        logSinks = logSinks.Remove(logSink);
    }

    /// <summary>
    /// Adds a filter <see cref="Predicate{T}"/> to the <see cref="LogMessagePipeline"/>.
    /// </summary>
    /// <param name="filter">The filter <see cref="Predicate{T}"/> to add.</param>
    /// <returns>A <see cref="IDisposable"/> that removes the filter from the <see cref="LogMessagePipeline"/> on dispose.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    public IDisposable AddFilter(Predicate<LogMessage> filter)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        if (filter is null)
        {
            throw new ArgumentNullException(nameof(filter));
        }

        Flush();

        filters = filters.Add(filter);

        return new Disposable(() => RemoveFilter(filter));
    }

    /// <summary>
    /// Removes the filter <see cref="Predicate{T}"/> from the <see cref="LogMessagePipeline"/>.
    /// </summary>
    /// <param name="filter">The filter <see cref="Predicate{T}"/> to remove.</param>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    public void RemoveFilter(Predicate<LogMessage> filter)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        Flush();

        filters = filters.Remove(filter);
    }

    /// <summary>
    /// Adds a filter to the log message pipeline.
    /// </summary>
    /// <param name="filter">The filter to add.</param>
    /// <returns>An <see cref="IDisposable"/> object that can be used to remove the filter from the pipeline.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    public IDisposable Add(Predicate<LogMessage> filter) => AddFilter(filter);

    /// <summary>
    /// Flushes the <see cref="LogMessagePipeline"/> asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    public async Task FlushAsync()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        await StopAsync().ConfigureAwait(false);

        await Task.WhenAll(logSinks.Select(logSink => logSink.FlushAsync())).ConfigureAwait(false);

        task = StartAsync();
    }

    /// <summary>
    /// Flushes the <see cref="LogMessagePipeline"/> synchronously.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    public void Flush() => FlushAsync().GetAwaiter().GetResult();

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    IEnumerator<ILogSink> IEnumerable<ILogSink>.GetEnumerator()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        foreach (ILogSink logSink in logSinks)
        {
            yield return logSink;
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    IEnumerator<Predicate<LogMessage>> IEnumerable<Predicate<LogMessage>>.GetEnumerator()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        foreach (Predicate<LogMessage> filter in filters)
        {
            yield return filter;
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    IEnumerator IEnumerable.GetEnumerator()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        foreach (ILogSink logSink in logSinks)
        {
            yield return logSink;
        }

        foreach (Predicate<LogMessage> filter in filters)
        {
            yield return filter;
        }
    }

    // ┌────────────────────────────────────────────────────────────────────────────────┐
    // │ Private Methods                                                                │
    // └────────────────────────────────────────────────────────────────────────────────┘
    private async Task StartAsync()
    {
        ChannelReader<LogMessage> reader = channel.Reader;

        try
        {
            while (true)
            {
                LogMessage logMessage = await reader.ReadAsync().ConfigureAwait(false);

                if (!filters.All(filter => filter(logMessage)))
                {
                    continue;
                }

                if (Transform is not null)
                {
                    logMessage = Transform(logMessage);
                }

                await Task.WhenAll(logSinks.Select(logSink => logSink.ProcessAsync(logMessage))).ConfigureAwait(false);
            }
        }
        catch (ChannelClosedException)
        {
        }
    }

    private Task StopAsync()
    {
        Channel<LogMessage> oldChannel = Interlocked.Exchange(ref channel, Channel.CreateUnbounded<LogMessage>());

        oldChannel.Writer.Complete();

        return task;
    }
}
