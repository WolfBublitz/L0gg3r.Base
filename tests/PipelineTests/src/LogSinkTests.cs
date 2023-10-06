using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipelineTests.AttachOutputHandlerMethodTests;

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
[TestCategory("LogSinkTests")]
public class ThePipeline
{
    [TestMethod]
    public async Task ShallForwardALogMessageToAllLogSinks()
    {
        // arrange
        LogSink logSink1 = new();
        LogSink logSink2 = new();
        LogMessagePipeline logMessagePipeline = new()
        {
            logSink1,
            logSink2,
        };

        // act
        logMessagePipeline.Write(new LogMessage { LogLevel = LogLevel.Info });
        await logMessagePipeline.FlushAsync().ConfigureAwait(false);

        // assert
        logSink1.LogMessages.Should().HaveCount(1);
        logSink1.LogMessages[0].LogLevel.Should().Be(LogLevel.Info);
        logSink2.LogMessages.Should().HaveCount(1);
        logSink2.LogMessages[0].LogLevel.Should().Be(LogLevel.Info);
    }
}
