namespace CleanArchitect.Domain.UserAggregate;

public class User : BaseEntity
{
    public string? DisplayName { get; set; }

    public List<UserAddress> Addresses { get; set; } = [];
}
