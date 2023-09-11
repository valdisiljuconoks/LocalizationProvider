using System;
using DbLocalizationProvider.Internal;
using Xunit;

namespace DbLocalizationProvider.Tests;

public class PrimitiveDataTypeTests
{
    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(int?))]
    [InlineData(typeof(Guid))]
    public void CheckAllPrimitiveTypes(Type dataType)
    {
        Assert.True(dataType.IsSimpleType());
    }
}
