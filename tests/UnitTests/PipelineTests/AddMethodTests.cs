using System;
using System.Threading.Tasks;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.PipelineTests.AddMethodTests;

internal class LogSink : ILogSink
{
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    public Task FlushAsync() => Task.CompletedTask;

    public Task ProcessAsync(in LogMessage logMessage) => Task.CompletedTask;
}

[TestClass]
public class TheAddFilterMethod
{
    [TestMethod]
    public void ShallAddTheFilter()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();
        static bool filter(LogMessage logMessage) => true;

        // act
        logMessagePipeline.Add(filter);

        // assert
        logMessagePipeline.Filters.Should().Contain(filter);
    }

    [TestMethod]
    public void ShallAddTheLogSink()
    {
        // arrange
        LogSink logSink = new();
        LogMessagePipeline logMessagePipeline = new();

        // act
        logMessagePipeline.Add(logSink);

        // assert
        logMessagePipeline.LogSinks.Should().Contain(logSink);
    }

    [TestMethod]
    public void ShallThrowArgumentNullExceptionIfLogSinkIsNull()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();

        // act
        Action action = () => logMessagePipeline.Add((ILogSink)null!);

        // assert
        action.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void ShallThrowArgumentNullExceptionIfFilterIsNull()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();

        // act
        Action action = () => logMessagePipeline.Add((Predicate<LogMessage>)null!);

        // assert
        action.Should().Throw<ArgumentNullException>();
    }
}
