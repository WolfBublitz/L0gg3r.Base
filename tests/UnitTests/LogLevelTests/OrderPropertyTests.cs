using System.Collections.Generic;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.LogLevelTests.OrderPropertyTests;

[TestClass]
public class TheOrderProperty
{
    [TestMethod]
    public void ShallHaveADefaultValue()
    {
        // arrange

        // act
        IReadOnlyList<LogLevel> order = LogLevel.Order;

        // assert
        order.Should().Equal(new[]
        {
            LogLevel.Info,
            LogLevel.Warning,
            LogLevel.Error,
        });
    }
}
