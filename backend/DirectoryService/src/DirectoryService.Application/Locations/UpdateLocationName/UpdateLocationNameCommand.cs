using DirectoryService.Application.Common;

namespace DirectoryService.Application.Locations;

public sealed record UpdateLocationNameCommand(Guid Id, string Name) : ICommand<Guid>;
