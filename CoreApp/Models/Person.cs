namespace CoreApp.Models;

public abstract class Person : EntityBase
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Pesel? Pesel { get; set; }
    public string Email { get; set; } = string.Empty;
}