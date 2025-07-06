using System.Threading;
using System.Threading.Tasks;

namespace DAYA.Cloud.Framework.V2.Domain;

internal interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
