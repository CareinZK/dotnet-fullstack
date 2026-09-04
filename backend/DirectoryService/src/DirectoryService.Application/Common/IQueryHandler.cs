using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common;

namespace DirectoryService.Application.Common;

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse, ErrorList>> Handle(TQuery query, CancellationToken cancellationToken = default);
}
