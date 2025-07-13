namespace DAYA.Cloud.Framework.V2.MicrosoftGraph;

public class GraphParameterResolver : IGraphParameterResolver
{
    private readonly string _issuer;
    private readonly string _extensionClientId;

    public GraphParameterResolver(string issuer, string extensionClientId)
    {
        _issuer = issuer;
        _extensionClientId = extensionClientId;
    }

    public string Issuer => _issuer;
    public string ExtensionClientId => _extensionClientId;
}