// ----------------------------------------------------------------------------
// <copyright file="LogMessagePipeline.cs" company="L0gg3r">
// Copyright (c) L0gg3r Project
// </copyright>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace L0gg3r.Base;

/// <summary>
/// A pipeline for <see cref="LogMessage"/>s.
/// </summary>
public sealed class LogMessagePipeline : IAsyncDisposable
{
    private readonly BufferBlock<LogMessage> startBlock;

    private readonly BroadcastBlock<LogMessage> broadcastBlock;

    private bool isDiposed;

    private ImmutableList<Predicate<LogMessage>> filters = ImmutableList<Predicate<LogMessage>>.Empty;

    private Func<LogMessage, LogMessage> transform = logMessage => logMessage;

    private ImmutableList<ActionBlock<LogMessage>> actionBlocks = ImmutableList<ActionBlock<LogMessage>>.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogMessagePipeline"/> class.
    /// </summary>
    public LogMessagePipeline()
    {
        startBlock = new();

        TransformBlock<LogMessage, LogMessage> transformBlock = new(logMessage => Transform(logMessage));

        startBlock.LinkTo(transformBlock, new DataflowLinkOptions { PropagateCompletion = true }, logMessage => filters.All(filter => filter(logMessage)));
        startBlock.LinkTo(DataflowBlock.NullTarget<LogMessage>());

        broadcastBlock = new(logMessage => logMessage);

        transformBlock.LinkTo(broadcastBlock, new DataflowLinkOptions { PropagateCompletion = true });
    }

    /// <summary>
    /// Gets or sets a transform <see cref="Func{T, TResult}"/>.
    /// </summary>
    /// <remarks>
    /// The transform <see cref="Func{T, TResult}"/> receives a <see cref="LogMessage"/> and returns a <see cref="LogMessage"/>.
    /// The default value is a <see cref="Func{T, TResult}"/> that returns the <see cref="LogMessage"/> unchanged.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public Func<LogMessage, LogMessage> Transform
    {
        get => transform;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            transform = value;
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        isDiposed = true;

        startBlock.Complete();

        await Task.WhenAll(actionBlocks.Select(actionBlock => actionBlock.Completion)).ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Posts a <see cref="LogMessage"/> to the pipeline.
    /// </summary>
    /// <param name="logMessage">The <see cref="LogMessage"/> to post.</param>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    public void Post(LogMessage logMessage)
    {
        if (isDiposed)
        {
            throw new ObjectDisposedException(nameof(LogMessagePipeline));
        }

        startBlock.Post(logMessage);
    }

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
        if (isDiposed)
        {
            throw new ObjectDisposedException(nameof(LogMessagePipeline));
        }

        if (outputHandler is null)
        {
            throw new ArgumentNullException(nameof(outputHandler));
        }

        ActionBlock<LogMessage> actionBlock = new(outputHandler);

        IDisposable disposable = broadcastBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });

        actionBlocks = actionBlocks.Add(actionBlock);

        return disposable;
    }

    /// <summary>
    /// Adds a filter <see cref="Predicate{T}"/> to the <see cref="LogMessagePipeline"/>.
    /// </summary>
    /// <param name="filter">The filter <see cref="Predicate{T}"/> to add.</param>
    /// <returns>A <see cref="IDisposable"/> that removes the filter from the <see cref="LogMessagePipeline"/> on dispose.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="LogMessagePipeline"/> is disposed.</exception>
    public IDisposable AddFilter(Predicate<LogMessage> filter)
    {
        if (isDiposed)
        {
            throw new ObjectDisposedException(nameof(LogMessagePipeline));
        }

        if (filter is null)
        {
            throw new ArgumentNullException(nameof(filter));
        }

        filters = filters.Add(filter);

        return new FilterDisposer(filter, this);
    }

    /// <summary>
    /// Removes the filter <see cref="Predicate{T}"/> from the <see cref="LogMessagePipeline"/>.
    /// </summary>
    /// <param name="filter">The filter <see cref="Predicate{T}"/> to remove.</param>
    internal void RemoveFilter(Predicate<LogMessage> filter) => filters = filters.Remove(filter);
}
