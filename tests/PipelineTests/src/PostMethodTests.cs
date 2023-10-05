using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipelineTests.PostMethodTests;

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
public class ThePostMethod
{
    [TestMethod]
    public async Task ShouldPostLogMessageToPipeline()
    {
        // arrange
        LogMessage logMessage = new()
        {
            LogLevel = LogLevel.Info,
            Payload = "payload",
            Senders = new[] { "sender" }
        };
        LogSink logSink = new();
        LogMessagePipeline pipeline = new()
        {
            logSink
        };

        // act
        pipeline.Write(logMessage);
        await pipeline.FlushAsync().ConfigureAwait(false);

        // assert
        logSink.LogMessages[0].Should().BeEquivalentTo(logMessage);
    }
}
