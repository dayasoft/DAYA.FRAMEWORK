using Azure.Identity;
using Microsoft.Graph;

namespace DAYA.Cloud.Framework.V2.MicrosoftGraph
{
	public class GraphClientFactory : IGraphClientFactory
	{
		private readonly string _appId;
		private readonly string _tenantId;
		private readonly string _clientSecret;

		private GraphServiceClient _graphServiceClient;

		public GraphClientFactory(string appId, string tenantId, string clientSecret)
		{
			_appId = appId;
			_tenantId = tenantId;
			_clientSecret = clientSecret;

			_graphServiceClient = Get();
		}

		public GraphServiceClient Get()
		{
			if (_graphServiceClient != null)
			{
				return _graphServiceClient;
			}
			var clientSecretCredential = new ClientSecretCredential(_tenantId, _appId, _clientSecret);

			_graphServiceClient = new GraphServiceClient(clientSecretCredential);
			return _graphServiceClient;
		}
	}
}