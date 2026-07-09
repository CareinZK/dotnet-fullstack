namespace TemplateService.Domain.Departments;

public class DepartmentPosition	
{
public DepartmentPosition(Guid id, Guid departmentId, IReadOnlyList<Guid> positionIds)
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
    
    public Guid Id { get; private init; }
    public Guid DepartmentId { get; private init; }
    
    public IReadOnlyList<Guid> PositionIds { get; private init; }
    

}
