using System;
using System.Collections.Generic;
using Xunit;

namespace DbLocalizationProvider.Tests;

public class ListExtensionsTests
{
    [Fact]
    public void SimpleListSplit()
    {
        var list = new List<int>
        {
            1,
            2,
            3,
            4,
            5
        };

        var (head, tail) = list;

        Assert.Equal(1, head);
        Assert.Equal([
                         2,
                         3,
                         4,
                         5
                     ],
                     tail);

        var (_, tail2) = tail!;
        Assert.Equal([
                         3,
                         4,
                         5
                     ],
                     tail2);

    }

    [Fact]
    public void DeconstructingEmptyList_ShouldReturnEmpty()
    {
        var list = new List<int>();

        Assert.Throws<InvalidOperationException>(() =>
        {
            (int? head, var tail) = list;
        });
    }
    
    [Fact]
    public void DeconstructingSingleElementList_ShouldReturn1stElement()
    {
        List<int> list = [1];
        var (head, tail) = list;

        Assert.Equal(1, head);
    }
}
