namespace Books.MinimalApi.Endpoints.Internal;

public interface IEndpoint
{
    public static abstract void DefineEndpoints(IEndpointRouteBuilder builder);

    public static abstract void AddServices(IServiceCollection services,
        IConfiguration configuration);
}
