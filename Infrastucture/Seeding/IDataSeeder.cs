namespace Infrastucture.Seeding;

public interface IDataSeeder
{
    int Order { get; }
    Task SeedAsync();
}

