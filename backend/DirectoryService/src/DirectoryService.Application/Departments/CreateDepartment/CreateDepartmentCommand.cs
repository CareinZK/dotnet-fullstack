using DirectoryService.Application.Common;
using DirectoryService.Contracts;

namespace DirectoryService.Application.Departments;

public sealed record CreateDepartmentCommand(
    string Name,
    string Slug,
    Guid? ParentId,
    List<Guid>? LocationIds) : ICommand<DepartmentDto>;
