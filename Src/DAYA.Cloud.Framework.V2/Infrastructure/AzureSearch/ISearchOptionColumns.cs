using DAYA.Cloud.Framework.V2.Application.Contracts;

namespace DAYA.Cloud.Framework.V2.Infrastructure.AzureSearch
{
    public interface ISearchOptionColumns<TRequest, out TResult>
        where TRequest : DownloadableQuery<TResult>
        where TResult : DownloadSearchDto
    {
        string[] GetFields();

        string GetFileName();

        string GetFileType();
    }
}