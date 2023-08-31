using System;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.LogMessageTests;

[TestClass]
[TestCategory("UnitTests")]
public class TheConstructor
{
    [TestMethod]
    public void ShallInitializeTheLogMessage()
    {
        // act
        LogMessage logMessage = new();

        // assert
        logMessage.LogLevel.Should().Be(LogLevel.Info);
        logMessage.Payload.Should().BeNull();
        logMessage.Timestamp.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromMilliseconds(100));
        logMessage.Senders.Should().BeEmpty();
    }
}
