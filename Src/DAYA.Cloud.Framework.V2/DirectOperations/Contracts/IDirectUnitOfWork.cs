using System.Threading;
using System.Threading.Tasks;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Contracts;

public interface IDirectUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}