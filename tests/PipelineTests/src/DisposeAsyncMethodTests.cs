using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipelineTests.DisposeAsyncMethodTests;

[TestClass]
[TestCategory("PipelineTests")]
public class TheDisposeAsyncMethod
{
    [TestMethod]
    public async Task ShallFlushThePipeline()
    {
        // arrange
        List<LogMessage> logMessages = new();
        LogMessagePipeline logMessagePipeline = new();
        logMessagePipeline.AttachOutputHandler(logMessage =>
        {
            logMessages.Add(logMessage);

            return Task.CompletedTask;
        });

        // act
        logMessagePipeline.Post(new LogMessage { LogLevel = LogLevel.Info });
        logMessagePipeline.Post(new LogMessage { LogLevel = LogLevel.Info });
        logMessagePipeline.Post(new LogMessage { LogLevel = LogLevel.Info });
        await logMessagePipeline.DisposeAsync().ConfigureAwait(false);

        // assert
        logMessages.Count.Should().Be(3);
    }
}
