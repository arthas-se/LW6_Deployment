using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using Task2.Data_Tier.DataBase;
using Task2.Data_Tier.Interfaces;
using Task2.Data_Tier.Repositories;
using Task2.Logic_Tier.Interfaces;
using Task2.Services;
using Task2.Utils;
using Task2.Utils.Mapping;
using Task2.Utils.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IScooterRepository, ScooterRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IScooterService, ScooterService>();
builder.Services.AddScoped<IRentalService, RentalService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(typeof(UserProfile), typeof(ScooterProfile), typeof(RentalProfile));

builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddSingleton<MongoDBClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration["MongoDBSettings:ConnectionString"];
    var databaseName = configuration["MongoDBSettings:DatabaseName"];

    return new MongoDBClient(connectionString, databaseName);
});

var app = builder.Build();

app.Services.GetRequiredService<MongoDBClient>();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    options.RoutePrefix = string.Empty;
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();