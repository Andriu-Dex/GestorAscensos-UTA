namespace SGA.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        FechaCreacion = DateTime.UtcNow;
    }
}
