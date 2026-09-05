using DirectoryService.Application.Common;

namespace DirectoryService.Application.Departments;

public sealed record UnlinkDepartmentLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand;
