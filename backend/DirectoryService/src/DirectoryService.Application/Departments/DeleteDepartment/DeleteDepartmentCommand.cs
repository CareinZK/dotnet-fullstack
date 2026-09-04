using DirectoryService.Application.Common;

namespace DirectoryService.Application.Departments;

public sealed record DeleteDepartmentCommand(Guid Id) : ICommand;
