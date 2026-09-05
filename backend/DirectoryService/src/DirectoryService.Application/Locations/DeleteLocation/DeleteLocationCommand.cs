using DirectoryService.Application.Common;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Locations;

public sealed record DeleteLocationCommand(Guid Id) : ICommand;
