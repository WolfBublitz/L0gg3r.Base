using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.LogLevelTests.LogLevelOrderTests;

[TestClass]
public class TheOrder
{
    [TestMethod]
    public void ShallBe()
    {
        // assert
        (LogLevel.Info < LogLevel.Warning).Should().BeTrue();
        (LogLevel.Info < LogLevel.Error).Should().BeTrue();
        (LogLevel.Warning < LogLevel.Error).Should().BeTrue();
    }
}
