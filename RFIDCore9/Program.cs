using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
static void Main()
{
    string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJHbG9iYWxETVMiLCJqdGkiOiI4ZjYxYmFjNy1lZWE5LTQwNzItOGIwOC1jZjZhMTJmZDhiMDkiLCJpYXQiOiIyMS8wMi8yMDI1IDAzOjEyOjE3IiwiVVNVQVJJTyI6IlNIQU5HNzAiLCJFTVBSRVNBSUQiOiIxIiwiQUdFTkNJQUlEIjoiMSIsImV4cCI6MTc0MDE5MzkzN30.vaBVMpM1fT5YwFPesljr_Fi7UQIMmFRZ5mvVLvyFqUo";
    string secretKey = "GlobalDms@llavesupersercretanoselacompartasanadie";

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(secretKey);

    var validationParams = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // Habilítalos si necesitas validar el issuer
        ValidateAudience = false, // Habilítalos si necesitas validar la audiencia
        ValidateLifetime = false
    };

    try
    {
        var principal = tokenHandler.ValidateToken(token, validationParams, out SecurityToken validatedToken);
        Console.WriteLine("✅ Token firmado correctamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Token inválido: {ex.Message}");
    }
}

// 🔹 1️⃣ Configurar JWT desde appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"] ??
          throw new ArgumentNullException("JWT Key is missing in configuration!");

// 🔹 2️⃣ Configuración de autenticación JWT
builder.Services.AddAuthentication(config =>
    {
        config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        };
    });

// 🔹 3️⃣ Agregar autorización
builder.Services.AddAuthorization();

// 🔹 4️⃣ Agregar soporte para controladores
builder.Services.AddControllers();

// 🔹 5️⃣ Configuración de Swagger con autenticación JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "globaldmsweb", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingrese el token JWT en este formato: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },new string[] { }
        }
    });
});

var app = builder.Build();

// 🔹 6️⃣ Habilitar enrutamiento
app.UseRouting();

app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"];
    Console.WriteLine($"Authorization Header: {authHeader}");
    await next();
});

// 🔹 7️⃣ Middleware de autenticación y autorización en el orden correcto
app.UseAuthentication();
app.UseAuthorization();

// 🔹 8️⃣ Habilitar Swagger en producción
app.UseSwagger();
app.UseSwaggerUI();

// 🔹 9️⃣ Habilitar redirección a HTTPS si es necesario
// app.UseHttpsRedirection();

// 🔹 🔟 Asignar controladores y ejecutar la aplicación
app.MapControllers();
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ ERROR: {ex.Message}");
        Console.WriteLine($"🛠 StackTrace: {ex.StackTrace}");
        throw;
    }
});
app.Run();