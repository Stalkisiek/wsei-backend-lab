using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Infrastucture.EntityFramework.Entities;
using CoreApp.Models;
using Microsoft.Extensions.Configuration;

namespace Infrastucture.EntityFramework.Context;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lecturer> Lecturers { get; set; }
    public DbSet<AcademicYear> AcademicYears { get; set; }
    public DbSet<DegreeProgram> DegreePrograms { get; set; }
    public DbSet<Grade> Grades { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            try
            {
                var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                var config = new ConfigurationBuilder()
                    .AddJsonFile(configPath, optional: true)
                    .Build();
                var cs = config.GetConnectionString("AppDb");
                optionsBuilder.UseSqlite(string.IsNullOrEmpty(cs) ? "Data Source=app.db" : cs);
            }
            catch
            {
                optionsBuilder.UseSqlite("Data Source=app.db");
            }
        }
    }

    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
            entity.Property(u => u.Department).HasMaxLength(100);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        builder.Entity<AppRole>(entity =>
        {
            entity.Property(r => r.Name).HasMaxLength(20);
        });

        builder.Entity<Student>(entity => { entity.HasKey(s => s.Id); });
        builder.Entity<Course>(entity => { entity.HasKey(c => c.Id); });
        builder.Entity<Lecturer>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Title).HasMaxLength(50);
            entity.Property(l => l.Faculty).HasMaxLength(100);
        });
        builder.Entity<AcademicYear>(entity =>
        {
            entity.HasKey(y => y.Id);
            entity.Property(y => y.Name).HasMaxLength(50);
        });
        builder.Entity<DegreeProgram>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Code).HasMaxLength(20);
            entity.Property(p => p.Name).HasMaxLength(100);
            entity.Property(p => p.Faculty).HasMaxLength(100);
        });
        builder.Entity<Grade>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.HasOne(g => g.Student).WithMany(s => s.Grades);
            entity.HasOne(g => g.Course).WithMany();
            entity.HasOne(g => g.Lecturer).WithMany();
            entity.HasOne(g => g.AcademicYear).WithMany();
        });

        var adminRoleId = "11111111-1111-1111-1111-111111111111";
        var user1Id = "22222222-2222-2222-2222-222222222222";
        var user2Id = "33333333-3333-3333-3333-333333333333";

        builder.Entity<AppRole>().HasData(new AppRole
        {
            Id = adminRoleId,
            Name = UserRole.Administrator.ToString(),
            NormalizedName = UserRole.Administrator.ToString().ToUpper(),
            Description = "Administrator role",
            ConcurrencyStamp = Guid.NewGuid().ToString()
        });

        builder.Entity<AppUser>().HasData(
            new AppUser
            {
                Id = user1Id,
                UserName = "admin1",
                NormalizedUserName = "ADMIN1",
                Email = "admin1@example.local",
                NormalizedEmail = "ADMIN1@EXAMPLE.LOCAL",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "One",
                FullName = "Admin One",
                Department = "IT",
                Status = SystemUserStatus.Active,
                CreatedAt = DateTime.Parse("2026-05-24T00:00:00Z"),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new AppUser
            {
                Id = user2Id,
                UserName = "admin2",
                NormalizedUserName = "ADMIN2",
                Email = "admin2@example.local",
                NormalizedEmail = "ADMIN2@EXAMPLE.LOCAL",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "Two",
                FullName = "Admin Two",
                Department = "IT",
                Status = SystemUserStatus.Active,
                CreatedAt = DateTime.Parse("2026-05-24T00:00:00Z"),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );

        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { UserId = user1Id, RoleId = adminRoleId },
            new IdentityUserRole<string> { UserId = user2Id, RoleId = adminRoleId }
        );
    }
}

