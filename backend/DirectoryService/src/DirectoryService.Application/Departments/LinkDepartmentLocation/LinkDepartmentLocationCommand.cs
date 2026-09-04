using DirectoryService.Application.Common;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Departments;

public sealed record LinkDepartmentLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand;
