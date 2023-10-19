using System;
using System.Threading.Tasks;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.PipelineTests.AddFilterMethodTests;

[TestClass]
[TestCategory("UnitTests")]
[TestCategory("PipelineTests")]
public class TheAddFilterMethod
{
    [TestMethod]
    public void ShallAddTheFilter()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();
        static bool filter(LogMessage logMessage) => true;

        // act
        logMessagePipeline.AddFilter(filter);

        // assert
        logMessagePipeline.Filters.Should().Contain(filter);
    }

    [TestMethod]
    public void ShallThrowArgumentNullException()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();

        // act
        Action action = () => logMessagePipeline.AddFilter(null!);

        // assert
        action.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public async Task ShallThrowObjectDisposedException()
    {
        // arrange
        LogMessagePipeline logMessagePipeline = new();
        await logMessagePipeline.DisposeAsync().ConfigureAwait(false);

        // act
        Action action = () => logMessagePipeline.AddFilter(logMessage => true);

        // assert
        action.Should().Throw<ObjectDisposedException>();
    }

    [TestMethod]
    public void Example_ShallAddTheFilter()
    {
        #region AddFilter
        // creating a new log message pipeline
        LogMessagePipeline logMessagePipeline = new();

        // adding a filter that will only allow log messages with a log level of warning or higher
        logMessagePipeline.AddFilter(logMessage => logMessage.LogLevel >= LogLevel.Warning);
        #endregion
    }

    [TestMethod]
    public void Example_Dispose()
    {
        #region Dispose
        // creating a new log message pipeline
        LogMessagePipeline logMessagePipeline = new();

        // adding a filter that will only allow log messages with a log level of warning or higher
        using (IDisposable disposable = logMessagePipeline.AddFilter(logMessage => logMessage.LogLevel >= LogLevel.Warning))
        {
            logMessagePipeline.Write(new LogMessage()
            {
                LogLevel = LogLevel.Info,
                Payload = "This message will not be written"
            });
        } // the filter will be removed here

        logMessagePipeline.Write(new LogMessage()
        {
            LogLevel = LogLevel.Info,
            Payload = "This message will be written"
        });
        #endregion
    }
}
