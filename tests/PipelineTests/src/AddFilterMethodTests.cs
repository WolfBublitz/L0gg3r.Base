using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipelineTests.AddFilterMethodTests;

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
public class TheAddFilterMethod
{
    [TestMethod]
    public async Task ShallAddAFilter()
    {
        // arrange
        LogSink logSink = new();
        LogMessagePipeline logMessagePipeline = new()
        {
            logSink,
        };

        // act
        logMessagePipeline.Write(new LogMessage { Payload = 1 });
        await logMessagePipeline.FlushAsync().ConfigureAwait(false);
        LogMessage logMessage1 = logSink.LogMessages[0];

        // assert
        logMessage1.Payload.Should().Be(1);

        // act
        using (IDisposable disposable = logMessagePipeline.AddFilter(logMessage => logMessage.Payload is int number && number == 2))
        {
            logMessagePipeline.Write(new LogMessage { Payload = 1 });
            logMessagePipeline.Write(new LogMessage { Payload = 2 });
        }

        LogMessage logMessage2 = logSink.LogMessages[1];

        // assert
        logMessage2.Payload.Should().Be(2, because: "the filter should have removed the log message with the payload 1");
    }

    [TestMethod]
    public async Task ShallReturnADisposableThatRemovesTheFilter()
    {
        // arrange
        LogSink logSink = new();
        LogMessagePipeline logMessagePipeline = new()
        {
            logSink
        };

        // act
        using (IDisposable disposable = logMessagePipeline.AddFilter(logMessage => logMessage.Payload is int number && number == 2))
        {
            logMessagePipeline.Write(new LogMessage { Payload = 1 });
            logMessagePipeline.Write(new LogMessage { Payload = 2 });
            await logMessagePipeline.FlushAsync().ConfigureAwait(false);

            // assert
            logSink.LogMessages[0].Payload.Should().Be(2, because: "the filter should have removed the log message with the payload 1");
        }

        // act
        await logMessagePipeline.Write(new LogMessage { Payload = 1 }).FlushAsync().ConfigureAwait(false);

        LogMessage logMessage2 = logSink.LogMessages[1];

        // assert
        logMessage2.Payload.Should().Be(1, because: "the filter should have been removed");
    }

    [TestMethod]
    public void ShallThrowArgumentNullException()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();

        // act
        Action action = () => logMessagePipeline.AddFilter(null!);

        // assert
        action.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public async Task ShallThrowObjectDisposedException()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();
        await logMessagePipeline.DisposeAsync().ConfigureAwait(false);

        // act
        Action action = () => logMessagePipeline.AddFilter(logMessage => true);

        // assert
        action.Should().Throw<ObjectDisposedException>();
    }
}
