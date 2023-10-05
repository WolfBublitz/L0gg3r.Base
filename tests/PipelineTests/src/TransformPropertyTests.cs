using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipelineTests.TransformPropertyTests;

internal sealed class LogSink : ILogSink
{
    public List<LogMessage> LogMessages { get; } = new();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public Task FlushAsync() => Task.CompletedTask;

    public Task ProcessAsync(in LogMessage logMessage)
    {
        LogMessages.Add(logMessage);

        return Task.CompletedTask;
    }
}

[TestClass]
[TestCategory("PipelineTests")]
public class TheTransformProperty
{
    [TestMethod]
    public async Task ShallEnableATransformation()
    {
        // arrange
        LogSink logSink = new();
        LogMessagePipeline logMessagePipeline = new()
        {
            Transform = logMessage =>
            {
                return new()
                {
                    LogLevel = logMessage.LogLevel,
                    Payload = "transformed payload",
                    Senders = logMessage.Senders
                };
            },
        };
        logMessagePipeline.AddLogSink(logSink);

        // act
        logMessagePipeline.Write(new LogMessage { LogLevel = LogLevel.Info });
        await logMessagePipeline.FlushAsync().ConfigureAwait(false);

        // assert
        logSink.LogMessages.First().Payload.Should().Be("transformed payload");
    }
}
