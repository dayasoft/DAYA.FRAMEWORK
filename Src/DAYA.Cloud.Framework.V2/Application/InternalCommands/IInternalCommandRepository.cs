using System.Threading.Tasks;

namespace DAYA.Cloud.Framework.V2.Application.InternalCommands;

public interface IInternalCommandRepository
{
    Task AddAsync(InternalCommandMessage message);
}