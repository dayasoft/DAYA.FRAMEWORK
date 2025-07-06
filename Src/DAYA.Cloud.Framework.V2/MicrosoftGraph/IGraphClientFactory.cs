using Microsoft.Graph;

namespace DAYA.Cloud.Framework.V2.MicrosoftGraph
{
    public interface IGraphClientFactory
    {
        GraphServiceClient Get();
    }
}