using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Application.Locations;
using DirectoryService.Domain.Common;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Departments;

public sealed class LinkDepartmentLocationHandler : ICommandHandler<LinkDepartmentLocationCommand>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;

    public LinkDepartmentLocationHandler(
        IDepartmentRepository departmentRepository,
        ILocationRepository locationRepository)
    {
        _departmentRepository = departmentRepository;
        _locationRepository = locationRepository;
    }

    public async Task<UnitResult<ErrorList>> Handle(LinkDepartmentLocationCommand command, CancellationToken cancellationToken = default)
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

        var linkExistsResult = await _departmentRepository.LocationLinkExistsAsync(command.DepartmentId, command.LocationId, cancellationToken);
        if (linkExistsResult.IsFailure)
        {
            return linkExistsResult.Error.ToErrorList();
        }

        if (linkExistsResult.Value)
        {
            return Errors.Department.LocationAlreadyLinked(command.DepartmentId, command.LocationId).ToErrorList();
        }

        var addLinkResult = await _departmentRepository.AddLocationLinkAsync(command.DepartmentId, command.LocationId, cancellationToken);
        if (addLinkResult.IsFailure)
        {
            return addLinkResult.Error.ToErrorList();
        }

        return UnitResult.Success<ErrorList>();
    }
}
