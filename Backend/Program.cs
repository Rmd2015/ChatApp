using Backend.Data;                    
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// ====================== Configuration de la base de données ======================

// Récupère la connection string depuis appsettings.json (recommandé)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("La connection string 'DefaultConnection' n'est pas configurée.");
}

// Ajout du DbContext avec PostgreSQL (Npgsql)
builder.Services.AddDbContext<ChatDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql =>
        {
         
        });
});
// ====================== JWT Configuration ======================
var jwtSection = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key is missing"))),

            ClockSkew = TimeSpan.Zero   // Évite le délai de 5 minutes par défaut
        };
    });
// ====================== Ajout des services ======================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ajout de CORS (important pour React)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.AllowAnyOrigin()           // À restreindre en production
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ====================== Configuration du pipeline ======================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors("AllowReactApp");     // Active CORS
app.UseAuthentication();     
app.UseAuthorization();

app.MapControllers();

app.Run();