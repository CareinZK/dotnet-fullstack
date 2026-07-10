namespace DirectoryService.Domain.Departments;

public class DepartmentPositions
{
	// Parameterless constructor for EF Core materialization
	private DepartmentPositions()
	{
		PositionIds = [];
	}

public DepartmentPositions(Guid id, Guid departmentId, IReadOnlyList<Guid> positionIds)
	{
 		if (id != Guid.Empty)
			Id = id;
        else       
        	throw new ArgumentException("Id cannot be empty.", nameof(id));
 		if (departmentId != Guid.Empty)
			DepartmentId = departmentId;
        else       
        	throw new ArgumentException("DepartmentId cannot be empty.", nameof(departmentId));
 		if (positionIds.All(g => g != Guid.Empty))
			PositionIds = positionIds.ToList();
        else       
        	throw new ArgumentException("PositionIds cannot be empty.", nameof(positionIds));
        
	}
    
    public Guid Id { get; private set; }
    public Guid DepartmentId { get; private set; }

    public IReadOnlyList<Guid> PositionIds { get; private set; }
    

}
