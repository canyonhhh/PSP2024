namespace PSPOS.ServiceDefaults.Models;

public class BaseClass
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
}