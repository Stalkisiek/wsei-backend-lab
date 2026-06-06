using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Infrastucture.EntityFramework.Entities;
using Infrastucture.Security;
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
    public DbSet<GradeChangeHistory> GradeChangeHistories { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

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

        builder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Email)
                .HasConversion(
                    e => e.ToString(),
                    s => EmailAddress.From(s)
                )
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(s => s.Pesel)
                .HasConversion(
                    p => p != null ? p.ToString() : null,
                    s => Pesel.FromOrNull(s)
                )
                .HasColumnName("NationalId")
                .IsRequired(false);
        });

        builder.Entity<Lecturer>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Title).HasMaxLength(50);
            entity.Property(l => l.Faculty).HasMaxLength(100);
            entity.Property(l => l.Email)
                .HasConversion(
                    e => e.ToString(),
                    s => EmailAddress.From(s)
                )
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(l => l.Pesel)
                .HasConversion(
                    p => p != null ? p.ToString() : null,
                    s => Pesel.FromOrNull(s)
                )
                .HasColumnName("NationalId")
                .IsRequired(false);
        });

        builder.Entity<Course>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasOne(c => c.Lecturer)
                .WithMany(l => l.TaughtCorses)
                .OnDelete(DeleteBehavior.SetNull);
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
            
            entity.Property(g => g.CreatedBy).HasMaxLength(450);
            entity.Property(g => g.ModifiedBy).HasMaxLength(450);
            entity.Property(g => g.CreatedAt).HasDefaultValue(DateTime.UtcNow);
        });

        builder.Entity<GradeChangeHistory>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.HasOne(h => h.Grade)
                .WithMany(g => g.ChangeHistory)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(h => h.ChangedBy).HasMaxLength(450).IsRequired();
            entity.Property(h => h.ChangedAt).HasDefaultValue(DateTime.UtcNow);
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.UserId).HasMaxLength(450).IsRequired();
            entity.Property(rt => rt.Token).IsRequired();
            entity.HasIndex(rt => rt.Token).IsUnique();
            entity.HasIndex(rt => rt.UserId);
            entity.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        var adminRoleId = "11111111-1111-1111-1111-111111111111";
        var lecturerRoleId = "22222222-2222-2222-2222-111111111111";
        var deanOfficeRoleId = "33333333-3333-3333-3333-111111111111";
        var user1Id = "22222222-2222-2222-2222-222222222222";
        var user2Id = "33333333-3333-3333-3333-333333333333";
        var user3Id = "44444444-4444-4444-4444-444444444444";
        var user4Id = "55555555-5555-5555-5555-555555555555";

        builder.Entity<AppRole>().HasData(
            new AppRole
            {
                Id = adminRoleId,
                Name = UserRole.Administrator.ToString(),
                NormalizedName = UserRole.Administrator.ToString().ToUpper(),
                Description = "Administrator role",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new AppRole
            {
                Id = lecturerRoleId,
                Name = UserRole.Lecturer.ToString(),
                NormalizedName = UserRole.Lecturer.ToString().ToUpper(),
                Description = "Lecturer role",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new AppRole
            {
                Id = deanOfficeRoleId,
                Name = UserRole.DeanOffice.ToString(),
                NormalizedName = UserRole.DeanOffice.ToString().ToUpper(),
                Description = "Dean Office role",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );

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
            },
            new AppUser
            {
                Id = user3Id,
                UserName = "lecturer1",
                NormalizedUserName = "LECTURER1",
                Email = "lecturer1@example.local",
                NormalizedEmail = "LECTURER1@EXAMPLE.LOCAL",
                EmailConfirmed = true,
                FirstName = "Jan",
                LastName = "Kowal",
                FullName = "Jan Kowal",
                Department = "IT",
                Status = SystemUserStatus.Active,
                CreatedAt = DateTime.Parse("2026-05-24T00:00:00Z"),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new AppUser
            {
                Id = user4Id,
                UserName = "deanoffice1",
                NormalizedUserName = "DEANOFFICE1",
                Email = "deanoffice1@example.local",
                NormalizedEmail = "DEANOFFICE1@EXAMPLE.LOCAL",
                EmailConfirmed = true,
                FirstName = "Magda",
                LastName = "Dziekan",
                FullName = "Magda Dziekan",
                Department = "Dean Office",
                Status = SystemUserStatus.Active,
                CreatedAt = DateTime.Parse("2026-05-24T00:00:00Z"),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );

        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { UserId = user1Id, RoleId = adminRoleId },
            new IdentityUserRole<string> { UserId = user2Id, RoleId = adminRoleId },
            new IdentityUserRole<string> { UserId = user3Id, RoleId = lecturerRoleId },
            new IdentityUserRole<string> { UserId = user4Id, RoleId = deanOfficeRoleId }
        );
    }
}

