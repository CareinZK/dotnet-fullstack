namespace TemplateService.Domain.Departments;
public class DepartmentLocation
{
	public DepartmentLocation(Guid id, Guid departmentId, IReadOnlyList<Guid> locationIds, bool isPrimaryLocation)
	{
 		if (id != Guid.Empty)
			Id = id;
        else       
        	throw new ArgumentException("Id cannot be empty.", nameof(id));
 		if (departmentId != Guid.Empty)
			DepartmentId = departmentId;
        else       
        	throw new ArgumentException("DepartmentId cannot be empty.", nameof(departmentId));
 		if (locationIds.All(g => g != Guid.Empty))
			LocationIds = locationIds.ToList();
        else       
        	throw new ArgumentException("LocationIds cannot be empty.", nameof(locationIds));
        IsPrimaryLocation = isPrimaryLocation;
	}
    
    public Guid Id { get; private init; }
    public Guid DepartmentId { get; private init; }
    
    public IReadOnlyList<Guid> LocationIds { get; private init; }
    
    public bool IsPrimaryLocation {get; private set; }

    public void UpdatePrimaryStatus (bool isPrimaryLocation)
	{
		IsPrimaryLocation = isPrimaryLocation;
	}

    
}
