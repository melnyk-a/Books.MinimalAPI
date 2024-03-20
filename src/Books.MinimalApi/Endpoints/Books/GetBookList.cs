using Books.MinimalApi.Endpoints.Internal;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Books.MinimalApi.Endpoints.Books;

public class GetBookList : IEndpoint
{
    public static void AddServices(IServiceCollection services,
        IConfiguration configuration)
    {
        // Do nothing, all services were registered in infrastructure layer
    }

    public static void DefineEndpoints(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("/books", GetBooksAsync)
            .AllowAnonymous();
    }

    private static async Task<Ok<IReadOnlyList<BookResponse>>> GetBooksAsync(
        ISender sender,
        [FromQuery] string? name,
        CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
