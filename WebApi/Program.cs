using Application.Services;
using Core.Interfaces;
using Core.Models;
using Infrastructure.ApiClients;
using Microsoft.Extensions.Options;
using Polly;
using Serilog;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

var configuration = configurationBuilder.Build();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, config) => config
.WriteTo.Console()
.MinimumLevel.Information());

builder.Services.Configure<BookApiConfig>(configuration.GetSection(nameof(BookApiConfig)));
builder.Services.Configure<BookOwnerConfig>(configuration.GetSection(nameof(BookOwnerConfig)));

builder.Services.AddTransient<IBookApiClient, BookApiClient>();
builder.Services.AddTransient<IBookService, BookService>();

var retryPolicy = Policy<HttpResponseMessage>.Handle<Exception>()
            .OrResult(response => (int)response.StatusCode >= 300)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

builder.Services.AddHttpClient<IBookApiClient, BookApiClient>((serviceProvider, httpClient) =>
{
    var bookApiConfig = serviceProvider.GetRequiredService<IOptions<BookApiConfig>>().Value;
    httpClient.BaseAddress = new Uri(bookApiConfig.BaseUrl);
})
.AddPolicyHandler(retryPolicy)
.SetHandlerLifetime(TimeSpan.FromSeconds(30));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.MapControllers();

app.Run();
