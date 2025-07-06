using MediatR;

namespace DAYA.Cloud.Framework.V2.Application.Events;

public interface IDomainNotificationRequest : IRequest, IRequest<Unit>
{
}