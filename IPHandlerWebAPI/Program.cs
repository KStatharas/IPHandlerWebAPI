using IPHandlerWebAPI.Controllers;
using IPHandlerWebAPI.Data;
using IPHandlerWebAPI.Jobs;
using IPHandlerWebAPI.Repositories;
using IPHandlerWebAPI.Services;
using IPInfoProvider.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IPServiceImpl>();
builder.Services.AddScoped<IPRepository>();
builder.Services.AddScoped<IIPRepository, CachedIPRepository>();
builder.Services.AddSingleton<BackgroundJobs>();
builder.Services.AddHostedService<BackgroundIPService>();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//OData allows the user to make queries when accessing an entry point.
builder.Services.AddControllers()
    .AddOData(options => options
    .Select().OrderBy().Filter());

//Local Server
//builder.Services.AddDbContext<IPHandlerWebApiDBContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("IPHandlerAPIConnectionString")));


//Docker
builder.Services.AddDbContext<IPHandlerWebApiDBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DockerConnectionString")));



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


