using DAYA.Cloud.Framework.V2.Application.Contracts;
using MediatR;

namespace DAYA.Cloud.Framework.V2.Application.Configuration.Queries;

public interface IQueryHandler<in TQuery, TResult> :
      IRequestHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
}
