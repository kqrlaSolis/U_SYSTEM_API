// Pruebas de la paginación común.
using USystem.Domain.Common;

namespace USystem.Tests.UnitTests;

public class PagedQueryTests
{
    [Fact]
    public void DefaultsArePageOneAndPageSizeTwenty()
    {
        var Query = new PagedQuery();

        Assert.Equal(1, Query.Page);
        Assert.Equal(PagedQuery.DefaultPageSize, Query.PageSize);
        Assert.Equal(0, Query.Skip);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-4)]
    public void PageLowerThanOneIsClampedToOne(int Page)
    {
        var Query = new PagedQuery { Page = Page };

        Assert.Equal(1, Query.Page);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(500, PagedQuery.MaxPageSize)]
    public void PageSizeIsClamped(int PageSize, int Expected)
    {
        var Query = new PagedQuery { PageSize = PageSize };

        Assert.Equal(Expected, Query.PageSize);
    }

    [Fact]
    public void SkipUsesPageAndPageSize()
    {
        var Query = new PagedQuery { Page = 3, PageSize = 25 };

        Assert.Equal(50, Query.Skip);
    }
}
