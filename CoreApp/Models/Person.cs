namespace CoreApp.Models;

public abstract class Person : EntityBase
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Pesel? Pesel { get; set; }
    public EmailAddress Email { get; set; } = EmailAddress.From("unknown@example.local");
}