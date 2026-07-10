namespace DirectoryService.Domain.Departments;
public class DepartmentLocation
{
	// Parameterless constructor for EF Core materialization
	private DepartmentLocation()
	{
	}

	public DepartmentLocation(Guid id, Guid departmentId, Guid locationId, bool isPrimaryLocation)
	{
 		if (id != Guid.Empty)
			Id = id;
        else       
        	throw new ArgumentException("Id cannot be empty.", nameof(id));
 		if (departmentId != Guid.Empty)
			DepartmentId = departmentId;
        else       
        	throw new ArgumentException("DepartmentId cannot be empty.", nameof(departmentId));
 		if (locationId != Guid.Empty)
			LocationId = locationId;
        else       
        	throw new ArgumentException("LocationId cannot be empty.", nameof(locationId));
        IsPrimaryLocation = isPrimaryLocation;
	}
    
    public Guid Id { get; private set; }
    public Guid DepartmentId { get; private set; }

    public Guid LocationId { get; private set; }
    
    public bool IsPrimaryLocation {get; private set; }

    public void UpdatePrimaryStatus (bool isPrimaryLocation)
	{
		IsPrimaryLocation = isPrimaryLocation;
	}

    
}
