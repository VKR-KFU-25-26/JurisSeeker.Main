using System.Text;
using JurisSeeker.Application;
using JurisSeeker.Infrastructure;
using JurisSeeker.Kafka;
using JurisSeeker.Persistence;
using JurisSeeker.Persistence.Helpers;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddKafka(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); 
    });
    
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using var scoped = app.Services.CreateScope();
var migrator = scoped.ServiceProvider.GetRequiredService<Migrator>();
await migrator.MigrateAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("📨 Request: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
    logger.LogInformation("📬 Response: {StatusCode} for {Path}", context.Response.StatusCode, context.Request.Path);
});

app.UseRouting();
app.UseCors("AllowFrontend"); 

app.MapGet("/", () => "JurisSeeker API is running!");
app.MapGet("/health", () => new { status = "Healthy", time = DateTime.UtcNow });
app.MapGet("/api/status", () => new { 
    message = "API is ready", 
    timestamp = DateTime.UtcNow,
    graphql = "/graphql",
    swagger = "/swagger"
});
app.MapGet("/api/version", () => new { version = "1.0.0", environment = app.Environment.EnvironmentName });

app.MapControllers();
app.MapGraphQL(); 

app.Run();