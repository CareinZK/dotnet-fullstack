using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Application.Locations;
using DirectoryService.Contracts;
using DirectoryService.Domain.Common;
using DirectoryService.Domain.Departments;
using FluentValidation;

namespace DirectoryService.Application.Departments;

public sealed class DepartmentsService : IDepartmentsService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IValidator<CreateDepartmentDto> _createValidator;
    private readonly IValidator<UpdateDepartmentDto> _updateValidator;

    public DepartmentsService(
        IDepartmentRepository departmentRepository,
        ILocationRepository locationRepository,
        IValidator<CreateDepartmentDto> createValidator,
        IValidator<UpdateDepartmentDto> updateValidator)
    {
        _departmentRepository = departmentRepository;
        _locationRepository = locationRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<DepartmentDto, ErrorList>> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
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

        var department = deptResult.Value;
        var addResult = await _departmentRepository.AddAsync(department, locationIds, cancellationToken);
        if (addResult.IsFailure)
        {
            return addResult.Error.ToErrorList();
        }

        return Map(department, locationIds);
    }

    public async Task<Result<IReadOnlyList<DepartmentDto>, ErrorList>> GetAllAsync(CancellationToken cancellationToken)
    {
        var departmentsResult = await _departmentRepository.GetAllAsync(cancellationToken);
        if (departmentsResult.IsFailure)
        {
            return departmentsResult.Error.ToErrorList();
        }

        IReadOnlyList<DepartmentDto> dtos = departmentsResult.Value
            .Select(department => Map(department, null))
            .ToList();

        return Result.Success<IReadOnlyList<DepartmentDto>, ErrorList>(dtos);
    }

    public async Task<Result<DepartmentDto, ErrorList>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var departmentResult = await _departmentRepository.GetByIdAsync(id, cancellationToken);
        if (departmentResult.IsFailure)
        {
            return departmentResult.Error.ToErrorList();
        }

        return Map(departmentResult.Value, []);
    }

    public async Task<UnitResult<ErrorList>> UpdateAsync(Guid id, UpdateDepartmentDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        var departmentResult = await _departmentRepository.GetByIdAsync(id, cancellationToken);
        if (departmentResult.IsFailure)
        {
            return departmentResult.Error.ToErrorList();
        }

        var department = departmentResult.Value;
        var changeNameResult = department.ChangeName(dto.Name);
        if (changeNameResult.IsFailure)
        {
            return changeNameResult.Error.ToErrorList();
        }

        var updateRepoResult = await _departmentRepository.UpdateAsync(department, cancellationToken);
        if (updateRepoResult.IsFailure)
        {
            return updateRepoResult.Error.ToErrorList();
        }

        return UnitResult.Success<ErrorList>();
    }

    public async Task<UnitResult<ErrorList>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleteResult = await _departmentRepository.DeleteAsync(id, cancellationToken);
        if (deleteResult.IsFailure)
        {
            return deleteResult.Error.ToErrorList();
        }

        return UnitResult.Success<ErrorList>();
    }

    public async Task<UnitResult<ErrorList>> LinkLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        var ensureResult = await EnsureDepartmentAndLocationExistAsync(departmentId, locationId, cancellationToken);
        if (ensureResult.IsFailure)
        {
            return ensureResult.Error;
        }

        var linkExistsResult = await _departmentRepository.LocationLinkExistsAsync(departmentId, locationId, cancellationToken);
        if (linkExistsResult.IsFailure)
        {
            return linkExistsResult.Error.ToErrorList();
        }

        if (linkExistsResult.Value)
        {
            return Errors.Department.LocationAlreadyLinked(departmentId, locationId).ToErrorList();
        }

        var addLinkResult = await _departmentRepository.AddLocationLinkAsync(departmentId, locationId, cancellationToken);
        if (addLinkResult.IsFailure)
        {
            return addLinkResult.Error.ToErrorList();
        }

        return UnitResult.Success<ErrorList>();
    }

    public async Task<UnitResult<ErrorList>> UnlinkLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        var ensureResult = await EnsureDepartmentAndLocationExistAsync(departmentId, locationId, cancellationToken);
        if (ensureResult.IsFailure)
        {
            return ensureResult.Error;
        }

        var removeLinkResult = await _departmentRepository.RemoveLocationLinkAsync(departmentId, locationId, cancellationToken);
        if (removeLinkResult.IsFailure)
        {
            return removeLinkResult.Error.ToErrorList();
        }

        return UnitResult.Success<ErrorList>();
    }

    private async Task<UnitResult<ErrorList>> EnsureDepartmentAndLocationExistAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        var departmentResult = await _departmentRepository.GetByIdAsync(departmentId, cancellationToken);
        if (departmentResult.IsFailure)
        {
            return departmentResult.Error.ToErrorList();
        }

        var locationResult = await _locationRepository.GetByIdAsync(locationId, cancellationToken);
        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrorList();
        }

        return UnitResult.Success<ErrorList>();
    }

    private static DepartmentDto Map(Department department, IReadOnlyCollection<Guid>? locationIds)
    {
        return new DepartmentDto(
            department.Id,
            department.Name,
            department.Slug,
            department.Path,
            department.ParentId,
            locationIds?.ToList());
    }
}
