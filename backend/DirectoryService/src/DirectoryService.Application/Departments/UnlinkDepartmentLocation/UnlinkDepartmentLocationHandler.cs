using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Application.Locations;
using DirectoryService.Domain.Common;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Departments;

public sealed class UnlinkDepartmentLocationHandler : ICommandHandler<UnlinkDepartmentLocationCommand>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;

    public UnlinkDepartmentLocationHandler(
        IDepartmentRepository departmentRepository,
        ILocationRepository locationRepository)
    {
        _departmentRepository = departmentRepository;
        _locationRepository = locationRepository;
    }

    public async Task<UnitResult<ErrorList>> Handle(UnlinkDepartmentLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var departmentResult = await _departmentRepository.GetByIdAsync(command.DepartmentId, cancellationToken);
        if (departmentResult.IsFailure)
        {
            return departmentResult.Error.ToErrorList();
        }

        var locationResult = await _locationRepository.GetByIdAsync(command.LocationId, cancellationToken);
        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrorList();
        }

        var removeLinkResult =
            await _departmentRepository.RemoveLocationLinkAsync(command.DepartmentId, command.LocationId,
                cancellationToken);
        if (removeLinkResult.IsFailure)
        {
            return removeLinkResult.Error.ToErrorList();
        }

        return UnitResult.Success<ErrorList>();
    }
}
