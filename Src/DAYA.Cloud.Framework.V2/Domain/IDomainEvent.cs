using System;
using MediatR;

namespace DAYA.Cloud.Framework.V2.Domain;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
    string AggregateId { get; }
}