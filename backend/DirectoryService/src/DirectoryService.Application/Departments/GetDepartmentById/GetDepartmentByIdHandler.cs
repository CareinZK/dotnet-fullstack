using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Contracts;
using DirectoryService.Domain.Common;
using DirectoryService.Domain.Departments;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Departments;

public sealed class GetDepartmentByIdHandler : IQueryHandler<GetDepartmentByIdQuery, DepartmentDto>
{
    private readonly IDepartmentRepository _departmentRepository;

    public GetDepartmentByIdHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<Result<DepartmentDto, ErrorList>> Handle(GetDepartmentByIdQuery query, CancellationToken cancellationToken = default)
    {
        var departmentResult = await _departmentRepository.GetByIdAsync(query.Id, cancellationToken);
        if (departmentResult.IsFailure)
        {
            return departmentResult.Error.ToErrorList();
        }

        return Map(departmentResult.Value, []);
    }

    private static DepartmentDto Map(Department department, IReadOnlyCollection<Guid>? locationIds) =>
        new(department.Id, department.Name, department.Slug, department.Path, department.ParentId, locationIds?.ToList());
}
