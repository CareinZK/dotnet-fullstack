using DirectoryService.Application.Common;

namespace DirectoryService.Application.Locations;

public sealed record UpdateLocationCommand(Guid Id, string Name, string Address) : ICommand;
