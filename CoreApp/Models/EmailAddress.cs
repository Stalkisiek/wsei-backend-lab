namespace CoreApp.Models;

public sealed class EmailAddress : ValueObject
{
    private readonly string _value;

    private EmailAddress(string value)
    {
        _value = value;
    }

    public string User => _value.Split('@')[0];

    public string Domain => _value.Split('@')[1];

    public static EmailAddress From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.");

        var trimmed = value.Trim().ToLowerInvariant();
        var atIndex = trimmed.IndexOf('@');

        if (atIndex <= 0 || atIndex != trimmed.LastIndexOf('@') || atIndex == trimmed.Length - 1)
            throw new ArgumentException("Invalid email format.");

        return new EmailAddress(trimmed);
    }

    public static EmailAddress Parse(string value)
    {
        return From(value);
    }

    public static EmailAddress? FromOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        try
        {
            return From(value);
        }
        catch
        {
            return null;
        }
    }

    public static bool TryFrom(string? value, out EmailAddress? emailAddress)
    {
        emailAddress = null;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            emailAddress = From(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string Format()
    {
        return _value;
    }

    public override string ToString()
    {
        return _value;
    }

    public static implicit operator string(EmailAddress emailAddress)
    {
        return emailAddress._value;
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return _value;
    }
}

