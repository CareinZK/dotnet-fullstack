using DirectoryService.Application.Common;

namespace DirectoryService.Application.Departments;

public sealed record UpdateDepartmentCommand(Guid Id, string Name) : ICommand;
