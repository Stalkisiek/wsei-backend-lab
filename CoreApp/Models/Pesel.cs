namespace CoreApp.Models;

public sealed class Pesel : ValueObject
{
    private readonly string _value;

    private const int PeselLength = 11;
    private static readonly int[] Weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };

    private Pesel(string value)
    {
        _value = value.Trim();
    }

    public static Pesel From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("PESEL cannot be empty.");

        var trimmed = value.Trim();

        if (trimmed.Length != PeselLength)
            throw new ArgumentException($"PESEL must be exactly {PeselLength} digits long.");

        if (!trimmed.All(char.IsDigit))
            throw new ArgumentException("PESEL must contain only digits.");

        if (!IsValidChecksum(trimmed))
            throw new ArgumentException("PESEL checksum validation failed.");

        return new Pesel(trimmed);
    }

    public static Pesel? FromOrNull(string? value)
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
    
    public static bool TryFrom(string? value, out Pesel? pesel)
    {
        pesel = null;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            pesel = From(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    private static bool IsValidChecksum(string pesel)
    {
        int sum = 0;
        for (int i = 0; i < 10; i++)
        {
            sum += (pesel[i] - '0') * Weights[i];
        }

        int checksum = (10 - (sum % 10)) % 10;
        int lastDigit = pesel[10] - '0';

        return checksum == lastDigit;
    }
    
    public DateTime GetBirthDate()
    {
        int year = int.Parse(_value.Substring(0, 2));
        int month = int.Parse(_value.Substring(2, 2));
        int day = int.Parse(_value.Substring(4, 2));

        int centuryMark = (month / 10) % 8;
        int century = centuryMark switch
        {
            8 => 1800,
            0 => 1900,
            1 => 2000,
            _ => 1900
        };

        int actualMonth = month % 20;
        if (actualMonth > 12) actualMonth -= 20;

        int actualYear = century + year;

        try
        {
            return new DateTime(actualYear, actualMonth, day);
        }
        catch
        {
            throw new InvalidOperationException($"Invalid birth date in PESEL: {_value}");
        }
    }
    
    public char GetGender()
    {
        int genderDigit = int.Parse(_value.Substring(9, 1));
        return genderDigit % 2 == 0 ? 'F' : 'M';
    }
    
    public override string ToString() => _value;
    
    public string Value => _value;
    
    public override bool Equals(object? obj)
    {
        if (obj is not Pesel other)
            return false;
        return _value == other._value;
    }
    
    public override int GetHashCode() => _value.GetHashCode();

    public static bool operator ==(Pesel? left, Pesel? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Pesel? left, Pesel? right) => !(left == right);
    
    public static implicit operator string(Pesel pesel) => pesel._value;
    
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return _value;
    }
}

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetAtomicValues();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
    }

    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Aggregate(default(int), (acc, value) =>
            {
                return HashCode.Combine(acc, value);
            });
    }

    public bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
}



