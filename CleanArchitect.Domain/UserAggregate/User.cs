namespace CleanArchitect.Domain.UserAggregate;

public class User : BaseEntity
{
    public User(Guid id, Guid createBy, string? displayName = null) : base(id, createBy)
    {
        DisplayName = displayName;
    }

    public string? DisplayName { get; init; }

    public List<UserAddress> Addresses { get; private set; } = [];

    public void UpdateAddress(List<UserAddress> addresses)
    {
        Addresses = addresses;
    }
}
