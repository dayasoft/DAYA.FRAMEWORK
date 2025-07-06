using MediatR;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration.Processing.Outbox;

public record ProcessOutboxCommand(string MessageId) : IRequest;