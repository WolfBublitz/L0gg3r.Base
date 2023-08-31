// ----------------------------------------------------------------------------
// <copyright file="OutputHandlerDisposer.cs" company="L0gg3r">
// Copyright (c) L0gg3r Project
// </copyright>
// ----------------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace L0gg3r.Base;

/// <summary>
/// Disposes a output handler.
/// </summary>
internal sealed class OutputHandlerDisposer : IDisposable
{
    private readonly Func<LogMessage, Task> outputHandler;

    private readonly LogMessagePipeline logMessagePipeline;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutputHandlerDisposer"/> class.
    /// </summary>
    /// <param name="outputHandler">The output handler to dispose.</param>
    /// <param name="logMessagePipeline">The <see cref="LogMessagePipeline"/> the <paramref name="outputHandler"/> is attached to.</param>
    public OutputHandlerDisposer(Func<LogMessage, Task> outputHandler, LogMessagePipeline logMessagePipeline)
    {
        this.outputHandler = outputHandler;
        this.logMessagePipeline = logMessagePipeline;
    }

    /// <inheritdoc/>
    public void Dispose() => logMessagePipeline.RemoveOutputHandler(outputHandler);
}
