using DirectoryService.Application.Common;
using DirectoryService.Contracts;

namespace DirectoryService.Application.Departments;

public sealed record GetDepartmentByIdQuery(Guid Id) : IQuery<DepartmentDto>;
