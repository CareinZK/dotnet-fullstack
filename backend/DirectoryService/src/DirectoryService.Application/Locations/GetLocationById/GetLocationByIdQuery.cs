using DirectoryService.Application.Common;
using DirectoryService.Contracts;

namespace DirectoryService.Application.Locations;

public sealed record GetLocationByIdQuery(Guid Id) : IQuery<LocationDto>;
