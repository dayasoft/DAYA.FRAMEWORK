using System.Reflection;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Daya.Sample.API.Configuration.Logging;
using DAYA.Cloud.Framework.V2.Cosmos;
using DAYA.Cloud.Framework.V2.Infrastructure.Configuration;
using DAYA.Cloud.Framework.V2.MicrosoftGraph;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

IHostEnvironment environment = builder.Environment;

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
if (environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddEnvironmentVariables();

var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
{
    TenantId = builder.Configuration.GetValue<string>("AzureServices:TenantId"),
    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
    ManagedIdentityClientId = builder.Configuration.GetValue<string>("AzureServices:ManagedIdentityClientId"),
});

builder.Services.AddAzureClients(
    clientBuilder =>
    {
        clientBuilder.UseCredential(credential);
        //clientBuilder.AddServiceBusClientWithNamespace(builder.Configuration.GetValue<string>("ServiceBus:NameSpace"));
    }
);
builder.Services.AddSingleton(new ServiceBusClient("Endpoint=sb://apc-dev-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=BXl6Bw42hFohpKjN66i62BicNG9gB6vMg+ASbMGx04E="));
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

    return new CosmosClient("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", cosmosClientOptions);
});

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

// Add DAYA Framework processing services
builder.Services.AddDayaMediator(builder.Configuration, infrastructureAssembly, applicationAssembly);
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