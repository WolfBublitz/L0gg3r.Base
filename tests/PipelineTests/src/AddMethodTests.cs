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
public class TheAddMethod
{
    [TestMethod]
    public async Task ShallAddALogSink()
    {
        // arrange
        LogSink logSink = new();
        LogMessagePipeline logMessagePipeline = new();

        // act
        logMessagePipeline.AddLogSink(logSink);
        logMessagePipeline.Write(new LogMessage { LogLevel = LogLevel.Info });
        await logMessagePipeline.FlushAsync().ConfigureAwait(false);

        // assert
        logSink.LogMessages.Should().HaveCount(1);
        logSink.LogMessages[0].LogLevel.Should().Be(LogLevel.Info);
    }

    [TestMethod]
    public void ShallThrowArgumentNullException()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();

        // act
        Action action = () => logMessagePipeline.AddLogSink(null!);

        // assert
        action.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public async Task ShallThrowObjectDisposedException()
    {
        // arrange
        LogSink logSink = new();
        LogMessagePipeline logMessagePipeline = new();
        await logMessagePipeline.DisposeAsync().ConfigureAwait(false);

        // act
        Action action = () => logMessagePipeline.AddLogSink(logSink);

        // assert
        action.Should().Throw<ObjectDisposedException>();
    }
}
