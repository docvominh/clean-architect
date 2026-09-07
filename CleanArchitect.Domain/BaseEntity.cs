namespace CleanArchitect.Domain;

public abstract class BaseEntity
{
    public Guid Id { get; init; }

    public Guid CreateBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Guid UpdateBy { get; set; }
    public DateTimeOffset ModifiedAt { get; set; }
}