using System.Reflection;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration;

public interface IApplicationAssemblyResolver
{
    Assembly Resolve();
}