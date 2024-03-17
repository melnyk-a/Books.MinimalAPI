using Books.Application;
using Books.Infrastructure;
using Books.MinimalApi;
using Books.MinimalApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddProblemDetails();

// AuthNZ
builder.Services
    .AddAuthentication()
    .AddJwtBearer();
builder.Services.AddAuthorization();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminFromUkraine", policy =>
        policy
            .RequireRole("admin")
            .RequireClaim("country", "Ukraine"));

// Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("TokenAuthNZ",
        new()
        {
            Name = "Authorization",
            Description = "Token-based authentication and authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "Bearer",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header
        });
    options.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "TokenAuthNZ"
                }
            }, new List<string>()
        }
    });
});

var app = builder.Build();

app.SeedData();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();

    app.UseSwagger();
    app.UseSwaggerUI();

    // if there is no ProblemDetails
    //app.UseExceptionHandler(builder =>
    //{
    //    builder.Run(
    //        async context =>
    //        {
    //            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    //            context.Response.ContentType = "text/html";
    //            await context.Response.WriteAsync("An unexpected problem happened");
    //        });
    //});
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.RegisterCountryEndpoints();
app.RegisterBooksEndpoints();

app.Run();