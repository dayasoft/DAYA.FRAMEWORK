using System.Reflection;
using Azure.Identity;
using Daya.Sample.API.Configuration.Keyvault;
using Daya.Sample.API.Configuration.Logging;
using Daya.Sample.WebAPI.Configuration.CosmosDatabase;
using Daya.Sample.WebAPI.Configuration.Scope;
using Daya.Sample.WebAPI.Configuration.ServiceBus;
using DAYA.Cloud.Framework.V2.Infrastructure.Configuration;
using DAYA.Cloud.Framework.V2.MicrosoftGraph;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

IHostEnvironment environment = builder.Environment;

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
if (environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddEnvironmentVariables();

ConfigureAzureServices(builder);

builder.Services.AddLogging(builder.Environment);
builder.Services.AddScopeAccessors();

var infrastructureAssembly = Assembly.Load("Daya.Sample.Infrastructure");
var applicationAssembly = Assembly.Load("Daya.Sample.Application");

// Add DAYA Framework processing services
builder.Services.AddDayaMediator(builder.Configuration, infrastructureAssembly, applicationAssembly);
builder.Services.AddDayaHostedServices();

builder.Services.AddDayaAuthentication(builder.Configuration);
builder.Services.AddDayaAuthorization();

builder.Services.AddSingleton<IGraphClientFactory>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var appId = configuration["EntraExternalGraph:ClientId"];
    var tenantId = configuration["EntraExternalGraph:TenantId"];
    var clientSecret = configuration["EntraExternalGraph:ClientSecret"];
    return new GraphClientFactory(appId, tenantId, clientSecret);
});

builder.Services.AddControllers();

builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Set the service provider for the ServiceCompositionRoot
ServiceCompositionRoot.SetServiceProvider(app.Services);

app.UseSwaggerDocumentation();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureAzureServices(WebApplicationBuilder builder)
{
    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
    {
        TenantId = builder.Configuration.GetValue<string>("AzureServices:TenantId"),
        AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        ManagedIdentityClientId = builder.Configuration.GetValue<string>("AzureServices:ManagedIdentityClientId"),
        ExcludeAzureCliCredential = !environment.IsDevelopment(),
        ExcludeManagedIdentityCredential = environment.IsDevelopment()
    });

    // Configure Key Vault
    var keyVaultUri = builder.Configuration["KeyVaultUri"];
    if (!string.IsNullOrEmpty(keyVaultUri))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            credential);
    }

    builder.Services.AddAzureClients(
        clientBuilder =>
        {
            clientBuilder.UseCredential(credential);
        }
    );

    // Register KeyVault service
    builder.Services.AddSingleton<KeyVaultService>();

    // Register Azure services
    builder.Services.AddServiceBus(builder.Configuration);

    // Register Cosmos DB service
    builder.Services.AddCosmosDb(builder.Configuration, credential);
}