using System;
using System.Collections.Generic;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipelineTests.ConstructorTests;

[TestClass]
[TestCategory("UnitTests")]
[TestCategory("PipelineTests")]
public class TheConstructor
{
    [TestMethod]
    public void ShallCreateAPipelineWithoutLogSinks()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();

        // act
        IEnumerable<ILogSink> logSinks = logMessagePipeline.LogSinks;

        // assert
        logSinks.Should().BeEmpty();
    }

    [TestMethod]
    public void ShallCreateAPipelineWithoutFilters()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();

        // act
        IEnumerable<Predicate<LogMessage>> filters = logMessagePipeline.Filters;

        // assert
        filters.Should().BeEmpty();
    }

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
