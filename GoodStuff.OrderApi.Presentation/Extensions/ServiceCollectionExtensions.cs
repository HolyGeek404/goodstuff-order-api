
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

namespace GoodStuff.OrderApi.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureConfig(this IServiceCollection services, IConfigurationManager configuration)
    {
        var azureAd = configuration.GetSection("AzureAd");

        configuration.AddAzureKeyVault(new Uri(azureAd["KvUrl"]!), new DefaultAzureCredential());
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(azureAd);

        return services;
    }
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var tenantId = configuration.GetSection("AzureAd")["TenantId"];
        var swaggerScope = configuration.GetSection("Swagger")["Scope"];
        var authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";

        services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc("v1", new OpenApiInfo { Title = "GoodStuff Order Api Swagger", Version = "v1" });
            x.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "OAuth 2.0 Auth Code with PKCE",
                Name = "oauth2",
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize"),
                        TokenUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token"),
                        Scopes = new Dictionary<string, string> { { $"{swaggerScope}", "Base rights" } }
                    }
                }
            });
            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "oauth2"}
                    },
                    new[] { $"{swaggerScope}" }
                }
            });
        });

        return services;
    }
}