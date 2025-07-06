using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

internal static class SwaggerUiExtension
{
    public const string PublicApiGroup = "public";
    public const string InternalApiGroup = "Internal";
    public const string ExternalApiGroup = "External";
    public const string DeveloperApiGroup = "developer";

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(DeveloperApiGroup, new OpenApiInfo
            {
                Title = "DAYA SERVICE DEVELOPER API",
                Version = DeveloperApiGroup,
                Description = "DAYASOFT SERVICE DEVELOPER API",
                Contact = new OpenApiContact
                {
                    Name = "DAYA SOFT Development department",
                    Email = "support@dayasoft.com"
                },
                License = new OpenApiLicense
                {
                    Name = "(c) Puls Cloud GmbH 2024, All Right Reserved"
                }
            });

            c.SwaggerDoc(ExternalApiGroup, new OpenApiInfo
            {
                Title = "DAYA SERVICE EXTERNAL API",
                Version = ExternalApiGroup,
                Description = "DAYASOFT SERVICE EXTERNAL API",
                Contact = new OpenApiContact
                {
                    Name = "DAYA SOFT Development department",
                    Email = "support@dayasoft.com"
                },
                License = new OpenApiLicense
                {
                    Name = "(c) Puls Cloud GmbH 2024, All Right Reserved"
                }
            });

            c.SwaggerDoc(PublicApiGroup, new OpenApiInfo
            {
                Title = "DAYA SERVICE PUBLIC API",
                Version = PublicApiGroup,
                Description = "DAYASOFT SERVICE PUBLIC API",
                Contact = new OpenApiContact
                {
                    Name = "DAYA SOFT Development department",
                    Email = "support@dayasoft.com"
                },
                License = new OpenApiLicense
                {
                    Name = "(c) Puls Cloud GmbH 2024, All Right Reserved"
                }
            });
            c.ExampleFilters();
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
            });

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
        services.AddSwaggerGenNewtonsoftSupport();

        // TODO: difference?
        services.AddSwaggerExamplesFromAssemblyOf(typeof(SwaggerUiExtension));
        services.AddControllersWithViews(options => options.Conventions.Add(new SwaggerFileMapperConvention()));
        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger(c =>
        {
            //c.RouteTemplate = "swagger/docs/{documentName}";
            //c.SerializeAsV2 = true;
            c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                swaggerDoc.Servers = new List<OpenApiServer>
                {
                        new OpenApiServer { Url = $"https://{httpReq.Host.Value}" }
                };
            });
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"/swagger/{PublicApiGroup}/swagger.json", "PUBLIC APIs");
            c.SwaggerEndpoint($"/swagger/{ExternalApiGroup}/swagger.json", "EXTERNAL APIs");
            c.SwaggerEndpoint($"/swagger/{DeveloperApiGroup}/swagger.json", "DEVELOPER APIs");
        });

        return app;
    }

    public class SwaggerFileMapperConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            string groupName = (controller.Attributes.FirstOrDefault(x => x is ApiExplorerSettingsAttribute) as ApiExplorerSettingsAttribute)?.GroupName;
            controller.ApiExplorer.GroupName = groupName ?? InternalApiGroup;
        }
    }
}