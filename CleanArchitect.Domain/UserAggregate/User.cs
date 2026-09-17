namespace CleanArchitect.Domain.UserAggregate;

public class User : BaseEntity
{
    public User(Guid id, Guid createBy, string? displayName = null) : base(id, createBy)
    {
        DisplayName = displayName;
    }

    public string? DisplayName { get; private set; }

    public List<UserAddress> Addresses { get; } = [];

    public void UpdateDisplayName(string? displayName)
    {
        DisplayName = displayName;
    }

    public void UpdateAddress(IReadOnlyCollection<UserAddress> addresses)
    {
        // Mutate the tracked list in place rather than replacing the reference, so EF Core's
        // collection change tracking can correctly diff old (removed) vs new (added) addresses.
        Addresses.Clear();
        Addresses.AddRange(addresses);
    }
}
