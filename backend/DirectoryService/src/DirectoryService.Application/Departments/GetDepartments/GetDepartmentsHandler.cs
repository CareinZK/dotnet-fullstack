using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Contracts;
using DirectoryService.Domain.Common;
using DirectoryService.Domain.Departments;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Departments;

public sealed class GetDepartmentsHandler : IQueryHandler<GetDepartmentsQuery, IReadOnlyList<DepartmentDto>>
{
    private readonly IDepartmentRepository _departmentRepository;

    public GetDepartmentsHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<Result<IReadOnlyList<DepartmentDto>, ErrorList>> Handle(GetDepartmentsQuery query,
        CancellationToken cancellationToken = default)
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

    private static DepartmentDto Map(Department department, IReadOnlyCollection<Guid>? locationIds) =>
        new(department.Id, department.Name, department.Slug, department.Path, department.ParentId,
            locationIds?.ToList());
}
