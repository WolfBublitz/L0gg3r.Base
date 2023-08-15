using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipelineTests.TransformPropertyTests;

[TestClass]
public class TheTransformProperty
{
    [TestMethod]
    public async Task ShallEnableATransformation()
    {
        // arrange
        List<LogMessage> receivedLogMessages = new();
        LogMessagePipeline logMessagePipeline = new()
        {
            Transform = logMessage =>
            {
                return new LogMessage()
                {
                    LogLevel = logMessage.LogLevel,
                    Payload = "transformed payload",
                    Senders = logMessage.Senders
                };
            },
        };
        logMessagePipeline.AttachOutputHandler(logMessage =>
        {
            receivedLogMessages.Add(logMessage);

            return Task.CompletedTask;
        });

        // act
        logMessagePipeline.Post(new LogMessage { LogLevel = LogLevel.Info });
        await logMessagePipeline.DisposeAsync().ConfigureAwait(false);

        // assert
        receivedLogMessages.Should().HaveCount(1);
        receivedLogMessages.First().Payload.Should().Be("transformed payload");
    }

    [TestMethod]
    public void ShallThrowArgumentNullException()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();

        // act
        Action action = () => logMessagePipeline.Transform = null!;

        // assert
        action.Should().Throw<ArgumentNullException>();
    }
}
