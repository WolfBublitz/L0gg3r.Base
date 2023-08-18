using System.Threading.Tasks;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipelineTests.PostMethodTests;

[TestClass]
[TestCategory("PipelineTests")]
public class ThePostMethod
{
    [TestMethod]
    public async Task ShouldPostLogMessageToPipeline()
    {
        // arrange
        LogMessage? receivedLogMessage = null;
        LogMessage logMessage = new()
        {
            LogLevel = LogLevel.Info,
            Payload = "payload",
            Senders = new[] { "sender" }
        };
        LogMessagePipeline pipeline = new();
        pipeline.AttachOutputHandler(logMessage =>
        {
            receivedLogMessage = logMessage;

            return Task.CompletedTask;
        });

        // act
        pipeline.Post(logMessage);
        await pipeline.DisposeAsync().ConfigureAwait(false);

        // assert
        receivedLogMessage.Should().NotBeNull();
        receivedLogMessage.Should().BeEquivalentTo(logMessage);
    }
}
