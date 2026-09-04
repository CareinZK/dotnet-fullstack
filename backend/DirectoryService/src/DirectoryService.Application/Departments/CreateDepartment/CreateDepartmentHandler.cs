using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Application.Locations;
using DirectoryService.Contracts;
using DirectoryService.Domain.Common;
using DirectoryService.Domain.Departments;
using FluentValidation;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Departments;

public sealed class CreateDepartmentHandler : ICommandHandler<CreateDepartmentCommand, DepartmentDto>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IValidator<CreateDepartmentCommand> _validator;

    public CreateDepartmentHandler(
        IDepartmentRepository departmentRepository,
        ILocationRepository locationRepository,
        IValidator<CreateDepartmentCommand> validator)
    {
        _departmentRepository = departmentRepository;
        _locationRepository = locationRepository;
        _validator = validator;
    }

    public async Task<Result<DepartmentDto, ErrorList>> Handle(CreateDepartmentCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        var nameExistsResult = await _departmentRepository.NameExistsAsync(command.Name, cancellationToken);
        if (nameExistsResult.IsFailure)
        {
            return nameExistsResult.Error.ToErrorList();
        }

        if (nameExistsResult.Value)
        {
            return Errors.Department.AlreadyExists(command.Name).ToErrorList();
        }

        Department? parentDepartment = null;
        if (command.ParentId.HasValue)
        {
            var parentResult = await _departmentRepository.GetByIdAsync(command.ParentId.Value, cancellationToken);
            if (parentResult.IsFailure)
            {
                return Errors.Department.ParentNotFound(command.ParentId.Value).ToErrorList();
            }

            parentDepartment = parentResult.Value;
        }

        var locationIds = command.LocationIds?.Distinct().ToList() ?? [];
        foreach (var locationId in locationIds)
        {
            var locResult = await _locationRepository.GetByIdAsync(locationId, cancellationToken);
            if (locResult.IsFailure)
            {
                return Errors.Location.NotFound(locationId).ToErrorList();
            }
        }

        var deptResult = Department.Create(Guid.NewGuid(), command.Name, command.Slug, parentDepartment);
        if (deptResult.IsFailure)
        {
            return deptResult.Error.ToErrorList();
        }

        var department = deptResult.Value;
        var addResult = await _departmentRepository.AddAsync(department, locationIds, cancellationToken);
        if (addResult.IsFailure)
        {
            return addResult.Error.ToErrorList();
        }

        var dto = new DepartmentDto(
            department.Id,
            department.Name,
            department.Slug,
            department.Path,
            department.ParentId,
            locationIds);

        return Result.Success<DepartmentDto, ErrorList>(dto);
    }

    public async Task<Result<Guid, ErrorList>> ExecuteAsync(CreateDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var command = new CreateDepartmentCommand(dto.Name, dto.Slug, dto.ParentId, dto.LocationIds);
        var result = await Handle(command, cancellationToken);
        return result.Map(dept => dept.Id);
    }
}

public sealed class CreateDepartment
{
    private readonly CreateDepartmentHandler _handler;

    public CreateDepartment(
        IDepartmentRepository departmentRepository,
        ILocationRepository locationRepository,
        IValidator<CreateDepartmentDto> validator)
    {
        _ = validator;
        _handler = new CreateDepartmentHandler(departmentRepository, locationRepository, new CreateDepartmentCommandValidator());
    }

    public Task<Result<Guid, ErrorList>> ExecuteAsync(CreateDepartmentDto dto, CancellationToken cancellationToken = default) =>
        _handler.ExecuteAsync(dto, cancellationToken);
}
