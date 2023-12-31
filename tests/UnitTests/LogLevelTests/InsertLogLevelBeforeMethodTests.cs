using System;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.LogLevelTests.InsertLogLevelBeforeMethodTests;

[TestClass]
[TestCategory("UnitTests")]
public class TheInsertLogLevelBeforeMethod
{
    [TestInitialize]
    public void TestInitialize()
    {
        LogLevel.ResetOrder();
    }

    [TestMethod]
    public void ShallInsertANewLogLevelBeforeTheGivenLogLevel()
    {
        // assert
        LogLevel.Order.Should().Equal(new LogLevel[]
        {
            LogLevel.Info, LogLevel.Warning, LogLevel.Error,
        });

        // act
        LogLevel logLevel = LogLevel.InsertLogLevelBefore(LogLevel.Info, "Debug", "Debugging information.");

        // assert
        LogLevel.Order.Should().Equal(new LogLevel[]
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
