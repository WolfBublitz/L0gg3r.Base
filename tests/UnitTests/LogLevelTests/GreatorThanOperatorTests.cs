using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using L0gg3r.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.LogLevelTests.GreatorThanOperatorTests;

[TestClass]
public class TheGreatorThanOperator
{
    [DataTestMethod]
    [DynamicData(nameof(GetLogLevels), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetLogLevelsDisplayName))]
    public void ShallReturnTrueIfLeftIsGreatorThanRight(LogLevel left, LogLevel right, bool expectedResult)
    {
        // assert
        (left > right).Should().Be(expectedResult);
    }

    private static IEnumerable<object[]> GetLogLevels()
    {
        yield return new object[] { LogLevel.Info, LogLevel.Info, false };
        yield return new object[] { LogLevel.Info, LogLevel.Warning, false };
        yield return new object[] { LogLevel.Info, LogLevel.Error, false };

        yield return new object[] { LogLevel.Warning, LogLevel.Info, true };
        yield return new object[] { LogLevel.Warning, LogLevel.Warning, false };
        yield return new object[] { LogLevel.Warning, LogLevel.Error, false };

        yield return new object[] { LogLevel.Error, LogLevel.Info, true };
        yield return new object[] { LogLevel.Error, LogLevel.Warning, true };
        yield return new object[] { LogLevel.Error, LogLevel.Error, false };
    }

    public static string GetLogLevelsDisplayName(MethodInfo methodInfo, object[] data)
    {
        return $"{data[0]}, {data[1]}";
    }
}

