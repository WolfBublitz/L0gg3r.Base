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
        logMessagePipeline.Write(new LogMessage { LogLevel = LogLevel.Info, Payload = 1 });
        logMessagePipeline.Write(new LogMessage { LogLevel = LogLevel.Info, Payload = 2 });
        logMessagePipeline.Write(new LogMessage { LogLevel = LogLevel.Info, Payload = 3 });
        await logMessagePipeline.DisposeAsync().ConfigureAwait(false);

        // assert
        logMessages.Count.Should().Be(3);
        logMessages[0].Payload.Should().Be(1);
        logMessages[1].Payload.Should().Be(2);
        logMessages[2].Payload.Should().Be(3);
    }
}
