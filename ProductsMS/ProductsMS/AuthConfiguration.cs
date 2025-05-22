using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ProductsMS
{
    public static class AuthConfiguration
    {
        public static IServiceCollection KeycloakConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configurar autenticación basada en JWT con Keycloak
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Audience = configuration["Authentication:Audience"];
                    options.MetadataAddress = configuration.GetValue<string>("Authentication:MetadataAddress");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Authentication:ValidIssuer"],
                        ValidateAudience = true,
                        ValidAudiences = new[] { "account", "realm-management" },
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    // Agregar eventos para depuración
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "").Trim();
                            Console.WriteLine($"Token recibido: {token}");
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"Error de autenticación: {context.Exception}");
                            return Task.CompletedTask;
                        }
                    };
                });

            // Configurar políticas de autorización basadas en roles de Keycloak
            services.AddAuthorization(o =>
            {
                o.AddPolicy("AdministradorPolicy", policy =>
                    policy.RequireAuthenticatedUser()
                        .RequireAssertion(context =>
                        {
                            var resourceAccess = context.User.FindFirst("resource_access")?.Value;
                            if (string.IsNullOrEmpty(resourceAccess)) return false;

                            var resourceAccessJson = JsonDocument.Parse(resourceAccess);
                            if (resourceAccessJson.RootElement.TryGetProperty("admin-client", out var webClientAccess))
                            {
                                return webClientAccess.GetProperty("roles").EnumerateArray()
                                    .Any(role => role.GetString() == "Administrator");
                            }

                            return false;
                        }));

                o.AddPolicy("PostorPolicy", policy =>
                    policy.RequireAuthenticatedUser()
                        .RequireAssertion(context =>
                        {
                            var resourceAccess = context.User.FindFirst("resource_access")?.Value;
                            if (string.IsNullOrEmpty(resourceAccess)) return false;

                            var resourceAccessJson = JsonDocument.Parse(resourceAccess);
                            if (resourceAccessJson.RootElement.TryGetProperty("admin-client", out var webClientAccess))
                            {
                                return webClientAccess.GetProperty("roles").EnumerateArray()
                                    .Any(role => role.GetString() == "Auctioneer");
                            }

                            return false;
                        }));

                o.AddPolicy("SubastadorPolicy", policy =>
                    policy.RequireAuthenticatedUser()
                        .RequireAssertion(context =>
                        {
                            var resourceAccess = context.User.FindFirst("resource_access")?.Value;
                            if (string.IsNullOrEmpty(resourceAccess)) return false;

                            var resourceAccessJson = JsonDocument.Parse(resourceAccess);
                            if (resourceAccessJson.RootElement.TryGetProperty("admin-client", out var webClientAccess))
                            {
                                return webClientAccess.GetProperty("roles").EnumerateArray()
                                    .Any(role => role.GetString() == "Bidder");
                            }

                            return false;
                        }));

                o.AddPolicy("SoportePolicy", policy =>
                    policy.RequireAuthenticatedUser()
                        .RequireAssertion(context =>
                        {
                            var resourceAccess = context.User.FindFirst("resource_access")?.Value;
                            if (string.IsNullOrEmpty(resourceAccess)) return false;

                            var resourceAccessJson = JsonDocument.Parse(resourceAccess);
                            if (resourceAccessJson.RootElement.TryGetProperty("admin-client", out var webClientAccess))
                            {
                                return webClientAccess.GetProperty("roles").EnumerateArray()
                                    .Any(role => role.GetString() == "Support");
                            }

                            return false;
                        }));
            });

            // Agregar IHttpContextAccessor (necesario para acceder al contexto HTTP)
            services.AddHttpContextAccessor();

           

            return services;
        }
    }


}
