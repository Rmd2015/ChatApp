using Backend.Data;                    
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Models;
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
/// ====================== JWT Authentication + Blacklist Verification ======================
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
            ClockSkew = TimeSpan.Zero
        };

        // ====================== BLACKLIST CHECK ======================
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                try
                {
                    var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

                    if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                    {
                        Console.WriteLine("No Bearer token found in header");
                        return;
                    }

                    var rawToken = authHeader.Replace("Bearer ", "").Trim();
                    Console.WriteLine($"Token to check in blacklist: {rawToken.Substring(0, 50)}...");

                    var dbContext = context.HttpContext.RequestServices
                        .GetRequiredService<ChatDbContext>();

                    var isBlacklisted = await dbContext.Tokens
                        .AnyAsync(t => t.Token == rawToken && t.Isvalid == false);

                    Console.WriteLine($"IsBlacklisted = {isBlacklisted}");

                    if (isBlacklisted)
                    {
                        Console.WriteLine("Token is blacklisted → rejecting request");
                        context.Fail("Token has been revoked");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in blacklist check: {ex.Message}");
                }
            }
        };
    });
// ====================== Ajout des services ======================

builder.Services.AddControllers();
// ====================== Swagger avec JWT ======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ChatApp API",
        Version = "v1",
        Description = "Backend API pour l'application de chat"
    });

    // Définition de la sécurité Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Entrez le token JWT : Bearer {votre_token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // Exigence de sécurité globale
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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