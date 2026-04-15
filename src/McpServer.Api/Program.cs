using McpServer.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "World Cup MCP API",
        Version = "v1",
        Description = "REST API for World Cup matches and user message replay"
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register singleton store
builder.Services.AddSingleton<WorldCupStore>(sp =>
{
    var store = new WorldCupStore();
    store.SeedSampleData();
    return store;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "World Cup MCP API v1");
});

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Log startup info
app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║      World Cup MCP API Server                             ║");
    Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
    Console.WriteLine();
    Console.WriteLine($"API running on: http://localhost:5001");
    Console.WriteLine($"Swagger UI: http://localhost:5001/swagger");
    Console.WriteLine();
});

app.Run();
