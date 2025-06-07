using ConcertoReservoApi.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace ConcertoReservoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //TODO
            //- hosting parameters
            //- configuration

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            DependencyConfiguration.Register(builder);

            //normally wouldn't put this here, but moving fast for now
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.SlidingExpiration = true;
                options.AccessDeniedPath = "/";
                options.Cookie.Name = "auth-cookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.MaxAge = TimeSpan.FromDays(365);

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToLogout = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("events", policy => policy.RequireAssertion(a => a.User?.Claims?.FirstOrDefault(c => c.Type == "permissions")?.Value?.Contains("Events") ?? false));
                options.AddPolicy("venues", policy => policy.RequireAssertion(a => a.User?.Claims?.FirstOrDefault(c => c.Type == "permissions")?.Value?.Contains("Venues") ?? false));
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseRouting();
            app.MapControllers();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.Run();
        }
    }
}
