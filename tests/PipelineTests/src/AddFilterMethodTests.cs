using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipelineTests.AddFilterMethodTests;

[TestClass]
public class TheAttachFilterMethod
{
    [TestMethod]
    public async Task ShallAddAFilter()
    {
        // arrange
        List<LogMessage> receivedLogMessages = new();
        LogMessagePipeline logMessagePipeline = new();
        TaskCompletionSource<LogMessage> taskCompletionSource = new();

        logMessagePipeline.AttachOutputHandler(logMessage =>
        {
            taskCompletionSource.SetResult(logMessage);
        });

        // act
        logMessagePipeline.Post(new LogMessage { Payload = 1 });
        LogMessage logMessage1 = await taskCompletionSource.Task.ConfigureAwait(false);

        // assert
        logMessage1.Payload.Should().Be(1);

        // act
        taskCompletionSource = new();

        using (IDisposable disposable = logMessagePipeline.AddFilter(logMessage => logMessage.Payload is int number && number == 2))
        {
            logMessagePipeline.Post(new LogMessage { Payload = 1 });
            logMessagePipeline.Post(new LogMessage { Payload = 2 });
        }

        LogMessage logMessage2 = await taskCompletionSource.Task.ConfigureAwait(false);

        // assert
        logMessage2.Payload.Should().Be(2, because: "the filter should have removed the log message with the payload 1");
    }

    [TestMethod]
    public async Task ShallReturnADisposableThatRemovesTheFilter()
    {
        // arrange
        List<LogMessage> receivedLogMessages = new();
        LogMessagePipeline logMessagePipeline = new();
        TaskCompletionSource<LogMessage> taskCompletionSource = new();

        logMessagePipeline.AttachOutputHandler(logMessage =>
        {
            taskCompletionSource.SetResult(logMessage);
        });

        // act
        using (IDisposable disposable = logMessagePipeline.AddFilter(logMessage => logMessage.Payload is int number && number == 2))
        {
            logMessagePipeline.Post(new LogMessage { Payload = 1 });
            logMessagePipeline.Post(new LogMessage { Payload = 2 });

            LogMessage logMessage1 = await taskCompletionSource.Task.ConfigureAwait(false);

            // assert
            logMessage1.Payload.Should().Be(2, because: "the filter should have removed the log message with the payload 1");
        }

        // act
        taskCompletionSource = new();

        logMessagePipeline.Post(new LogMessage { Payload = 1 });

        LogMessage logMessage2 = await taskCompletionSource.Task.ConfigureAwait(false);

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
    public async void ShallThrowObjectDisposedException()
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
