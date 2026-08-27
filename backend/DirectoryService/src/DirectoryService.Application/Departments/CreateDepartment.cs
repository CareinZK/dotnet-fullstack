using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Application.Locations;
using DirectoryService.Contracts;
using DirectoryService.Domain.Common;
using DirectoryService.Domain.Departments;
using FluentValidation;

namespace DirectoryService.Application.Departments;

public sealed class CreateDepartment
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IValidator<CreateDepartmentDto> _validator;

    public CreateDepartment(
        IDepartmentRepository departmentRepository,
        ILocationRepository locationRepository,
        IValidator<CreateDepartmentDto> validator)
    {
        _departmentRepository = departmentRepository;
        _locationRepository = locationRepository;
        _validator = validator;
    }

    public async Task<Result<Guid, ErrorList>> ExecuteAsync(CreateDepartmentDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        var nameExistsResult = await _departmentRepository.NameExistsAsync(dto.Name, cancellationToken);
        if (nameExistsResult.IsFailure)
        {
            return nameExistsResult.Error.ToErrorList();
        }

        if (nameExistsResult.Value)
        {
            return Errors.Department.AlreadyExists(dto.Name).ToErrorList();
        }

        Department? parentDepartment = null;
        if (dto.ParentId.HasValue)
        {
            var parentResult = await _departmentRepository.GetByIdAsync(dto.ParentId.Value, cancellationToken);
            if (parentResult.IsFailure)
            {
                return Errors.Department.ParentNotFound(dto.ParentId.Value).ToErrorList();
            }

            parentDepartment = parentResult.Value;
        }

        var locationIds = dto.LocationIds?.Distinct().ToList() ?? [];
        foreach (var locationId in locationIds)
        {
            var locResult = await _locationRepository.GetByIdAsync(locationId, cancellationToken);
            if (locResult.IsFailure)
            {
                return Errors.Location.NotFound(locationId).ToErrorList();
            }
        }

        var deptResult = Department.Create(Guid.NewGuid(), dto.Name, dto.Slug, parentDepartment);
        if (deptResult.IsFailure)
        {
            return deptResult.Error.ToErrorList();
        }

        var addResult = await _departmentRepository.AddAsync(deptResult.Value, locationIds, cancellationToken);
        if (addResult.IsFailure)
        {
            return addResult.Error.ToErrorList();
        }

        return Result.Success<Guid, ErrorList>(deptResult.Value.Id);
    }
}
