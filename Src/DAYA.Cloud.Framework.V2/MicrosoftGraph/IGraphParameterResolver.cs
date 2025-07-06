namespace DAYA.Cloud.Framework.V2.MicrosoftGraph
{
    public interface IGraphParameterResolver
    {
        string Issuer { get; }
        string ExtensionClientId { get; }
    }
}