using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "globalapiMariaDB", Version = "v1" });

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
app.Run();