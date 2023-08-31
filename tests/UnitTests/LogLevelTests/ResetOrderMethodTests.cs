using System.Collections.Generic;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.LogLevelTests.ResetOrderMethodTests;

[TestClass]
[TestCategory("UnitTests")]
public class TheResetOrderMethod
{
    [TestMethod]
    public void ShallResetTheOrderToItsInitialState()
    {
        // assert
        LogLevel.Order.Should().Equal(new[]
        {
            LogLevel.Info,
            LogLevel.Warning,
            LogLevel.Error,
        });

        // act
        LogLevel debugLogLevel = LogLevel.InsertLogLevelBefore(LogLevel.Info, "Debug");

        // assert
        LogLevel.Order.Should().Equal(new[]
        {
            debugLogLevel,
            LogLevel.Info,
            LogLevel.Warning,
            LogLevel.Error,
        });

        // act
        LogLevel.ResetOrder();

        // assert
        LogLevel.Order.Should().Equal(new[]
        {
            LogLevel.Info,
            LogLevel.Warning,
            LogLevel.Error,
        });
    }
}
