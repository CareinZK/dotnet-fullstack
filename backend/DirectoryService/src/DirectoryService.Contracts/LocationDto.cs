namespace DirectoryService.Contracts;

public sealed record LocationDto(Guid Id, string Name, string Address);

public sealed record CreateLocationDto(string Name, string Address);

public sealed record UpdateLocationDto(string Name, string Address);