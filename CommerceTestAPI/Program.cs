using System.Reflection;
using CommerceTest.Infrastructure.BigCommerce.ServiceCollections;
using CommerceTestAPI.Mutations;
using CommerceTestAPI.Queries;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings." + environmentName + ".json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
}

builder.Services
    .AddGraphQLServer()
    .AddMutationType<Mutation>()
    .AddTypeExtension<BasketMutations>()
    .AddCommerceAPITypes();

builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

builder.Services.AddBigCommerceServices(builder.Configuration.GetSection("BigCommerce"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGraphQL();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();
