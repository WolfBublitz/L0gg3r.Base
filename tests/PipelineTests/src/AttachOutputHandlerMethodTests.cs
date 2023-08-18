using System;
using System.Threading.Tasks;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipelineTests.AttachOutputHandlerMethodTests;

[TestClass]
[TestCategory("PipelineTests")]
public class TheAttachOutputHandlerMethod
{
    [TestMethod]
    public async Task ShallAttachAnOutputHandler()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();
        TaskCompletionSource<bool> taskCompletionSource = new();

        Task Handler(LogMessage _)
        {
            taskCompletionSource.SetResult(true);

            return Task.CompletedTask;
        }

        // act
        logMessagePipeline.AttachOutputHandler(Handler);
        logMessagePipeline.Post(new LogMessage { LogLevel = LogLevel.Info });
        bool result = await taskCompletionSource.Task.ConfigureAwait(false);

        // assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task ShallReturnADisposableThatReleasesTheOutputAction()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();
        TaskCompletionSource<int> taskCompletionSource1 = new();
        TaskCompletionSource<int> taskCompletionSource2 = new();

        Task Handler1(LogMessage _)
        {
            taskCompletionSource1.SetResult(1);

            return Task.CompletedTask;
        }

        Task Handler2(LogMessage _)
        {
            taskCompletionSource2.SetResult(2);

            return Task.CompletedTask;
        }

        // act
        IDisposable disposable1 = logMessagePipeline.AttachOutputHandler(Handler1);
        IDisposable disposable2 = logMessagePipeline.AttachOutputHandler(Handler2);
        disposable1.Dispose();
        logMessagePipeline.Post(new LogMessage { LogLevel = LogLevel.Info });
        Task<int> task = await Task.WhenAny(taskCompletionSource1.Task, taskCompletionSource2.Task).ConfigureAwait(false);
        int result = await task.ConfigureAwait(false);

        // assert
        result.Should().Be(2);
    }

    [TestMethod]
    public void ShallThrowArgumentNullException()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();

        // act
        Action action = () => logMessagePipeline.AttachOutputHandler(null!);

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
        Action action = () => logMessagePipeline.AttachOutputHandler(_ => Task.CompletedTask);

        // assert
        action.Should().Throw<ObjectDisposedException>();
    }
}
