using DirectoryService.Application.Common;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Locations;

public sealed record CreateLocationCommand(string Name, string Address) : ICommand<Guid>;
