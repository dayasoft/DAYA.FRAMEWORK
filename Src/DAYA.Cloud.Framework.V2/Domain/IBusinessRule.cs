using System.Threading.Tasks;

namespace DAYA.Cloud.Framework.V2.Domain;

public interface IBusinessRule
{
    Task<bool> IsBrokenAsync();

    string Message { get; }
}
