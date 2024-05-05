using Esercizio_DTO.Autenticazione;
using Esercizio_DTO.Database;
using Esercizio_DTO.Profiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Esercizio_DTO
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            String key = "12345678901234567890123456789012";

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddAutoMapper(typeof(ProdottoProfile).Assembly);
            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("ProdottiDb"));
            builder.Services.AddSingleton<JwtAuthManager>(new JwtAuthManager(key));      // Configurazione di autenticazione/autorizzazione. Prima parte

            // builder.Services.AddMemoryCache();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>         // Seconda parte
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Inserire parola chiave Bearer più il token restituito dal metodo Auth",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement         // Seconda parte
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme, Id = "Bearer"
                            }
                        },
                    new String[] {}
                    }
                });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>      // Terza parte
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,                 // disabilita la convalida dell'emittente del token. In un contesto di testing. In produzione si può pensare di settarlo a "true"
                    ValidateAudience = false,               // disabilita la convalida del token della chiave pubblica. Potrebbe venire abilitata in produzione
                    ValidateIssuerSigningKey = true,        // colui che sta emettendo questo token/key, deve essere convalidato durante il processo di autenticazione
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
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

            app.UseAuthentication();        // Abilitazione middleware di autenticazione. Quarta parte

            app.UseAuthorization();         // Abilitazione middleware di autorizzazione


            app.MapControllers();

            app.Run();
        }
    }
}
