using DataBase;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "YourIssuer", // Укажите ваш Issuer
        ValidAudience = "YourAudience", // Укажите вашу Audience
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aXj9cK2F7s8i4M1At0rDqLs5UbWz3VeYpEoHnXf6ZbIuBlJmQvTzCR9gWkF3Qj0oEtZaPpVYcM2Rf6H5eIx7S0LmJ3TcU8xYlK9VuFh4DiQmN0Yz5TfH2gMmOx8Rna6DwWqSvY1ZsF8Pj3JeRqU5KqL2BvIxY4GmN9ZoR7FqXc9KpCL2FgVrB1Ad3H6QzU8sM1EoG34WX9VfZx7JjHkUm5Rz1LgBc2YpQnTdV8K6PlGjR3FyAtX9VmZ5SkO8ZhN0LfAqD1CtJ7XyY2Oa4Ws3PrB9NeQ7GdKl0E6Kh8Tx5S1BzQvG3YhXz4OpRqF9M5Ct3KxU1Hw8Yp2DfRqW0J")) //Секретный ключ
    };
});

// Задайте строку подключения напрямую
var connectionString = "Host=localhost;Port=5432;Database=outdb;Username=postgres;Password=1234";

// Добавление контекста базы данных
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(connectionString)); // Используйте UseNpgsql для PostgreSQL

// Добавление контроллеров
builder.Services.AddControllers();

// Настройка Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Конфигурация HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

