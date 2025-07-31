using System.Security.Claims;
using System.Text.Encodings.Web;
using DAYA.Cloud.Framework.V2.Authentication.Contracts;
using DAYA.Cloud.Framework.V2.Authentication.Contracts.ApiKeyAuthentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace DAYA.Cloud.Framework.V2.Authentication.AuthenticationClient
{
    public class DayaAuthenticationClientHandler : AuthenticationHandler<DayaAuthenticationClientOptions>
    {
        public DayaAuthenticationClientHandler(
            IOptionsMonitor<DayaAuthenticationClientOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.ContainsKey(DayaAuthenticationClientOptions.TokenHeaderName) == false &&
                Request.Headers.ContainsKey(DayaApiKeyAuthenticationOptions.TokenHeaderName) == false &&
                Request.Query.ContainsKey(DayaApiKeyAuthenticationOptions.TokenQueryParameterName) == false)
            {
                return AuthenticateResult.Fail($"Missing header: {DayaAuthenticationClientOptions.TokenHeaderName} or {DayaApiKeyAuthenticationOptions.TokenHeaderName} or Query parameter: {DayaApiKeyAuthenticationOptions.TokenQueryParameterName} ");
            }

            string? bearerToken = null;
            if (Request.Headers.TryGetValue(DayaAuthenticationClientOptions.TokenHeaderName, out var bToken))
            {
                bearerToken = bToken;
                if (string.IsNullOrWhiteSpace(bearerToken) == true)
                {
                    return AuthenticateResult.Fail($"Missing header value: {DayaAuthenticationClientOptions.TokenHeaderName}");
                }
            }

            string? apiKeyToken = null;
            if (Request.Headers.TryGetValue(DayaApiKeyAuthenticationOptions.TokenHeaderName, out var aToken))
            {
                apiKeyToken = aToken;
            }

            if (string.IsNullOrWhiteSpace(apiKeyToken) == true &&
                Request.Query.TryGetValue(DayaApiKeyAuthenticationOptions.TokenQueryParameterName, out var aqToken))
            {
                apiKeyToken = aqToken;
            }

            var endpoint = Request!.HttpContext!.GetEndpoint();
            var authorizeAttribute = endpoint?.Metadata.GetMetadata<DayaAuthorizeAttribute>();
            if (authorizeAttribute == null)
            {
                return AuthenticateResult.Fail($"Missing endpoint authorize attribute.");
            }

            using var client = new RestClient();

            if (string.IsNullOrWhiteSpace(bearerToken) == false)
            {
                client.AddDefaultHeader(DayaAuthenticationClientOptions.TokenHeaderName, bearerToken);
            }

            if (string.IsNullOrWhiteSpace(apiKeyToken) == false)
            {
                client.AddDefaultHeader(DayaApiKeyAuthenticationOptions.TokenHeaderName, apiKeyToken);
            }

            client.AddDefaultHeader(DayaAuthenticationClientOptions.ApiManagementSubscriptionKeyHeaderName, Options.AuthenticationClientConfig.SubscriptionKey);

            if (Request.Headers.TryGetValue("Origin", out var origin) && !string.IsNullOrEmpty(origin))
            {
                client.AddDefaultHeader("Origin", origin!);
            }
            if (Request.Headers.TryGetValue("tenant-id", out var tenantId) && !string.IsNullOrEmpty(tenantId))
            {
                client.AddDefaultHeader("tenant-id", tenantId!);
            }

            var restRequest = new RestRequest(Options.AuthenticationClientConfig.InternalAuthenticationUrl, Method.Post);

            restRequest.AddBody(new
            {
                authorizeAttribute.Policy,
                authorizeAttribute.Permission,
            });

            var response = await client.ExecuteAsync(restRequest);
            if (response is not { IsSuccessful: true })
            {
                Logger.LogInformation($"Internal Authentication Failed. {response.StatusCode}");
                return AuthenticateResult.Fail("Internal Authentication Failed.");
            }

            List<Claim>? claims = null;

            if (string.IsNullOrWhiteSpace(response.Content) == false)
            {
                var claimModel = JsonConvert.DeserializeObject<DayaInternalAuthenticationModel>(response.Content);
                if (claimModel?.Claims is { Count: > 0 })
                {
                    claims = claimModel.Claims.Select(x => new Claim(x.Type, x.Value)).ToList();
                    claims.Add(new Claim("IsAuthorized", claimModel.IsAuthorized.ToString()));
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
        }
    }
}