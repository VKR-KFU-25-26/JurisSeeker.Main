using System.Text;
using JurisSeeker.Infrastructure;
using JurisSeeker.Infrastructure.Services.Elastic;
using JurisSeeker.Persistence;
using JurisSeeker.Persistence.Helpers;
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

using var scoped = app.Services.CreateScope();
var migrator = scoped.ServiceProvider.GetRequiredService<Migrator>();
await migrator.MigrateAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.MapControllers();
app.MapGraphQL("/graphql"); 

app.Run();
