// Pruebas del sobre de respuesta de la API.
using USystem.Application.Common;
using USystem.Domain.Common;

namespace USystem.Tests.UnitTests;

public class ApiResponseTests
{
    [Fact]
    public void OkSetsSuccessMessageAndData()
    {
        var Response = ApiResponse<PagedResult<string>>.Ok(new PagedResult<string> { TotalCount = 3 });

        Assert.True(Response.Success);
        Assert.Equal("OK", Response.Message);
        Assert.NotNull(Response.Data);
        Assert.Equal(3, Response.Data!.TotalCount);
        Assert.Empty(Response.Errors);
    }

    [Fact]
    public void FailSetsErrorsAndKeepsDataEmpty()
    {
        var Response = ApiResponse<string>.Fail(
            "Datos inválidos",
            new ApiError { Code = "Validation.Required", Field = "Email", Message = "Email es obligatorio." });

        Assert.False(Response.Success);
        Assert.Null(Response.Data);
        Assert.Single(Response.Errors);
        Assert.Equal("Validation.Required", Response.Errors[0].Code);
        Assert.Equal("Email", Response.Errors[0].Field);
    }

    [Fact]
    public void PagedResultComputesNavigationProperties()
    {
        var Result = new PagedResult<int> { Items = new List<int> { 1, 2 }, TotalCount = 45, Page = 2, PageSize = 20 };

        Assert.Equal(3, Result.TotalPages);
        Assert.True(Result.HasNextPage);
        Assert.True(Result.HasPreviousPage);
    }
}
