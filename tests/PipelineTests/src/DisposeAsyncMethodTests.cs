using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipelineTests.DisposeAsyncMethodTests;

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
public class TheDisposeAsyncMethod
{
    [TestMethod]
    public async Task ShallFlushThePipeline()
    {
        // arrange
        LogSink logSink = new();
        LogMessagePipeline logMessagePipeline = new()
        {
            logSink
        };

        // act
        logMessagePipeline.Write(new LogMessage { LogLevel = LogLevel.Info, Payload = 1 });
        logMessagePipeline.Write(new LogMessage { LogLevel = LogLevel.Info, Payload = 2 });
        logMessagePipeline.Write(new LogMessage { LogLevel = LogLevel.Info, Payload = 3 });
        await logMessagePipeline.DisposeAsync().ConfigureAwait(false);

        // assert
        logSink.LogMessages.Count.Should().Be(3);
        logSink.LogMessages[0].Payload.Should().Be(1);
        logSink.LogMessages[1].Payload.Should().Be(2);
        logSink.LogMessages[2].Payload.Should().Be(3);
    }
}
