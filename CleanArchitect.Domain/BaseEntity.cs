namespace CleanArchitect.Domain;

public abstract class BaseEntity
{
    protected BaseEntity(Guid id, Guid createBy)
    {
        Id = id;
        CreateBy = createBy;
        UpdateBy = createBy;
    }

    public Guid Id { get; }

    public Guid CreateBy { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Guid UpdateBy { get; private set; }
    public DateTimeOffset ModifiedAt { get; set; }

    protected void MarkUpdated(Guid updateBy)
    {
        UpdateBy = updateBy;
    }
}
