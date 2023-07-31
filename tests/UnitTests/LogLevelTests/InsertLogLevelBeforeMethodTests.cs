using System;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.LogLevelTests.InsertLogLevelBeforeMethodTests;

[TestClass]
public class TheInsertLogLevelBeforeMethod
{
    [TestInitialize]
    public void TestInitialize()
    {
        LogLevel.ResetLogLevels();
    }

    [TestMethod]
    public void ShallInsertANewLogLevelBeforeTheGivenLogLevel()
    {
        // assert
        LogLevel.LogLevels.Should().BeEquivalentTo(new LogLevel[]
        {
            LogLevel.Info, LogLevel.Warning, LogLevel.Error,
        });

        // act
        LogLevel logLevel = LogLevel.InsertLogLevelBefore(LogLevel.Info, "Debug", "Debugging information.");

        // assert
        LogLevel.LogLevels.Should().BeEquivalentTo(new LogLevel[]
        {
            logLevel, LogLevel.Info, LogLevel.Warning, LogLevel.Error,
        });
    }

    [TestMethod]
    public void ShallThrowArgumentExceptionIfNameAlreadyExists()
    {
        // act
        Action action = () => LogLevel.InsertLogLevelBefore(LogLevel.Info, "Info");

        // assert
        action.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void ShallThrowArgumentExceptionIfLogLevelDoesNotExists()
    {
        // arrange
        LogLevel logLevel = new("Fatal");

        // act
        Action action = () => LogLevel.InsertLogLevelAfter(logLevel, "Info");

        // assert
        action.Should().Throw<ArgumentException>();
    }
}
