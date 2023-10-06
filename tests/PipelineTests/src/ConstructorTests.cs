using System;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipelineTests.ConstructorTests;

[TestClass]
[TestCategory("PipelineTests")]
public class TheConstructor
{
    [TestMethod]
    public void ShallCreateAPipelineWithANullTransform()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();

        // act
        Func<LogMessage, LogMessage>? transform = logMessagePipeline.Transform;

        // assert
        transform.Should().BeNull();
    }
}
