using CustomHTTPClient;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//1-)Get Property From Config Constructor - RealTime
var configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", false, true)
           .Build();
builder.Services.Configure<CustomWebClientConfig>(configuration.GetSection("CustomWebClientConfig"));
builder.Services.AddTransient<CustomWebClient>();

//2-)Get Property From Parameter Constructor
//var settings = builder.Configuration.GetSection("CustomWebClientConfig").Get<CustomWebClientConfig>();
//builder.Services.AddTransient<CustomWebClient>(timeOut => ActivatorUtilities.CreateInstance<CustomWebClient>(timeOut, settings.TimeOut));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
