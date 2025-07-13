using System.Text;
using DAYA.Cloud.Framework.V2.Authentication.Contracts;
using DAYA.Cloud.Framework.V2.Authentication.Contracts.AnonymousAuthentication;
using DAYA.Cloud.Framework.V2.Authentication.Contracts.ApiKeyAuthentication;
using DAYA.Cloud.Framework.V2.Authentication.Contracts.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace DAYA.Cloud.Framework.V2.Authentication.Authentication
{
    public static class DayaAuthenticationServiceCollectionExtension
    {
        public static void AddDayaAuthentication(
            this IServiceCollection services,
            Func<B2CAuthenticationConfig>? b2CConfigurationBuilder,
            Func<DayaCustomJwtConfig>? dayaCustomJwtConfigBuilder,
            Func<EntraExternalIdConfig>? entraExternalIdConfigBuilder,
            Action<IServiceCollection>? apiKeyStoreRegistrant,
            JwtBearerEvents? customJwtEvents = null)
        {
            B2CAuthenticationConfig? b2CConfiguration = null;
            if (b2CConfigurationBuilder != null)
            {
                b2CConfiguration = b2CConfigurationBuilder();
            }
            DayaCustomJwtConfig? dayaCustomJwtConfig = null;
            if (dayaCustomJwtConfigBuilder != null)
            {
                dayaCustomJwtConfig = dayaCustomJwtConfigBuilder();
            }

            EntraExternalIdConfig? entraExternalIdConfig = null;
            if (entraExternalIdConfigBuilder != null)
            {
                entraExternalIdConfig = entraExternalIdConfigBuilder();
            }

            if (b2CConfiguration != null)
            {
                if (b2CConfiguration.UseFakeAdb2c)
                {
                    var encryptionKey = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00").ToByteArray();
                    services.AddAuthentication(x =>
                        {
                            x.DefaultAuthenticateScheme = DayaAuthenticationSchemeNames.Adb2cScheme;
                            x.DefaultChallengeScheme = DayaAuthenticationSchemeNames.Adb2cScheme;
                        })
                        .AddJwtBearer(x =>
                        {
                            x.RequireHttpsMetadata = false;
                            x.SaveToken = true;
                            x.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(FakeJwtTokenGenerator.EncryptionKey),
                                ValidateIssuer = false,
                                ValidateAudience = false
                            };
                            if (customJwtEvents != null)
                            {
                                x.Events = customJwtEvents;
                            }
                        });
                    services.AddSingleton<IFakeJwtTokenGenerator, FakeJwtTokenGenerator>();
                }
                else
                {
                    services.AddAuthentication(DayaAuthenticationSchemeNames.Adb2cScheme)
                        .AddJwtBearer(DayaAuthenticationSchemeNames.Adb2cScheme, options =>
                        {
                            options.Authority = $"https://{b2CConfiguration.TenantName}.b2clogin.com/tfp/{b2CConfiguration.DomainName}/{b2CConfiguration.DefaultPolicy}";
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateAudience = true,
                                ValidateIssuer = true,
                                ValidAudiences = b2CConfiguration.Audiences.Split(","),
                                ValidIssuer = $"https://{b2CConfiguration.TenantName}.b2clogin.com/{b2CConfiguration.TenantId}/v2.0/",
                            };
                            if (customJwtEvents != null)
                            {
                                options.Events = customJwtEvents;
                            }
                        });
                }
            }

            if (entraExternalIdConfig != null)
            {
                services.AddAuthentication(DayaAuthenticationSchemeNames.EntraExternalId)
                    .AddJwtBearer(DayaAuthenticationSchemeNames.EntraExternalId, options =>
                    {
                        // Entra External ID uses a specific authority format
                        options.Authority = $"https://{entraExternalIdConfig.EntraName}.ciamlogin.com/{entraExternalIdConfig.TenantId}/v2.0";
                        options.MetadataAddress = $"https://{entraExternalIdConfig.EntraName}.ciamlogin.com/{entraExternalIdConfig.TenantId}/v2.0/.well-known/openid-configuration";

                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ClockSkew = TimeSpan.FromMinutes(5),

                            // Entra External ID specific issuer validation
                            ValidIssuer = $"https://{entraExternalIdConfig.EntraName}.ciamlogin.com/{entraExternalIdConfig.TenantId}/v2.0",
                            ValidAudiences = entraExternalIdConfig.Audiences.Split(","),
                            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                            {
                                // This will force fetching keys from the JWKS endpoint
                                var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                                    $"https://{entraExternalIdConfig.EntraName}.ciamlogin.com/{entraExternalIdConfig.TenantId}/v2.0/.well-known/openid-configuration",
                                    new OpenIdConnectConfigurationRetriever());

                                var config = configManager.GetConfigurationAsync().Result;
                                return config.SigningKeys;
                            },

                            // Name claim mapping for Entra External ID
                            NameClaimType = "name",
                            RoleClaimType = "roles"
                        };

                        if (customJwtEvents != null)
                        {
                            options.Events = customJwtEvents;
                        }
                    });
            }

            if (dayaCustomJwtConfig != null)
            {
                if (dayaCustomJwtConfig.UseFakeJwt)
                {
                    services.AddAuthentication()
                            .AddJwtBearer(DayaAuthenticationSchemeNames.CustomJwt, x =>
                            {
                                x.RequireHttpsMetadata = false;
                                x.SaveToken = true;
                                x.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuerSigningKey = true,
                                    IssuerSigningKey = new SymmetricSecurityKey(FakeJwtTokenGenerator.EncryptionKey),
                                    ValidateIssuer = false,
                                    ValidateAudience = false
                                };
                                if (customJwtEvents != null)
                                {
                                    x.Events = customJwtEvents;
                                }
                            });
                }
                else
                {
                    services.AddAuthentication()
                        .AddJwtBearer(DayaAuthenticationSchemeNames.CustomJwt, options =>
                    {
                        byte[] secretKey = Encoding.UTF8.GetBytes(dayaCustomJwtConfig.SecurityKey);
                        byte[] encryptionKey = Encoding.UTF8.GetBytes(dayaCustomJwtConfig.EncryptionKey);
                        TokenValidationParameters validationParameters = new TokenValidationParameters
                        {
                            ClockSkew = TimeSpan.Zero,
                            RequireSignedTokens = true,

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(secretKey),

                            RequireExpirationTime = true,
                            ValidateLifetime = true,

                            ValidateAudience = true,
                            ValidAudience = dayaCustomJwtConfig.Audience,

                            ValidateIssuer = true,
                            ValidIssuer = dayaCustomJwtConfig.Issuer,

                            TokenDecryptionKey = new SymmetricSecurityKey(encryptionKey)
                        };

                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        options.TokenValidationParameters = validationParameters;
                        if (customJwtEvents != null)
                        {
                            options.Events = customJwtEvents;
                        }
                    });
                }
            }

            if (b2CConfiguration?.UseFakeAdb2c == true || dayaCustomJwtConfig?.UseFakeJwt == true)
            {
                services.AddSingleton<IFakeJwtTokenGenerator, FakeJwtTokenGenerator>();
            }

            if (apiKeyStoreRegistrant != null)
            {
                apiKeyStoreRegistrant(services);
                services.AddAuthentication(DayaAuthenticationSchemeNames.ApiKey)
                    .AddScheme<DayaApiKeyAuthenticationOptions, DayaApiKeyAuthenticationHandler>(DayaApiKeyAuthenticationOptions.DefaultScheme, null);
            }

            services.AddAuthentication(DayaAuthenticationSchemeNames.Anonymous)
                .AddScheme<DayaAnonymousAuthenticationOptions, DayaAnonymousAuthenticationHandler>(DayaAnonymousAuthenticationOptions.DefaultScheme, null);
        }

        public static void AddDayaAuthorization(this IServiceCollection services, List<DayaPolicyDefinition> dayaPolicyDefinitions)
        {
            services.AddAuthorization(opts =>
            {
                foreach (var dayaPolicyDefinition in dayaPolicyDefinitions)
                {
                    opts.AddPolicy(dayaPolicyDefinition.PolicyName, policyBuilder =>
                    {
                        policyBuilder.AddAuthenticationSchemes(dayaPolicyDefinition.AuthenticationSchemeName);
                        dayaPolicyDefinition.RequirementBuilder?.Invoke(policyBuilder);
                    });
                }
                opts.AddPolicy(DayaPolicyNames.AllowAnonymous, policyBuilder =>
                {
                    policyBuilder.AddAuthenticationSchemes(DayaAnonymousAuthenticationOptions.DefaultScheme);
                    policyBuilder.RequireAuthenticatedUser();
                });
            });
        }
    }
}