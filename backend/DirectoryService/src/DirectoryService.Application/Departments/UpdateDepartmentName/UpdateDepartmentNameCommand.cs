using DirectoryService.Application.Common;

namespace DirectoryService.Application.Departments;

public sealed record UpdateDepartmentNameCommand(Guid Id, string Name) : ICommand<Guid>;
