using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.PipelineTests.RemoveFilterMethodTests;

[TestClass]
public class TheRemoveFilterMethod
{
    [TestMethod]
    public void ShallRemoveTheFilter()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();
        static bool filter(LogMessage logMessage) => true;
        logMessagePipeline.AddFilter(filter);

        // assert
        logMessagePipeline.Filters.Should().Contain(filter);

        // act
        logMessagePipeline.RemoveFilter(filter);

        // assert
        logMessagePipeline.Filters.Should().NotContain(filter);
    }
}
