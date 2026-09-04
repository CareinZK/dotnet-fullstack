namespace DirectoryService.Contracts;

public sealed record DepartmentDto(Guid Id, string Name, string Slug, string Path, Guid? ParentId, List<Guid>? LocationIds);

public sealed record CreateDepartmentDto(string Name, string Slug, Guid? ParentId, List<Guid>? LocationIds);

public sealed record UpdateDepartmentDto(string Name);
