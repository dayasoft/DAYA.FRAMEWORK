using DAYA.Cloud.Framework.V2.Domain;
using MediatR;

namespace DAYA.Cloud.Framework.V2.Application.Configuration.Notifications;

public interface IDomainNotificationHandler<T> : INotificationHandler<T> where T : IDomainEvent
{
}