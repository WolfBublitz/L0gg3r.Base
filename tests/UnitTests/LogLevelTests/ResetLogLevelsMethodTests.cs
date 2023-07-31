using System;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.LogLevelTests.ResetLogLevelsMethodTests;

[TestClass]
public class TheResetLogLevelsMethod
{
    [TestInitialize]
    public void TestInitialize()
    {
        LogLevel.ResetLogLevels();
    }

    [TestMethod]
    public void ShallResetTheLogLevelsToDefault()
    {
        // assert
        LogLevel.LogLevels.Should().BeEquivalentTo(new LogLevel[]
        {
            LogLevel.Info, LogLevel.Warning, LogLevel.Error,
        });

        // act
        LogLevel logLevel = LogLevel.InsertLogLevelAfter(LogLevel.Error, "Fatal", "Debugging information.");

        // assert
        LogLevel.LogLevels.Should().BeEquivalentTo(new LogLevel[]
        {
            LogLevel.Info, LogLevel.Warning, LogLevel.Error, logLevel,
        });

        // act
        LogLevel.ResetLogLevels();

        // assert
        LogLevel.LogLevels.Should().BeEquivalentTo(new LogLevel[]
        {
            LogLevel.Info, LogLevel.Warning, LogLevel.Error,
        });
    }
}
