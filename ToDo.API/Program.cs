

using Microsoft.EntityFrameworkCore;
using ToDo.DataAccess.Data;
using ToDo.DataAccess.Repositories;
using ToDo.DataAccess.Repositories.IRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//Swagger for easier testing
builder.Services.AddSwaggerGen();
//EF Configuration
builder.Services.AddDbContext<AppDataContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Data")));
//AutoMapper Configuration
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//UnitOfWork Register
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
