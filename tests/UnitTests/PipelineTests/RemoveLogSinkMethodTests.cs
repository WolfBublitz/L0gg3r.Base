using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.PipelineTests.AddLogSinkMethodTests;

namespace UnitTests.PipelineTests.RemoveLogSinkMethodTests;

[TestClass]
public class TheRemoveLogSinkMethod
{
    [TestMethod]
    public void ShallRemoveTheLogSink()
    {
        // arrange
        LogSink logSink = new();
        LogMessagePipeline logMessagePipeline = new();
        logMessagePipeline.AddLogSink(logSink);

        // assert
        logMessagePipeline.LogSinks.Should().Contain(logSink);

        // act
        logMessagePipeline.RemoveLogSink(logSink);

        // assert
        logMessagePipeline.LogSinks.Should().NotContain(logSink);
    }
}
