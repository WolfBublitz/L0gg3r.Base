using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.LogMessageTests.UnequalityOperatorTests;

[TestClass]
[TestCategory("UnitTests")]
public class TheUnequalityOperator
{
    [DataTestMethod]
    [DynamicData(nameof(ShallIndicateWhetherTwoLogMessagesAreEqualData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(ShallIndicateWhetherTwoLogMessagesAreEqualDisplayName))]
    public void ShallIndicateWhetherTwoLogMessagesAreUnequal(LogMessage logMessage1, LogMessage logMessage2, bool expectedResult)
    {
        // arrange
        bool result = logMessage1 != logMessage2;

        // assert
        result.Should().Be(expectedResult);
    }

    private static IEnumerable<object[]> ShallIndicateWhetherTwoLogMessagesAreEqualData()
    {
        LogMessage logMessage = new();

        yield return new object[]
        {
            logMessage,
            logMessage,
            false
        };

        yield return new object[]
        {
            logMessage,
            new LogMessage(),
            true
        };
    }

    public static string ShallIndicateWhetherTwoLogMessagesAreEqualDisplayName(MethodInfo methodInfo, object[] data)
    {
        return $"{data[0]}, {data[1]}";
    }
}
