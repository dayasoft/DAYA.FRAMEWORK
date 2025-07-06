using DAYA.Cloud.Framework.V2.Application.Contracts;
using MediatR;

namespace DAYA.Cloud.Framework.V2.Application.Configuration.Queries;

public interface IPageableQueryHandler<in TQuery, TResult> :
      IRequestHandler<TQuery, PagedDto<TResult>> where TQuery : IQuery<PagedDto<TResult>>
{
}
