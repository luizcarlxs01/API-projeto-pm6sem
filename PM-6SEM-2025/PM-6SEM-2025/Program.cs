using Microsoft.EntityFrameworkCore;
using PM_6SEM_2025.Data;
using PM_6SEM_2025.Services.Denuncias;
using PM_6SEM_2025.Services.Usuarios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PM_6SEM_2025.Services.Auth;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ===========================================
// CONFIGURAÇÃO DE CORS
// ===========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJs", policy =>
        policy.WithOrigins(
                "http://localhost:3000",
                "http://192.168.0.13:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
        );
});

// ===========================================
// CONTROLLERS + SWAGGER
// ===========================================
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===========================================
// DATABASE
// ===========================================
builder.Services.AddDbContext<PmContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// ===========================================
// INJEÇÃO DE DEPENDÊNCIA
// ===========================================
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<IDenunciasService, DenunciasService>();
builder.Services.AddScoped<ITokenService, TokenService>(); // Serviço de geração de token JWT

// ===========================================
// AUTH + JWT (lendo do appsettings)
// ===========================================
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("Configure Jwt:Key no appsettings.json.");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Em produção, mantenha HTTPS configurado corretamente no Azure
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Autorização (se quiser políticas, adiciona aqui depois)
builder.Services.AddAuthorization();

var app = builder.Build();

// ===========================================
// PIPELINE
// ===========================================

// Se quiser Swagger também em produção, pode tirar o if
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowNextJs");

// Ordem correta: autenticação antes de autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
