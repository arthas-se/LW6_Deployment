using FluentValidation;
using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;
using Task2.Utils;
using Task2.Utils.Mapping;
using Task2.Utils.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Services.AddAutoMapper(typeof(ScooterProfile));
builder.Services.AddAutoMapper(typeof(RentalProfile));

builder.Services.AddAutoMapper(typeof(UserProfile), 
    typeof(ScooterProfile), 
    typeof(RentalProfile));

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    options.RoutePrefix = string.Empty;
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
