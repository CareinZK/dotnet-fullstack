using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Domain.Common;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Locations;

public sealed class DeleteLocationHandler : ICommandHandler<DeleteLocationCommand>
{
    private readonly ILocationRepository _locationRepository;

    public DeleteLocationHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<UnitResult<ErrorList>> Handle(DeleteLocationCommand command, CancellationToken cancellationToken = default)
    {
        var deleteResult = await _locationRepository.DeleteAsync(command.Id, cancellationToken);
        if (deleteResult.IsFailure)
        {
            return deleteResult.Error.ToErrorList();
        }

        return UnitResult.Success<ErrorList>();
    }
}
