using Api.Domain.Common;
using Api.Domain.ValueObjects;

namespace Api.Domain.Entities;

public class User : Entity<UserId>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public bool IsDeleted { get; private set; }

    private User(UserId id, string firstName, string lastName, Email email) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        IsDeleted = false;
    }

    // Requerido por EF Core
    #pragma warning disable CS8618
    private User() {}
    #pragma warning restore CS8618

    public static User Create(string firstName, string lastName, Email email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("El apellido no puede estar vacío.", nameof(lastName));

        ArgumentNullException.ThrowIfNull(email);

        return new User(UserId.From(Guid.NewGuid()), firstName, lastName, email);
    }

    public void UpdateDetails(string firstName, string lastName, Email email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("El apellido no puede estar vacío.", nameof(lastName));

        ArgumentNullException.ThrowIfNull(email);

        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public void Delete()
    {
        IsDeleted = true;
    }
}
