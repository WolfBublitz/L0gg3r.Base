using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests.LogLevelTests.LogLevelsPropertyTests;

[TestClass]
public class TheLogLevelsProperty
{
    [TestMethod]
    public void ShallBeInitialized()
    {
        // act
        IReadOnlyCollection<LogLevel> logLevels = LogLevel.LogLevels;

        // assert
        logLevels.Should().BeEquivalentTo(new LogLevel[]
        {
            LogLevel.Info, LogLevel.Warning, LogLevel.Error,
        });
    }
}
