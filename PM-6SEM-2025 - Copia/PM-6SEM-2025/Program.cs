using Microsoft.EntityFrameworkCore;
using PM_6SEM_2025.Data;
using PM_6SEM_2025.Services.Denuncias;
using PM_6SEM_2025.Services.Usuarios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PM_6SEM_2025.Services.Auth;
using System.Text;

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
// AUTH + JWT
// ===========================================
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "PM_6SEM_2025",
            ValidAudience = "PM_6SEM_2025",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = TokenService.GetSecurityKey(),
            ClockSkew = TimeSpan.Zero
        };
    });

// ===========================================
// CONTROLLERS + SWAGGER
// ===========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===========================================
// INJEÇÃO DE DEPENDÊNCIA
// ===========================================
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<IDenunciasService, DenunciasService>();

// ===========================================
// DATABASE
// ===========================================
builder.Services.AddDbContext<PmContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// ===========================================
// PIPELINE
// ===========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowNextJs");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
