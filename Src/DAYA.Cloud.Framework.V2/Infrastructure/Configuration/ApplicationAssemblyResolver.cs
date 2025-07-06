using System.Reflection;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration;

public class ApplicationAssemblyResolver : IApplicationAssemblyResolver
{
    private readonly Assembly _applicationAssembly;

    public ApplicationAssemblyResolver(Assembly applicationAssembly)
    {
        _applicationAssembly = applicationAssembly;
    }

    public Assembly Resolve() => _applicationAssembly;
}