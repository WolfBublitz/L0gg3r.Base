// ----------------------------------------------------------------------------
// <copyright file="LogMessagePipeline.cs" company="L0gg3r">
// Copyright (c) L0gg3r Project
// </copyright>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace L0gg3r.Base;

/// <summary>
/// A pipeline for <see cref="LogMessage"/>s.
/// </summary>
public sealed class LogMessagePipeline : IAsyncDisposable
{
    private Channel<LogMessage> channel = Channel.CreateUnbounded<LogMessage>();

    private Task task;

    private Func<LogMessage, LogMessage>? transform;

    private ImmutableArray<Predicate<LogMessage>> filters = ImmutableArray<Predicate<LogMessage>>.Empty;

    private ImmutableArray<Func<LogMessage, Task>> outputHandlers = ImmutableArray<Func<LogMessage, Task>>.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogMessagePipeline"/> class.
    /// </summary>
    public LogMessagePipeline()
    {
        task = StartAsync();
    }

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

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        IsDisposed = true;

        channel.Writer.Complete();

        await task.ConfigureAwait(false);
    }

    /// <summary>
    /// Posts a <see cref="LogMessage"/> to the pipeline.
    /// </summary>
    /// <param name="logMessage">The <see cref="LogMessage"/> to post.</param>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    public void Write(LogMessage logMessage) => channel.Writer.TryWrite(logMessage);

    /// <summary>
    /// Attaches a <see cref="Func{T, TResult}"/> output handler to the pipeline.
    /// </summary>
    /// <remarks>
    /// The <paramref name="outputHandler"/> is invoked for each <see cref="LogMessage"/> that passes the pipeline.
    /// </remarks>
    /// <param name="outputHandler">The output handler to attach.</param>
    /// <returns>An <see cref="IDisposable"/> that detaches the <paramref name="outputHandler"/> on dispose.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    public IDisposable AttachOutputHandler(Func<LogMessage, Task> outputHandler)
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(LogMessagePipeline));
        }

        if (outputHandler is null)
        {
            throw new ArgumentNullException(nameof(outputHandler));
        }

        Flush();

        outputHandlers = outputHandlers.Add(outputHandler);

        return new OutputHandlerDisposer(outputHandler, this);
    }

    /// <summary>
    /// Adds a filter <see cref="Predicate{T}"/> to the <see cref="LogMessagePipeline"/>.
    /// </summary>
    /// <param name="filter">The filter <see cref="Predicate{T}"/> to add.</param>
    /// <returns>A <see cref="IDisposable"/> that removes the filter from the <see cref="LogMessagePipeline"/> on dispose.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    public IDisposable AddFilter(Predicate<LogMessage> filter)
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(LogMessagePipeline));
        }

        if (filter is null)
        {
            throw new ArgumentNullException(nameof(filter));
        }

        Flush();

        filters = filters.Add(filter);

        return new FilterDisposer(filter, this);
    }

    /// <summary>
    /// Flushes the <see cref="LogMessagePipeline"/> asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task FlushAsync()
    {
        await StopAsync().ConfigureAwait(false);

        task = StartAsync();
    }

    /// <summary>
    /// Flushes the <see cref="LogMessagePipeline"/> synchronously.
    /// </summary>
    public void Flush() => FlushAsync().GetAwaiter().GetResult();

    /// <summary>
    /// Removes the <see cref="Func{T, TResult}"/> output handler from the pipeline.
    /// </summary>
    /// <param name="outputHandler">The output handler to remove.</param>
    internal void RemoveOutputHandler(Func<LogMessage, Task> outputHandler)
    {
        Flush();

        outputHandlers = outputHandlers.Remove(outputHandler);
    }

    /// <summary>
    /// Removes the filter <see cref="Predicate{T}"/> from the <see cref="LogMessagePipeline"/>.
    /// </summary>
    /// <param name="filter">The filter <see cref="Predicate{T}"/> to remove.</param>
    internal void RemoveFilter(Predicate<LogMessage> filter)
    {
        Flush();

        filters = filters.Remove(filter);
    }

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

                await Task.WhenAll(outputHandlers.Select(outputHandler => outputHandler(logMessage))).ConfigureAwait(false);
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
