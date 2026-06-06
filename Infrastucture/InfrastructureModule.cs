using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Infrastucture.EntityFramework.Context;
using Infrastucture.EntityFramework.Entities;
using Infrastucture.EntityFramework.Repositories;
using Infrastucture.EntityFramework.UnitOfWork;
using Infrastucture.Seeding;
using Infrastucture.Security;
using CoreApp.Authorization;
using CoreApp.Models;
using CoreApp.Repositories;
using CoreApp.Services;

namespace Infrastucture;

public static class InfrastructureModule
{
    public static IServiceCollection AddUniversityEfModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("AppDb") ?? "Data Source=app.db"));

        services.AddIdentity<AppUser, AppRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IStudentRepository, EfStudentRepository>();
        services.AddScoped<ILecturerRepository, EfLecturerRepository>();
        services.AddScoped<IGradeRepository, EfGradeRepository>();
        services.AddScoped<ICourseRepository, EfCourseRepository>();
        services.AddScoped<IAcademicYearRepository, EfAcademicYearRepository>();
        services.AddScoped<IDegreeProgramRepository, EfDegreeProgramRepository>();

        services.AddScoped<EfStudentRepository>();
        services.AddScoped<EfLecturerRepository>();
        services.AddScoped<EfGradeRepository>();
        services.AddScoped<EfCourseRepository>();
        services.AddScoped<EfAcademicYearRepository>();
        services.AddScoped<EfDegreeProgramRepository>();

        services.AddScoped<IUniversityUnitOfWork, EfUniversityUnitOfWork>();

        services.AddScoped<CoreApp.Services.IStudentService, CoreApp.Services.StudentService>();
        services.AddScoped<CoreApp.Services.ILecturerService, CoreApp.Services.LecturerService>();
        services.AddScoped<CoreApp.Services.IDegreeProgramService, CoreApp.Services.DegreeProgramService>();
        services.AddScoped<CoreApp.Services.ICourseManagementService, CoreApp.Services.CourseManagementService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDataSeeder, DatabaseSeeder>();


        return services;
    }

    public static IServiceCollection AddJwt(this IServiceCollection services, JwtSettings jwtOptions)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = jwtOptions.GetSymmetricKey(),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AppPolicies.Administrator.Name(), policy =>
                policy.RequireRole(UserRole.Administrator.ToString()));

            options.AddPolicy(AppPolicies.AdminOnly.Name(), policy =>
                policy.RequireRole(UserRole.Administrator.ToString()));

            options.AddPolicy(AppPolicies.ActiveUser.Name(), policy =>
                policy
                    .RequireAuthenticatedUser()
                    .RequireClaim("status", SystemUserStatus.Active.ToString()));

            options.AddPolicy(AppPolicies.SalesDepartment.Name(), policy =>
                policy.RequireClaim("department", "Sales"));

            options.AddPolicy(AppPolicies.Lecturer.Name(), policy =>
                policy.RequireRole(UserRole.Lecturer.ToString()));

            options.AddPolicy(AppPolicies.DeanOffice.Name(), policy =>
                policy.RequireRole(UserRole.DeanOffice.ToString()));

            options.AddPolicy(AppPolicies.LecturerOrDeanOffice.Name(), policy =>
                policy.RequireRole(UserRole.Lecturer.ToString(), UserRole.DeanOffice.ToString()));

            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }

    public static IServiceCollection AddUniversityMemoryModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<CoreApp.Repositories.IStudentRepository, Infrastucture.Memory.MemoryStudentRepository>();
        services.AddScoped<CoreApp.Repositories.ILecturerRepository, Infrastucture.Memory.MemoryLecturerRepository>();
        services.AddScoped<CoreApp.Repositories.IGradeRepository, Infrastucture.Memory.MemoryGradeRepository>();
        services.AddScoped<CoreApp.Repositories.ICourseRepository, Infrastucture.Memory.MemoryCourseRepository>();
        services.AddScoped<CoreApp.Repositories.IAcademicYearRepository, Infrastucture.Memory.MemoryAcademicYearRepository>();
        services.AddScoped<CoreApp.Repositories.IDegreeProgramRepository, Infrastucture.Memory.MemoryDegreeProgramRepository>();

        services.AddScoped<CoreApp.Repositories.IUniversityUnitOfWork, Infrastucture.Repository.MemoryUniversityUnitOfWork>(sp =>
        {
            var students = sp.GetRequiredService<CoreApp.Repositories.IStudentRepository>();
            var lecturers = sp.GetRequiredService<CoreApp.Repositories.ILecturerRepository>();
            var grades = sp.GetRequiredService<CoreApp.Repositories.IGradeRepository>();
            var courses = sp.GetRequiredService<CoreApp.Repositories.ICourseRepository>();
            var years = sp.GetRequiredService<CoreApp.Repositories.IAcademicYearRepository>();
            var programs = sp.GetRequiredService<CoreApp.Repositories.IDegreeProgramRepository>();
            return new Infrastucture.Repository.MemoryUniversityUnitOfWork(students, lecturers, grades, courses, years, programs);
        });

        services.AddScoped<CoreApp.Services.IStudentService, CoreApp.Services.StudentService>();
        services.AddScoped<CoreApp.Services.IDegreeProgramService, CoreApp.Services.DegreeProgramService>();
        services.AddScoped<CoreApp.Services.ICourseManagementService, CoreApp.Services.CourseManagementService>();

        return services;
    }
}


