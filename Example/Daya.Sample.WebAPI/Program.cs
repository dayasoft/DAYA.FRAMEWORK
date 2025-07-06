using System.Reflection;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Daya.Sample.WebAPI.Configuration.Logging;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.Application.InternalCommands;
using DAYA.Cloud.Framework.V2.Application.Outbox;
using DAYA.Cloud.Framework.V2.Cosmos;
using DAYA.Cloud.Framework.V2.Infrastructure;
using DAYA.Cloud.Framework.V2.Infrastructure.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
{
    TenantId = "55a6a626-be1a-4c04-b1ae-767bec31edd5",
    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
    ManagedIdentityClientId = "9ff5b77c-be82-4967-94a1-8e752ed1b604",
});

builder.Services.AddAzureClients(
    clientBuilder =>
    {
        clientBuilder.UseCredential(credential);
        clientBuilder.AddServiceBusClientWithNamespace("sb-passwordless-sdkui0.servicebus.windows.net");
    }
);

builder.Services.AddSingleton<CosmosClient>(serviceProvider =>
{
    var jsonSerializerSetting = new JsonSerializerSettings
    {
        ContractResolver = new PrivateSetterContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        Formatting = Formatting.Indented
    };

    var cosmosClientOptions = new CosmosClientOptions
    {
        Serializer = new CosmosJsonDotNetSerializer(jsonSerializerSetting)
    };

    return new CosmosClient("https://pj-dev-ms-cosmosdb.documents.azure.com:443/", "ZZJLGIEVqtAwyplILNpmurpox57yElsko0nDveBr4f7hwb2E6JRftnIZmJ8fmtdWrVtHJrxll0xjlmkccoBMZg==", cosmosClientOptions);
});

builder.Services.AddSingleton(new ServiceBusClient("sb-passwordless-sdkui0.servicebus.windows.net", credential));

builder.Services.AddSingleton<Database>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var cosmosClient = sp.GetRequiredService<CosmosClient>();
    var databaseName = configuration["CosmosDb:DatabaseName"] ?? "SampleDatabase";
    var database = cosmosClient.GetDatabase(databaseName);

    // Optionally, you can test the connection here synchronously, but ReadAsync is async. For now,
    // just return the database instance.
    return database;
});

// Add Logger
builder.Services.AddLogging(builder.Environment);

// Add Scope Accessor
builder.Services.AddScopeAccessors();

var infrastructureAssembly = Assembly.Load("Daya.Sample.Infrastructure");
var applicationAssembly = Assembly.Load("Daya.Sample.Application");

builder.Services.AddSingleton<IServiceModule, ServiceModule>();

// Add DAYA Framework processing services
builder.Services.AddDayaMediator(infrastructureAssembly, applicationAssembly);
builder.Services.AddDayaAuthentication(builder.Configuration);
builder.Services.AddDayaAuthorization();

builder.Services.AddSingleton<IApplicationAssemblyResolver>(new ApplicationAssemblyResolver(applicationAssembly));

builder.Services.AddHostedService<OutboxMessageBackgroundService>();
builder.Services.AddHostedService<InternalCommandMessageBackgroundService>();
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