using System;
using System.Threading.Tasks;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.PipelineTests.AddLogSinkMethodTests;

internal class LogSink : ILogSink
{
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    public Task FlushAsync() => Task.CompletedTask;

    public Task ProcessAsync(in LogMessage logMessage) => Task.CompletedTask;
}

[TestClass]
[TestCategory("UnitTests")]
[TestCategory("PipelineTests")]
public class TheAddLogSinkMethod
{
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
