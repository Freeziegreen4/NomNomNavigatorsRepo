using Microsoft.EntityFrameworkCore;
using NomNomsAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
/*
 * Make sure to add this to your appsettings.json file
 * ,
    "AllowedHosts": "*",
    "ConnectionStrings": {
        "NomNomConn": "Server=[YOUR_SERVER//DB_NAME_HERE];Database=NomNomDB;MultipleActiveResultSets=true"
    }
 * The reasoning for this exclusion has been detailed in the
 *  .gitignore file.
 */
builder.Services.AddDbContext<NomNomDBContext>(opts => {
    opts.UseSqlServer(
    builder.Configuration["ConnectionStrings:NomNomConn"]);
});
//builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
