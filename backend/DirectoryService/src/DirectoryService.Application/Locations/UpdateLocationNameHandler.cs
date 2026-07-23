using CSharpFunctionalExtensions;
using DirectoryService.Contracts;

namespace DirectoryService.Application.Locations;

public sealed class UpdateLocationNameHandler
{
    private readonly ILocationRepository _repository;

    public UpdateLocationNameHandler(ILocationRepository locationRepository)
    {
        _repository = locationRepository;
    }

    public async Task<Result<Guid>> Handle(UpdateLocationNameRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _repository.UpdateLocationNameAsync(request.Id, request.Name, cancellationToken);
            return Result.Success<Guid>(request.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>($"Failed to update location name: {ex.Message}");
        }
    }
}