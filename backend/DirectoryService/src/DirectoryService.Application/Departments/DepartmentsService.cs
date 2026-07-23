using System.Linq;
using DirectoryService.Application.Locations;
using DirectoryService.Contracts;
using DirectoryService.Domain.Departments;

namespace DirectoryService.Application.Departments;

public sealed class DepartmentsService : IDepartmentsService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;

    public DepartmentsService(
        IDepartmentRepository departmentRepository,
        ILocationRepository locationRepository)
    {
        _departmentRepository = departmentRepository;
        _locationRepository = locationRepository;
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken)
    {
        Department? parentDepartment = null;

        if (dto.ParentId.HasValue)
        {
            parentDepartment = await _departmentRepository.GetByIdAsync(dto.ParentId.Value, cancellationToken);
            if (parentDepartment is null)
            {
                throw new InvalidOperationException($"Parent department with id '{dto.ParentId}' was not found.");
            }
        }

        var locationIds = dto.LocationIds?.Distinct().ToList() ?? [];

        foreach (var locationId in locationIds)
        {
            var location = await _locationRepository.GetByIdAsync(locationId, cancellationToken);
            if (location is null)
            {
                throw new InvalidOperationException($"Location with id '{locationId}' was not found.");
            }
        }

        var department = Department.Create(Guid.NewGuid(), dto.Name, dto.Slug, parentDepartment);

        await _departmentRepository.AddAsync(department, locationIds, cancellationToken);

        return Map(department, locationIds);
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var departments = await _departmentRepository.GetAllAsync(cancellationToken);
        return departments.Select(department => Map(department, null)).ToList();
    }

    public async Task<DepartmentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByIdAsync(id, cancellationToken);
        return department is null ? null : Map(department, []);
    }

    public Task<bool> UpdateAsync(Guid id, UpdateDepartmentDto dto, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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
