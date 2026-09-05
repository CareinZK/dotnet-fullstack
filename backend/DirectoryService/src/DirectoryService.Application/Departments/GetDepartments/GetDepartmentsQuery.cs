using DirectoryService.Application.Common;
using DirectoryService.Contracts;

namespace DirectoryService.Application.Departments;

public sealed record GetDepartmentsQuery : IQuery<IReadOnlyList<DepartmentDto>>;
