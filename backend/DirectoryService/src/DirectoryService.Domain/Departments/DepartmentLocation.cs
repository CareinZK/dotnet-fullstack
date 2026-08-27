using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common;

namespace DirectoryService.Domain.Departments;

public class DepartmentLocation
{
    private DepartmentLocation()
    {
    }

    private DepartmentLocation(Guid id, Guid departmentId, Guid locationId, bool isPrimaryLocation)
    {
        Id = id;
        DepartmentId = departmentId;
        LocationId = locationId;
        IsPrimaryLocation = isPrimaryLocation;
    }

    public static Result<DepartmentLocation, Error> Create(Guid id, Guid departmentId, Guid locationId, bool isPrimaryLocation = false)
    {
        if (id == Guid.Empty)
        {
            return Error.Validation("department_location.id.invalid", "Id cannot be empty.", nameof(id));
        }

        if (departmentId == Guid.Empty)
        {
            return Error.Validation("department_location.department_id.invalid", "DepartmentId cannot be empty.", nameof(departmentId));
        }

        if (locationId == Guid.Empty)
        {
            return Error.Validation("department_location.location_id.invalid", "LocationId cannot be empty.", nameof(locationId));
        }

        return new DepartmentLocation(id, departmentId, locationId, isPrimaryLocation);
    }

    public Guid Id { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid LocationId { get; private set; }
    public bool IsPrimaryLocation { get; private set; }

    public void UpdatePrimaryStatus(bool isPrimaryLocation)
    {
        IsPrimaryLocation = isPrimaryLocation;
    }
}
