using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuramos CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// para produccion
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowFrontend", builder =>
//    {
//        builder.WithOrigins("https://bandaegc.com")
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//    });
//});

// Configure JWT authentication
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero // Esto elimina el margen de 5 minutos por defecto
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = async context =>
            {
                context.Fail(context.Exception);

                // 1. Configuramos la respuesta manual
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                string errorMessage = context.Exception is SecurityTokenExpiredException
                    ? "El token ha expirado. Por favor, inicia sesión nuevamente."
                    : "Token inválido. Verifica tu autenticación.";

                var response = new { ok = false, message = errorMessage };

                await context.Response.WriteAsJsonAsync(response);

                await context.Response.CompleteAsync();
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// para dev
app.UseCors("AllowAllOrigins");

// para produccion
//app.UseCors("AllowFrontend");

app.Run();
