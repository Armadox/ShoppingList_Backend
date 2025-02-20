using System.Text.Json.Serialization;
using API.Controllers.Services;
using API.Data;
using API.Hubs;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load(); 

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var address = Environment.GetEnvironmentVariable("ADDRESS");

if (string.IsNullOrEmpty(address))
{
    throw new InvalidOperationException("FOR ADMIN: Please config ENV file! Address missing!");
}


builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<ListService>();
builder.Services.AddScoped<ItemService>();
builder.Services.AddDbContext<StoreContext>(options => 
{
    var dbConnection = Environment.GetEnvironmentVariable("DB_CONNECTION");

    if (string.IsNullOrEmpty(dbConnection))
    {
        throw new InvalidOperationException("FOR ADMIN: Please config ENV file! Connection missing!");
    }

    options.UseNpgsql(dbConnection);
});

/* For local DATABASE
builder.Services.AddDbContext<StoreContext>(options => 
{
    options.UseSqlite(builder.Configuration.GetConnectionString("D-Connection"));
});
*/

var app = builder.Build();

app.UseCors("AllowAll");

app.MapControllers();
app.MapHub<ListHub>("/listHub");

DbInitializer.InitDb(app);

app.Run();
